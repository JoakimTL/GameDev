using Sandbox.Logic.Research.TechnologyFields.Science;

namespace Sandbox.Logic.Research.TechnologyFields;


[SubfieldOf<Biology>]
public sealed class Zoology() : TechnologyFieldBase( "Zoology" );


[SubfieldOf<Agriculture>]
public sealed class Horticulture() : TechnologyFieldBase( "Horticulture" );

[SubfieldOf<Horticulture>]
public sealed class Botany() : TechnologyFieldBase( "Botany" );

[SubfieldOf<Domestication>]
public sealed class Aquaculture() : TechnologyFieldBase( "Aquaculture" );

public sealed class Subsistence() : TechnologyFieldBase( "Subsistence" );

public sealed class Domestication() : TechnologyFieldBase( "Domestication" );

public sealed class BroadField() : TechnologyFieldBase( "Broad Technology Field" );

[SubfieldOf<BroadField>]
public sealed class NarrowedDownField() : TechnologyFieldBase( "Narrowed Down Technology Field" );

public sealed class Toolmaking() : TechnologyFieldBase( "Toolmaking" );
public sealed class MaterialProcessing() : TechnologyFieldBase( "Material Processing" );

public sealed class StoneTools() : TechnologyBase( "Stone Tools", TechnologyKind.Research, requiredResearchProgress: 0, technologyFields: [ TechnologyFieldList.GetTechField<Toolmaking>(), TechnologyFieldList.GetTechField<MaterialProcessing>() ] ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyBase>.After<StoneTools>()]
public sealed class StoneKnapping() : TechnologyBase( "Stone Knapping", TechnologyKind.Improvement, 0, TechnologyFieldList.GetTechField<Toolmaking>(), TechnologyFieldList.GetTechField<MaterialProcessing>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1;
}
[Do<TechnologyBase>.After<StoneTools>()]
public sealed class BoneAndWoodTools() : TechnologyBase( "Bone and Wood Tools", TechnologyKind.Research, 0, TechnologyFieldList.GetTechField<Toolmaking>(), TechnologyFieldList.GetTechField<MaterialProcessing>() ) {
	public override float GetDiscoveryChanceModifier( TechnologyResearcher techHolder ) => 1;
	public override float GetResearchProgressionModifier( TechnologyResearcher techHolder ) => 1 + (techHolder.GetResearchFor<StoneKnapping>().ResearchCompleted ? 1 : 0);
}