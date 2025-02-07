using Sandbox.Logic.Research.TechnologyFields;

namespace Sandbox.Logic.Research.Technologies;

[Do<TechnologyBase>.After<StoneTools>]
public sealed class FlintKnapping() : TechnologyBase( "Flint Knapping", TechnologyKind.Improvement, 0, TechnologyFieldList.GetTechField<Toolmaking>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
