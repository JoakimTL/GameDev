using Engine.GameLogic.ECS;

namespace StandardPackage.ECS.Components;

public sealed class Mass3Component : ComponentBase {
	public float Mass { get; set; }
	//public IInertiaTensorProvider InertiaTensorProvider { get; set; }

	protected override string UniqueNameTag => $"{Mass}kg";

	public Mass3Component() {
		Mass = 1;
	}
}
