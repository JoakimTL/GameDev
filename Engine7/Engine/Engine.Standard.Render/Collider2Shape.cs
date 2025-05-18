using Engine.Physics;
using Engine.Transforms;

namespace Engine.Standard.Render;

public sealed class Collider2Shape : ConvexShapeBase<Vector2<double>, double> {
	public Collider2Shape() : base( [] ) { }

	internal new void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetTransform( IMatrixProvider<double>? transform ) => base.SetTransform( transform );
}