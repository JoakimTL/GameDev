using Civlike.Logic.Setup;
using Civlike.Logic.Setup.Resources;

namespace Civlike.Logic.Setup.Recipes;

public sealed class SawmillingRecipe() : RecipeTypeBase( "Sawmilling",
	new( [
		(Definitions.Resources.Get<LogsResource>(), 1)
		] ),
	new( [
		(Definitions.Resources.Get<PlanksResource>(), 0.98),
		(Definitions.Resources.Get<SawdustResource>(), 0.02)
		] ),
	[ Definitions.Resources.Get<SawdustResource>() ] );

public sealed class MalachiteCrushingRecipe() : RecipeTypeBase( "Malachite Crushing",
	new( [
		(Definitions.Resources.Get<MalachiteResource>(), 1)
		] ),
	new( [
		(Definitions.Resources.Get<CopperConcentrateResource>(), 0.98),
		(Definitions.Resources.Get<SawdustResource>(), 0.02)
		] ),
	[ Definitions.Resources.Get<SawdustResource>() ] );

