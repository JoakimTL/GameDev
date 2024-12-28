using Engine.Transforms;
using System.Numerics;

namespace Engine.Physics;

public sealed class ConvexShape<TVector, TScalar> : ConvexShapeBase<TVector, TScalar>
	where TVector : unmanaged, ITransformableVector<Matrix4x4<TScalar>, TVector>, IInnerProduct<TVector, TScalar>
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
	public ConvexShape( ReadOnlySpan<TVector> vertices ) : base( vertices ) { }

	public ConvexShape( IEnumerable<TVector> vertices ) : base( vertices ) { }

	public new void SetBaseVertices( ReadOnlySpan<TVector> vertices ) => base.SetBaseVertices( vertices );
	public new void SetBaseVertices( IEnumerable<TVector> vertices ) => base.SetBaseVertices( vertices );
	public new void SetTransform( IMatrixProvider<TScalar>? transform ) => base.SetTransform( transform );
}
