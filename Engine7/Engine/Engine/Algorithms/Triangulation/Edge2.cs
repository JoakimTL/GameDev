using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public readonly struct Edge2<TScalar>( Vector2<TScalar> a, Vector2<TScalar> b ) : IEqualityComparer<Edge2<TScalar>>
where TScalar : unmanaged, INumber<TScalar> {
	public readonly Vector2<TScalar> A = a;
	public readonly Vector2<TScalar> B = b;

	public bool HasVertex( Vector2<TScalar> p ) => A == p || B == p;

	public override bool Equals( [NotNullWhen( true )] object? obj ) {
		if (obj is Edge2<TScalar> edge)
			return Equals( this, edge );
		return base.Equals( obj );
	}

	public bool Equals( Edge2<TScalar> x, Edge2<TScalar> y ) => x.HasVertex( y.A ) && x.HasVertex( y.B );

	public int GetHashCode( [DisallowNull] Edge2<TScalar> obj ) => HashCode.Combine( obj.A, obj.B );

	public override string ToString() => $"[{A} -> {B}]";
}
