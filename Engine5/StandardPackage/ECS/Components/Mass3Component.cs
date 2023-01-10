using Engine.ECS;

namespace StandardPackage.ECS.Components;

public sealed class Mass3Component : ComponentBase {
	public float Mass { get; set; }
	//public IInertiaTensorProvider InertiaTensorProvider { get; set; }

	public Mass3Component() {
		Mass = 1;
	}
}
