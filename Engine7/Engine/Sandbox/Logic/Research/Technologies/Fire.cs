using Sandbox.Logic.Research.TechnologyFields;

namespace Sandbox.Logic.Research.Technologies;

public sealed class Fire() : TechnologyBase( "Fire", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<FireKeeping>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}