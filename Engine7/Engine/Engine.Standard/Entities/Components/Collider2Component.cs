using Engine.Module.Entities.Container;

namespace Engine.Standard.Entities.Components;

public sealed class Collider2Component : ComponentBase {

	private readonly List<Vector2<double>> _colliderVertices = [];

	public IReadOnlyList<Vector2<double>> ColliderVertices => _colliderVertices;

	public void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) {
		_colliderVertices.Clear();
		_colliderVertices.AddRange( vertices );
		InvokeComponentChanged();
	}

	public void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) {
		_colliderVertices.Clear();
		_colliderVertices.AddRange( vertices );
		InvokeComponentChanged();
	}
}