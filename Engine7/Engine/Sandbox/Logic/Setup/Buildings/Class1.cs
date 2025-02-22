using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Setup.Buildings;
internal class Class1 {
}

[Guid("247779A5-09D8-43BD-BAF1-96E9D14D62A9")]
public sealed class TribalSiteType() : BuildingTypeBase( "Tribal Site", true );

public sealed class TribalSiteBuilding : BuildingBase<TribalSiteType> {
	public override int GetCurrentEmployment<TProfession>() {

	}

	public override int GetMaxEmployment<TProfession>() {

	}
}