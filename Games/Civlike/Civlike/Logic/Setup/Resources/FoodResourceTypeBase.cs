namespace Civlike.Logic.Setup.Resources;
public abstract class FoodResourceTypeBase( string name, IEnumerable<string> tags, float caloriesPerKg ) : ResourceTypeBase( name, [ .. tags, "food" ] ) {
	public float CaloriesPerKg { get; } = caloriesPerKg;
}
