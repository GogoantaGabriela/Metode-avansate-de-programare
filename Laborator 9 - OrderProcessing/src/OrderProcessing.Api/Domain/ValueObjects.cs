namespace OrderProcessing.Api.Domain;

public record OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public string Short() => Value.ToString()[..8];
    public override string ToString() => Value.ToString();
}

public record Money(decimal Amount, string Currency = "RON")
{
    public static Money Zero => new(0m);
    public Money Add(Money other) => this with { Amount = Amount + other.Amount };
    public override string ToString() => $"{Amount:F2} {Currency}";
}

public record Address(string Street, string City, string PostalCode, string Country);

public record Customer(Guid Id, string Name, string Email, int Age, bool IsTrusted);

public record OrderItem(
    Guid ProductId,
    string ProductName,
    int Quantity,
    Money UnitPrice,
    bool HasAgeRestriction = false)
{
    public Money Subtotal => new(UnitPrice.Amount * Quantity, UnitPrice.Currency);
}
