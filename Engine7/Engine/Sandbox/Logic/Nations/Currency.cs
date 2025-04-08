using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations;
public abstract class CurrencyBase( ResourceTypeBase? backingCommodity, string name ) {
	public ResourceTypeBase? BackingCommodity { get; } = backingCommodity;
	/// <summary>
	/// How much of the backing resource would a single unit of this currency be worth.
	/// </summary>
	public double BackingAmount { get; set; }
	public string Name { get; set; } = name;
}

public sealed class CurrencyExchange( CurrencyBase from, CurrencyBase to, double rate ) {
	public CurrencyBase From { get; } = from;
	public CurrencyBase To { get; } = to;
	public double Rate { get; set; } = rate;
}