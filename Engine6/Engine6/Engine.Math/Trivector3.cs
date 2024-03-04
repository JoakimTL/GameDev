using Engine.Math.Math;
using Engine.Math.Old;
using System.Numerics;

namespace Engine.Math;

public readonly struct Trivector3<T>( T xyz ) :
		ILinearOperators<Trivector3<T>, T>,
		IGeometricProductOperator<Trivector3<T>, Vector3<T>, Bivector3<T>>,
		IAdditiveIdentity<Trivector3<T>, Trivector3<T>>,
		IMultiplicativeIdentity<Trivector3<T>, Trivector3<T>>
	where T :
		unmanaged,
		INumberBase<T> {

	public readonly T XYZ = xyz;

	public Multivector3<T> GetMultivector() => new( T.AdditiveIdentity, Vector3<T>.AdditiveIdentity, Bivector3<T>.AdditiveIdentity, this );

	public static Trivector3<T> AdditiveIdentity => Zero;
	public static Trivector3<T> MultiplicativeIdentity => One;

	public static readonly Trivector3<T> Zero = new( T.Zero );
	public static readonly Trivector3<T> One = new( T.One );

	public static Trivector3<T> operator -( in Trivector3<T> l ) => l.Negate();
	public static Trivector3<T> operator +( in Trivector3<T> l, in Trivector3<T> r ) => l.Add( r );
	public static Trivector3<T> operator -( in Trivector3<T> l, in Trivector3<T> r ) => l.Subtract( r );
	public static Trivector3<T> operator *( in Trivector3<T> l, T r ) => l.ScalarMultiply( r );
	public static Trivector3<T> operator *( T l, in Trivector3<T> r ) => r.ScalarMultiply( l );
	public static Trivector3<T> operator /( in Trivector3<T> l, T r ) => l.ScalarDivide( r );
	public static Trivector3<T> operator *( in Trivector3<T> l, in Trivector3<T> r ) => l.ScalarMultiply( r.XYZ );
	public static Trivector3<T> operator /( in Trivector3<T> l, in Trivector3<T> r ) => l.ScalarDivide( r.XYZ );
	public static Bivector3<T> operator *( in Trivector3<T> l, in Vector3<T> r ) => l.Multiply( r );


	public static implicit operator Trivector3<T>( T xyz ) => new( xyz );


	public override string ToString() => $"[{XYZ:N3}XYZ]";
}
