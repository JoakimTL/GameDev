using Sandbox.Logic.Research.TechnologyFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research.Technologies;

[Do<TechnologyBase>.After<Fire>]
[Do<TechnologyBase>.After<StoneTools>]
public sealed class Firewood() : TechnologyBase( "Firewood", TechnologyKind.Improvement, 0, TechnologyFieldList.GetTechField<FireKeeping>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}

[Do<TechnologyBase>.After<Firewood>]
[Do<TechnologyBase>.After<FlintKnapping>]
public sealed class FireStarting() : TechnologyBase( "Fire Starting", TechnologyKind.Improvement, 0, TechnologyFieldList.GetTechField<FireKeeping>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}

[Do<TechnologyBase>.After<StoneTools>]
public sealed class SpearFishing() : TechnologyBase( "Spear Fishing", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Fishing>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}

[Do<TechnologyBase>.After<StoneTools>]
public sealed class Weaving() : TechnologyBase( "Weaving", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Fishing>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}

[Do<TechnologyBase>.After<StoneTools>]
public sealed class NetFishing() : TechnologyBase( "Net Fishing", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Fishing>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}


public sealed class Foraging() : TechnologyBase( "Foraging", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Gathering>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}

public sealed class Hunting() : TechnologyBase( "Hunting", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Animal>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
