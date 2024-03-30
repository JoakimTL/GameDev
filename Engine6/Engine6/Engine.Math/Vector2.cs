using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.Interfaces;
using Engine.Math.Operations;

namespace Engine.Math;

public readonly struct Vector2<T>( T x, T y ) :
		ILinearOperators<Vector2<T>, T>,
		IVectorEntrywiseOperators<Vector2<T>, T>,
		IProductOperator<Vector2<T>, Bivector2<T>, Vector2<T>>,
		IAdditiveIdentity<Vector2<T>, Vector2<T>>,
		IMultiplicativeIdentity<Vector2<T>, Vector2<T>>
	where T :
		unmanaged, INumber<T> {

	public readonly T X = x;
	public readonly T Y = y;

	public Multivector2<T> GetMultivector() => new( T.Zero, this, T.Zero );

	public static Vector2<T> AdditiveIdentity => Zero;
	public static Vector2<T> MultiplicativeIdentity => One;

	public static readonly Vector2<T> Zero = new( T.Zero, T.Zero );
	public static readonly Vector2<T> One = new( T.One, T.One );

	public static Vector2<T> operator -( in Vector2<T> l ) => l.Negate();
	public static Vector2<T> operator +( in Vector2<T> l, in Vector2<T> r ) => l.Add( r );
	public static Vector2<T> operator -( in Vector2<T> l, in Vector2<T> r ) => l.Subtract( r );
	public static Vector2<T> operator *( in Vector2<T> l, T r ) => l.ScalarMultiply( r );
	public static Vector2<T> operator *( T l, in Vector2<T> r ) => r.ScalarMultiply( l );
	public static Vector2<T> operator /( in Vector2<T> l, T r ) => l.ScalarDivide( r );
	public static Vector2<T> operator *( in Vector2<T> l, in Vector2<T> r ) => l.MultiplyEntrywise( r );
	public static Vector2<T> operator /( in Vector2<T> l, in Vector2<T> r ) => l.DivideEntrywise( r );
	public static Vector2<T> operator *( in Vector2<T> l, in Bivector2<T> r ) => l.Multiply( r );

	public static implicit operator Vector2<T>( T v ) => new( v, v );
	public static implicit operator Vector2<T>( (T x, T y) tuple ) => new( tuple.x, tuple.y );

	public static bool operator ==( in Vector2<T> l, in Vector2<T> r ) => l.X == r.X && l.Y == r.Y;
	public static bool operator !=( in Vector2<T> l, in Vector2<T> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2<T> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y]";
}
