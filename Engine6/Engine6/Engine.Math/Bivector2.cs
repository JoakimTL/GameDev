using Engine.Math.Old;
using System.Numerics;

namespace Engine.Math;

public readonly struct Bivector2<T>( T xy ) :
		ILinearOperators<Bivector2<T>, T>,
		IVectorEntrywiseOperators<Bivector2<T>, T>,
		IGeometricProductOperator<Bivector2<T>, Vector2<T>, Vector2<T>>,
		IAdditiveIdentity<Bivector2<T>, Bivector2<T>>,
		IMultiplicativeIdentity<Bivector2<T>, Bivector2<T>>
	where T :
		unmanaged,
		INumberBase<T> {

	public readonly T XY = xy;

	public Multivector2<T> GetMultivector() => new( T.Zero, Vector2<T>.Zero, this );

	public static Bivector2<T> AdditiveIdentity => Zero;
	public static Bivector2<T> MultiplicativeIdentity => One;

	public static readonly Bivector2<T> Zero = new( T.Zero );
	public static readonly Bivector2<T> One = new( T.One );

	public static Bivector2<T> operator -( in Bivector2<T> l ) => l.Negate();
	public static Bivector2<T> operator +( in Bivector2<T> l, in Bivector2<T> r ) => l.Add( r );
	public static Bivector2<T> operator -( in Bivector2<T> l, in Bivector2<T> r ) => l.Subtract( r );
	public static Bivector2<T> operator *( in Bivector2<T> l, T r ) => l.ScalarMultiply( r );
	public static Bivector2<T> operator *( T l, in Bivector2<T> r ) => r.ScalarMultiply( l );
	public static Bivector2<T> operator /( in Bivector2<T> l, T r ) => l.ScalarDivide( r );
	public static Bivector2<T> operator *( in Bivector2<T> l, in Bivector2<T> r ) => l.ScalarMultiply( r.XY );
	public static Bivector2<T> operator /( in Bivector2<T> l, in Bivector2<T> r ) => l.ScalarDivide( r.XY );
	public static Vector2<T> operator *( in Bivector2<T> l, in Vector2<T> r ) => l.Multiply( r );

	public static implicit operator Bivector2<T>( T xy ) => new( xy );

	public override string ToString() => $"[{XY:N3}XY]";
}
