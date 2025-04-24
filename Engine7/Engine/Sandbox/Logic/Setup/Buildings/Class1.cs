using Sandbox.Logic.Setup.MarketSectors.GatheringAndCultivation.FishingAndGathering;

namespace Sandbox.Logic.Setup.Buildings;
internal class Class1 {
}

[Guid( "247779A5-09D8-43BD-BAF1-96E9D14D62A9" )]
public sealed class TribalSiteType() : BuildingTypeBase( "Tribal Site", null, true );

public sealed class TribalSiteBuilding : BuildingBase<TribalSiteType> {

	private readonly Dictionary<ProfessionTypeBase, int> _wantedEmployment;

	public TribalSiteBuilding() {
		_wantedEmployment = [];
	}

	public void SetHunters( int count ) => this.Employer.SetWantedEmployment<HunterProfession>( count );
	public void SetForagers( int count ) => this.Employer.SetWantedEmployment<ForagerProfession>( count );


}