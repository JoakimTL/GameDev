using Sandbox.Logic.Research.TechnologyFields;

namespace Sandbox.Logic.Research.Technologies;

public sealed class StoneTools() : TechnologyBase( "Stone Tools", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Toolmaking>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}