using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Setup.MarketSectors.GatheringAndCultivation.Laborer;
internal class Class1 {
}


public sealed class BuilderProfession() : ProfessionTypeBase<LaborerVocation>( "Builder" );

public sealed class MasonProfession() : ProfessionTypeBase<LaborerVocation>( "Mason" );

public sealed class FarmerProfession() : ProfessionTypeBase<LaborerVocation>( "Farmer" );

public sealed class FishermanProfession() : ProfessionTypeBase<LaborerVocation>( "Fisherman" );

public sealed class HunterProfession() : ProfessionTypeBase<LaborerVocation>( "Hunter" );

public sealed class GathererProfession() : ProfessionTypeBase<LaborerVocation>( "Gatherer" );

public sealed class CarpenterProfession() : ProfessionTypeBase<LaborerVocation>( "Carpenter" );