using OrderProcessing.Api.States;

namespace OrderProcessing.Api.Domain;

public record OrderHistoryEntry(string FromState, string ToState, DateTime At);

public class Order
{
    public OrderId Id { get; private set; } = OrderId.New();
    public Customer Customer { get; private set; } = null!;
    public Address ShippingAddress { get; private set; } = null!;
    public List<OrderItem> Items { get; private set; } = new();
    public Money Total { get; private set; } = Money.Zero;
    public IOrderState CurrentState { get; internal set; } = null!;
    public string Status => CurrentState.Name;
    public List<OrderHistoryEntry> History { get; } = new();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    private Order() { }

    public static Order Create(Customer customer, Address address, IEnumerable<OrderItem> items)
    {
        var itemList = items.ToList();
        var total = itemList.Aggregate(Money.Zero, (acc, i) => acc.Add(i.Subtotal));
        var order = new Order
        {
            Id = OrderId.New(),
            Customer = customer,
            ShippingAddress = address,
            Items = itemList,
            Total = total,
            CurrentState = new PendingState()
        };
        return order;
    }

    public void Pay()     => CurrentState.Pay(this);
    public void Process() => CurrentState.Process(this);
    public void Ship()    => CurrentState.Ship(this);
    public void Deliver() => CurrentState.Deliver(this);
    public void Cancel()  => CurrentState.Cancel(this);

    internal void Transition(IOrderState newState)
    {
        History.Add(new OrderHistoryEntry(CurrentState.Name, newState.Name, DateTime.UtcNow));
        CurrentState = newState;
    }
}
