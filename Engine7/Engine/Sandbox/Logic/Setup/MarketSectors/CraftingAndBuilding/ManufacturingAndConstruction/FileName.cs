namespace Sandbox.Logic.Setup.MarketSectors.CraftingAndBuilding.Manufacturing;

[Guid( "B1249AC4-3C51-41C0-80E7-771ACB765294" )]
public sealed class LaborerProfession() : ProfessionTypeBase<ManufacturingAndConstructionVocation>( "Laborer" );

[Guid( "38A5DECB-C398-4687-B9BD-8F5760117741" )]
public sealed class MachinistProfession() : ProfessionTypeBase<ManufacturingAndConstructionVocation>( "Machinist" );

[Guid( "B7015AC8-4730-42C3-A7DB-0BFE3FE839F5" )]
public sealed class ProcessOperatorProfession() : ProfessionTypeBase<ManufacturingAndConstructionVocation>( "Process Operator" );

[Guid( "63BE5CA3-6909-4153-82D4-DFC2ABB39FD2" )]
public sealed class QualityControlProfession() : ProfessionTypeBase<ManufacturingAndConstructionVocation>( "Quality Control" );