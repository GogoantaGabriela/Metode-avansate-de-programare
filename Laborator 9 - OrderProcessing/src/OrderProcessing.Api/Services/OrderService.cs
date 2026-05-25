using OrderProcessing.Api.Domain;
using OrderProcessing.Api.Validation;

namespace OrderProcessing.Api.Services;

public class CreateOrderRequest
{
    public string CustomerName  { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public int    CustomerAge   { get; set; }
    public bool   IsTrusted     { get; set; }
    public string Street        { get; set; } = "";
    public string City          { get; set; } = "";
    public string PostalCode    { get; set; } = "";
    public string Country       { get; set; } = "Romania";
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public Guid   ProductId        { get; set; }
    public string ProductName      { get; set; } = "";
    public int    Quantity         { get; set; }
    public decimal UnitPrice       { get; set; }
    public bool   HasAgeRestriction { get; set; }
}

public interface IOrderRepository
{
    void              Save(Order order);
    Order?            GetById(OrderId id);
    IReadOnlyList<Order> GetAll();
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<string, Order> _store = new();

    public void Save(Order order)       => _store[order.Id.ToString()] = order;
    public Order? GetById(OrderId id)   => _store.TryGetValue(id.ToString(), out var o) ? o : null;
    public IReadOnlyList<Order> GetAll() =>
        _store.Values.OrderByDescending(o => o.CreatedAt).ToList();
}

public class OrderService
{
    private readonly IOrderRepository        _repo;
    private readonly IOrderValidationHandler _chain;

    public OrderService(IOrderRepository repo, IOrderValidationHandler chain)
    {
        _repo  = repo;
        _chain = chain;
    }

    public (Order? Order, ValidationResult Validation) CreateOrder(CreateOrderRequest req)
    {
        var customer = new Customer(Guid.NewGuid(), req.CustomerName, req.CustomerEmail,
                                    req.CustomerAge, req.IsTrusted);
        var address  = new Address(req.Street, req.City, req.PostalCode, req.Country);
        var items    = req.Items.Select(i =>
            new OrderItem(i.ProductId, i.ProductName, i.Quantity,
                          new Money(i.UnitPrice), i.HasAgeRestriction)).ToList();

        var ctx    = new ValidationContext(customer, address, items);
        var result = _chain.Handle(ctx);
        if (!result.IsValid) return (null, result);

        var order = Order.Create(customer, address, items);
        _repo.Save(order);
        return (order, result);
    }

    public IReadOnlyList<Order> GetAllOrders() => _repo.GetAll();

    public Order? GetOrder(string id) =>
        Guid.TryParse(id, out var g) ? _repo.GetById(new OrderId(g)) : null;

    public Order PayOrder(string id)     { var o = Require(id); o.Pay();     _repo.Save(o); return o; }
    public Order ProcessOrder(string id) { var o = Require(id); o.Process(); _repo.Save(o); return o; }
    public Order ShipOrder(string id)    { var o = Require(id); o.Ship();    _repo.Save(o); return o; }
    public Order DeliverOrder(string id) { var o = Require(id); o.Deliver(); _repo.Save(o); return o; }
    public Order CancelOrder(string id)  { var o = Require(id); o.Cancel();  _repo.Save(o); return o; }

    private Order Require(string id) =>
        GetOrder(id) ?? throw new KeyNotFoundException($"Comanda cu ID '{id}' nu a fost găsită.");
}
