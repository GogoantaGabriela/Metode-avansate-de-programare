using OrderProcessing.Api.Domain;
using OrderProcessing.Api.Services;
using OrderProcessing.Api.States;

namespace OrderProcessing.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var grp = app.MapGroup("/orders").WithTags("Orders");

        grp.MapPost("", async (CreateOrderRequest req, OrderService svc) =>
        {
            var (order, validation) = svc.CreateOrder(req);
            if (!validation.IsValid)
                return Results.BadRequest(new { errors = validation.Errors });

            return Results.Created($"/orders/{order!.Id}", ToDto(order));
        })
        .WithName("CreateOrder")
        .WithSummary("Creează o comandă nouă (rulează pipeline-ul de validare Chain of Responsibility)");

        grp.MapGet("", (OrderService svc) => Results.Ok(svc.GetAllOrders().Select(ToDto)))
           .WithName("GetAllOrders")
           .WithSummary("Returnează toate comenzile");

        grp.MapGet("{id}", (string id, OrderService svc) =>
        {
            var o = svc.GetOrder(id);
            return o is null ? Results.NotFound(new { error = $"Comanda '{id}' nu există." })
                             : Results.Ok(ToDto(o));
        })
        .WithName("GetOrder")
        .WithSummary("Detalii comandă + istoricul de stări");

        grp.MapPost("{id}/pay",     (string id, OrderService svc) => Transition(() => svc.PayOrder(id)))
           .WithName("PayOrder").WithSummary("Pending → Confirmed");

        grp.MapPost("{id}/process", (string id, OrderService svc) => Transition(() => svc.ProcessOrder(id)))
           .WithName("ProcessOrder").WithSummary("Confirmed → Processing");

        grp.MapPost("{id}/ship",    (string id, OrderService svc) => Transition(() => svc.ShipOrder(id)))
           .WithName("ShipOrder").WithSummary("Processing → Shipped");

        grp.MapPost("{id}/deliver", (string id, OrderService svc) => Transition(() => svc.DeliverOrder(id)))
           .WithName("DeliverOrder").WithSummary("Shipped → Delivered (terminal)");

        grp.MapPost("{id}/cancel",  (string id, OrderService svc) => Transition(() => svc.CancelOrder(id)))
           .WithName("CancelOrder").WithSummary("→ Cancelled (dacă starea permite)");
    }

    private static IResult Transition(Func<Order> action)
    {
        try                                             { return Results.Ok(ToDto(action())); }
        catch (InvalidOrderTransitionException ex)      { return Results.Conflict(new { error = ex.Message }); }
        catch (KeyNotFoundException ex)                 { return Results.NotFound(new { error = ex.Message }); }
    }

    private static object ToDto(Order o) => new
    {
        id       = o.Id.ToString(),
        shortId  = o.Id.Short(),
        status   = o.Status,
        customer = new
        {
            o.Customer.Name, o.Customer.Email,
            o.Customer.Age,  o.Customer.IsTrusted
        },
        address = new
        {
            o.ShippingAddress.Street, o.ShippingAddress.City,
            o.ShippingAddress.PostalCode, o.ShippingAddress.Country
        },
        items = o.Items.Select(i => new
        {
            i.ProductName, i.Quantity,
            unitPrice  = i.UnitPrice.Amount,
            subtotal   = i.Subtotal.Amount,
            i.HasAgeRestriction
        }),
        total    = o.Total.Amount,
        currency = o.Total.Currency,
        history  = o.History.Select(h => new
        {
            fromState = h.FromState,
            toState   = h.ToState,
            at        = h.At.ToString("HH:mm:ss")
        }),
        createdAt = o.CreatedAt
    };
}
