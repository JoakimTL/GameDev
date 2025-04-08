using Sandbox.Logic.Setup.MarketSectors.GatheringAndCultivation.FishingAndGathering;
using System.Runtime.InteropServices;

namespace Sandbox.Logic.Setup.Buildings;
internal class Class1 {
}

[Guid("247779A5-09D8-43BD-BAF1-96E9D14D62A9")]
public sealed class TribalSiteType() : BuildingTypeBase( "Tribal Site", null, true );

public sealed class TribalSiteBuilding : BuildingBase<TribalSiteType> {

	private readonly Dictionary<ProfessionTypeBase, int> _wantedEmployment;

	public TribalSiteBuilding() {
		_wantedEmployment = [];
	}

	public void SetHunters( int count ) => SetMaxEmployment<HunterProfession>( count );
	public void SetForagers( int count ) => SetMaxEmployment<ForagerProfession>( count );

	private void SetMaxEmployment<TProfession>(int count) where TProfession : ProfessionTypeBase {
		if (count < 0)
			throw new ArgumentOutOfRangeException( nameof( count ), "Employment count must be positive." );
		_wantedEmployment.Add( Definitions.Professions.Get<TProfession>(), count );
	}

	public override int GetCurrentEmployment( ProfessionTypeBase professionType ) {
		return 0;
	}

	public override int GetMaxEmployment( ProfessionTypeBase professionType ) => _wantedEmployment.GetValueOrDefault( professionType );
}