using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Shapes;

public readonly struct Edge3<TScalar> : IEqualityComparer<Edge3<TScalar>>
	where TScalar : unmanaged, INumber<TScalar> {
	public readonly Vector3<TScalar> A;
	public readonly Vector3<TScalar> B;

	public Edge3( Vector3<TScalar> a, Vector3<TScalar> b ) {
		this.A = a;
		this.B = b;
		if (this.A == this.B)
			throw new ArgumentException( "An edge cannot have the same vertex twice." );
	}

	public bool HasVertex( Vector3<TScalar> p ) => this.A == p || this.B == p;

	public override bool Equals( [NotNullWhen( true )] object? obj ) {
		if (obj is Edge3<TScalar> edge)
			return HasVertex( edge.A ) && HasVertex( edge.B );
		return base.Equals( obj );
	}

	public override int GetHashCode() => HashCode.Combine( this.A, this.B );

	public bool Equals( Edge3<TScalar> x, Edge3<TScalar> y ) => x.Equals( y );

	public int GetHashCode( [DisallowNull] Edge3<TScalar> obj ) => obj.GetHashCode();

	public override string ToString() => $"[{this.A} -> {this.B}]";

	public static bool operator ==( Edge3<TScalar> left, Edge3<TScalar> right ) => left.Equals( right );

	public static bool operator !=( Edge3<TScalar> left, Edge3<TScalar> right ) => !(left == right);
}
