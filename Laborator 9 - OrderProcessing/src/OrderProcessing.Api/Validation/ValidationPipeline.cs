using OrderProcessing.Api.Domain;

namespace OrderProcessing.Api.Validation;

public record ValidationResult(bool IsValid, List<string> Errors)
{
    public static ValidationResult Ok()             => new(true, new List<string>());
    public static ValidationResult Fail(string err) => new(false, new List<string> { err });
}

public record ValidationContext(Customer Customer, Address ShippingAddress, List<OrderItem> Items);

public interface IOrderValidationHandler
{
    IOrderValidationHandler SetNext(IOrderValidationHandler handler);
    ValidationResult Handle(ValidationContext context);
}

public abstract class BaseValidationHandler : IOrderValidationHandler
{
    private IOrderValidationHandler? _next;

    public IOrderValidationHandler SetNext(IOrderValidationHandler handler)
    {
        _next = handler;
        return handler; 
    }

    public ValidationResult Handle(ValidationContext context)
    {
        var result = Validate(context);
        if (!result.IsValid) return result;         
        return _next?.Handle(context) ?? ValidationResult.Ok();
    }

    protected abstract ValidationResult Validate(ValidationContext context);
}

public class StockValidationHandler : BaseValidationHandler
{
    public static readonly Dictionary<Guid, (string Name, int Stock)> Catalog = new()
    {
        { Guid.Parse("00000000-0000-0000-0000-000000000001"), ("Produs A", 100) },
        { Guid.Parse("00000000-0000-0000-0000-000000000002"), ("Produs B", 50)  },
        { Guid.Parse("00000000-0000-0000-0000-000000000003"), ("Produs C", 0)   }, 
        { Guid.Parse("00000000-0000-0000-0000-000000000004"), ("Produs D", 200) },
        { Guid.Parse("00000000-0000-0000-0000-000000000005"), ("Produs E 18+", 30)  },
    };

    protected override ValidationResult Validate(ValidationContext ctx)
    {
        foreach (var item in ctx.Items)
        {
            if (!Catalog.TryGetValue(item.ProductId, out var entry))
                return ValidationResult.Fail(
                    $"Produsul '{item.ProductName}' (ID: {item.ProductId}) nu există în catalog.");
            if (entry.Stock < item.Quantity)
                return ValidationResult.Fail(
                    $"Stoc insuficient pentru '{item.ProductName}': disponibil {entry.Stock}, cerut {item.Quantity}.");
        }
        return ValidationResult.Ok();
    }
}

public class PriceValidationHandler : BaseValidationHandler
{
    protected override ValidationResult Validate(ValidationContext ctx)
    {
        foreach (var item in ctx.Items)
        {
            if (item.UnitPrice.Amount <= 0)
                return ValidationResult.Fail(
                    $"Prețul pentru '{item.ProductName}' trebuie să fie pozitiv (primit: {item.UnitPrice.Amount}).");
            if (item.Quantity <= 0)
                return ValidationResult.Fail(
                    $"Cantitatea pentru '{item.ProductName}' trebuie să fie pozitivă (primită: {item.Quantity}).");
        }
        return ValidationResult.Ok();
    }
}

public class FraudDetectionHandler : BaseValidationHandler
{
    private const decimal MaxForUntrusted = 10_000m;
    private const int     MaxDistinctItems = 50;

    protected override ValidationResult Validate(ValidationContext ctx)
    {
        var total = ctx.Items.Sum(i => i.UnitPrice.Amount * i.Quantity);
        if (!ctx.Customer.IsTrusted && total > MaxForUntrusted)
            return ValidationResult.Fail(
                $"Comanda depășește limita de {MaxForUntrusted} RON pentru clienți ne-verificați " +
                $"(total calculat: {total:F2} RON). Marcați clientul ca trusted sau reduceți valoarea.");

        if (ctx.Items.Count > MaxDistinctItems)
            return ValidationResult.Fail(
                $"Comanda conține {ctx.Items.Count} produse distincte, limita maximă este {MaxDistinctItems}.");

        return ValidationResult.Ok();
    }
}

public class AgeVerificationHandler : BaseValidationHandler
{
    protected override ValidationResult Validate(ValidationContext ctx)
    {
        var restricted = ctx.Items.Where(i => i.HasAgeRestriction).ToList();
        if (restricted.Any() && ctx.Customer.Age < 18)
            return ValidationResult.Fail(
                $"Comanda conține produse cu restricție de vârstă " +
                $"({string.Join(", ", restricted.Select(r => r.ProductName))}), " +
                $"dar clientul '{ctx.Customer.Name}' are {ctx.Customer.Age} ani (minim 18).");

        return ValidationResult.Ok();
    }
}
