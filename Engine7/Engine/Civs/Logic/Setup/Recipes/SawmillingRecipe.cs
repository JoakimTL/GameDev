using Civs.Logic.Setup.Resources;

namespace Civs.Logic.Setup.Recipes;

public sealed class SawmillingRecipe() : RecipeTypeBase( "Sawmilling",
	new( [
		(Definitions.Resources.Get<WoodResource>(), 1)
		] ),
	new( [
		(Definitions.Resources.Get<PlanksResource>(), 0.98),
		(Definitions.Resources.Get<SawdustResource>(), 0.02)
		] ),
	[ Definitions.Resources.Get<SawdustResource>() ] );

