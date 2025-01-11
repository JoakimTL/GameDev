namespace Sandbox.Logic.Society;

public abstract class LawBase( LawCategory category, string displayName ) {
	public LawCategory Category { get; } = category;
	public string DisplayName { get; } = displayName;
}

public sealed class LawCategory {

	public static readonly LawCategory Civil = new( "Civil" );
	public static readonly LawCategory Criminal = new( "Criminal" );
	public static readonly LawCategory Religious = new( "Religious" );
	public static readonly LawCategory Military = new( "Military" );
	public static readonly LawCategory Economic = new( "Economic" );
	public static readonly LawCategory Social = new( "Social" );
	public static readonly LawCategory Political = new( "Political" );
	public static readonly LawCategory Environmental = new( "Environmental" );
	public static readonly LawCategory Health = new( "Health" );
	public static readonly LawCategory Safety = new( "Safety" );
	public static readonly LawCategory Education = new( "Education" );
	public static readonly LawCategory Labor = new( "Labor" );
	public static readonly LawCategory HumanRights = new( "Human Rights" );
	public static readonly LawCategory International = new( "International" );
	public static readonly LawCategory Customary = new( "Customary" );
	public static readonly LawCategory Common = new( "Common" );
	public static readonly LawCategory Statutory = new( "Statutory" );
	public static readonly LawCategory Administrative = new( "Administrative" );
	public static readonly LawCategory Constitutional = new( "Constitutional" );
	public static readonly LawCategory Case = new( "Case" );
	public static readonly LawCategory Equity = new( "Equity" );
	public static readonly LawCategory Contract = new( "Contract" );
	public static readonly LawCategory Tort = new( "Tort" );
	public static readonly LawCategory Property = new( "Property" );
	public static readonly LawCategory Succession = new( "Succession" );
	public static readonly LawCategory Family = new( "Family" );
	public static readonly LawCategory Trust = new( "Trust" );
	public static readonly LawCategory Corporate = new( "Corporate" );
	public static readonly LawCategory Bankruptcy = new( "Bankruptcy" );
	public static readonly LawCategory Tax = new( "Tax" );
	public static readonly LawCategory IntellectualProperty = new( "Intellectual Property" );
	public static readonly LawCategory Competition = new( "Competition" );
	public static readonly LawCategory Consumer = new( "Consumer" );
	public static readonly LawCategory Employment = new( "Employment" );

	private LawCategory( string displayName ) {
		this.DisplayName = displayName;
	}
	public string DisplayName { get; }
}