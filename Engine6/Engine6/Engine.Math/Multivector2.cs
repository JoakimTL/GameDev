using System.Numerics;

namespace Engine.Math;

public readonly struct Multivector2<T>( T scalar, in Vector2<T> vector, in Bivector2<T> bivector ) :
		ILinearOperators<Multivector2<T>, T>,
		IGeometricProductOperator<Multivector2<T>, Vector2<T>, Multivector2<T>>,
		IGeometricProductOperator<Multivector2<T>, Bivector2<T>, Multivector2<T>>,
		IGeometricProductOperator<Multivector2<T>, Multivector2<T>, Multivector2<T>>,
		IAdditiveIdentity<Multivector2<T>, Multivector2<T>>,
		IMultiplicativeIdentity<Multivector2<T>, Multivector2<T>>
	where T :
		unmanaged,
		INumberBase<T> {

	public readonly T Scalar = scalar;
	public readonly Vector2<T> Vector = vector;
	public readonly Bivector2<T> Bivector = bivector;

	public Multivector2( T scalar, T vectorX, T vectorY, T bivectorXY ) : this( scalar, new( vectorX, vectorY ), new( bivectorXY ) ) { }

	public static Multivector2<T> AdditiveIdentity => Zero;
	public static Multivector2<T> MultiplicativeIdentity => One;

	public static readonly Multivector2<T> Zero = new( T.Zero, Vector2<T>.Zero, T.Zero );
	public static readonly Multivector2<T> One = new( T.One, Vector2<T>.One, T.One );

	public static Multivector2<T> operator -( in Multivector2<T> l ) => l.Negate();
	public static Multivector2<T> operator -( in Multivector2<T> l, in Multivector2<T> r ) => l.Subtract( r );
	public static Multivector2<T> operator +( in Multivector2<T> l, in Multivector2<T> r ) => l.Add( r );
	public static Multivector2<T> operator *( in Multivector2<T> l, T r ) => l.Multiply( r );
	public static Multivector2<T> operator *( T l, in Multivector2<T> r ) => r.Multiply( l );
	public static Multivector2<T> operator /( in Multivector2<T> l, T r ) => l.Divide( r );
	public static Multivector2<T> operator *( in Multivector2<T> l, in Vector2<T> r ) => l.Multiply( r );
	public static Multivector2<T> operator *( in Multivector2<T> l, in Bivector2<T> r ) => l.Multiply( r );
	public static Multivector2<T> operator *( in Multivector2<T> l, in Multivector2<T> r ) => l.Multiply( r );

}
