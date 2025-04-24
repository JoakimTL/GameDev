using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations.Taxes;
internal class Class1 {
}

public abstract class CurrencyBasedTaxationBase( string name, CurrencyBase currency ) : TaxationBase( name ) {
	public CurrencyBase Currency { get; set; } = currency;
}

public abstract class CommodityBasedTaxationBase( string name, ResourceTable commodities ) : TaxationBase( name ) {
	public ResourceTable Commodities { get; set; } = commodities;
}