using Sandbox.Logic.Old.OldCiv.Research;
using Sandbox.Logic.Old.OldCiv.Resources.Materials;

namespace Sandbox.Logic.Old.OldCiv.Research.Technologies;

[Do<ResourceRequirement>.After<Wood>]
public sealed class FireMastery() : TechnologyBase( "Fire Mastery", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class PlantCultivation() : TechnologyBase( "Plant Cultivation", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyRequirement>.After<FireMastery>]
[Do<ResourceRequirement>.After<AnimalProducts>]
public sealed class CampfireCooking() : TechnologyBase( "Campfire Cooking", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class ClayWorking() : TechnologyBase( "Clay Working", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyRequirement>.After<ClayWorking>]
public sealed class EarthenwarePottery() : TechnologyBase( "Earthenware Pottery", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class WaterManagement() : TechnologyBase( "Water Management", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class Weaving() : TechnologyBase( "Weaving", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class HuntingAndTracking() : TechnologyBase( "Hunting and Tracking", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyRequirement>.After<FireMastery>]
public sealed class Firewood() : TechnologyBase( "Firewood", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyRequirement>.After<HuntingAndTracking>]
public sealed class HideProcessing() : TechnologyBase( "Hide Processing", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class Foraging() : TechnologyBase( "Foraging", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
public sealed class BasicToolmaking() : TechnologyBase( "Basic Toolmaking", TechnologyKind.Research, 0 ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}