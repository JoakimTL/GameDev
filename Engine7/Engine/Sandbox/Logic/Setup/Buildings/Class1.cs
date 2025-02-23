using System.Runtime.InteropServices;

namespace Sandbox.Logic.Setup.Buildings;
internal class Class1 {
}

[Guid("247779A5-09D8-43BD-BAF1-96E9D14D62A9")]
public sealed class TribalSiteType() : BuildingTypeBase( "Tribal Site", null, true );

public sealed class TribalSiteBuilding : BuildingBase<TribalSiteType> {
	public override int GetCurrentEmployment<TProfession>() {
		return 0;
	}

	public override int GetMaxEmployment<TProfession>() {
		return 0; 
	}
}