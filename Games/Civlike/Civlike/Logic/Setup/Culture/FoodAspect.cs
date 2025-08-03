using Civlike.Logic.Setup.Resources;

namespace Civlike.Logic.Setup.Culture;
public sealed class FoodAspect : CultureAspectBase {

	private FoodAspect( FoodResourceTypeBase foodResource ) {
		this.FoodResource = foodResource;
	}

	public FoodResourceTypeBase FoodResource { get; }

	public static FoodAspect Create<T>() where T : FoodResourceTypeBase, new() => new( Definitions.Resources.Get<T>() );
}
