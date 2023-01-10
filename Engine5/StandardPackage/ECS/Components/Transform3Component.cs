using Engine.Datatypes.Transforms;
using Engine.ECS;

namespace StandardPackage.ECS.Components;
public sealed class Transform3Component : ComponentBase {
	public readonly Transform3 Transform;

	public Transform3Component() {
		Transform = new();
	}
}
