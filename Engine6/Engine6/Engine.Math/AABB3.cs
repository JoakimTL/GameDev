using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.Operations;

namespace Engine.Math;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct AABB3<T> 
	where T : 
		unmanaged, INumber<T> {
	public readonly Vector3<T> Minima;
	public readonly Vector3<T> Maxima;

	public AABB3( Vector3<T> a, Vector3<T> b ) {
		Minima = a.Min( b );
		Maxima = a.Max( b );
	}
	public AABB3( AABB3<T> aabb, Vector3<T> v ) {
		Minima = aabb.Minima.Min( v );
		Maxima = aabb.Maxima.Max( v );
	}

	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is AABB3<T> aabb && Equals( aabb );
	public override int GetHashCode() => HashCode.Combine( this.Minima, this.Maxima );
	public bool Equals( AABB3<T> other ) => other.Minima == this.Minima && other.Maxima == this.Maxima;
	public static bool operator ==( AABB3<T> left, AABB3<T> right ) => left.Equals( right );
	public static bool operator !=( AABB3<T> left, AABB3<T> right ) => !left.Equals( right );
	public override string ToString() => $"{this.Minima} -> {this.Maxima}";
}

