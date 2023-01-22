using Engine.Datatypes;
using Engine.Datatypes.Transforms;

namespace Engine.GameLogic.ECS.Components;
public sealed class Transform3Component : ComponentBase {
	public readonly Transform3 Transform;

	protected override string UniqueNameTag => $"{Transform}";

	public Transform3Component() {
		Transform = new();
		Transform.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider obj ) => AlertComponentChanged();
	protected override void OnDispose() => Transform.MatrixChanged -= OnMatrixChanged;
}
