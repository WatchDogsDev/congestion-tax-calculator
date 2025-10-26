using Congestion.Calculator.Enums;

namespace Congestion.Calculator.Types;

public readonly struct Money(decimal amount, Currency currency)
{
    public decimal Amount { get; } = amount;
    public Currency Currency { get; } = currency;

    public static implicit operator decimal(Money money) => money.Amount;
    public override string ToString() => $"{Amount} {Currency}";
}