using Engine.Datatypes;
using Engine.Datatypes.Transforms;

namespace Engine.GameLogic.ECS.Components;

public sealed class Transform2Component : ComponentBase {
	public readonly Transform2 Transform;

	protected override string UniqueNameTag => $"{Transform}";

	public Transform2Component() {
		Transform = new();
		Transform.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider obj ) => AlertComponentChanged();
	protected override void OnDispose() => Transform.MatrixChanged -= OnMatrixChanged;
}
