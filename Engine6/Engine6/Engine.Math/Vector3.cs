using System.Numerics;

namespace Engine.Math;

public readonly struct Vector3<T>( T x, T y, T z ) : 
		ILinearOperators<Vector3<T>, T>,
		IVectorEntrywiseOperators<Vector3<T>, T>,
		IGeometricProductOperator<Vector3<T>, Bivector3<T>, Multivector3<T>>,
		IGeometricProductOperator<Vector3<T>, Trivector3<T>, Bivector3<T>>,
		IGeometricProductOperator<Vector3<T>, Multivector3<T>, Multivector3<T>>,
		IAdditiveIdentity<Vector3<T>, Vector3<T>>,
		IMultiplicativeIdentity<Vector3<T>, Vector3<T>>
	where T : 
		unmanaged, 
		INumberBase<T> {

	public readonly T X = x;
	public readonly T Y = y;
	public readonly T Z = z;

	public Multivector3<T> GetMultivector() => new( T.Zero, this, Bivector3<T>.Zero, T.Zero );

	public static Vector3<T> AdditiveIdentity => Zero;
	public static Vector3<T> MultiplicativeIdentity => One;

	public static readonly Vector3<T> Zero = new( T.Zero, T.Zero, T.Zero );
	public static readonly Vector3<T> One = new( T.One, T.One, T.One );

	public static Vector3<T> operator -( in Vector3<T> l ) => l.Negate();
	public static Vector3<T> operator +( in Vector3<T> l, in Vector3<T> r ) => l.Add( r );
	public static Vector3<T> operator -( in Vector3<T> l, in Vector3<T> r ) => l.Subtract( r );
	public static Vector3<T> operator *( in Vector3<T> l, T r ) => l.ScalarMultiply( r );
	public static Vector3<T> operator *( T l, in Vector3<T> r ) => r.ScalarMultiply( l );
	public static Vector3<T> operator /( in Vector3<T> l, T r ) => l.ScalarDivide( r );
	public static Vector3<T> operator *( in Vector3<T> l, in Vector3<T> r ) => l.MultiplyEntrywise( r );
	public static Vector3<T> operator /( in Vector3<T> l, in Vector3<T> r ) => l.DivideEntrywise( r );
	public static Multivector3<T> operator *( in Vector3<T> l, in Bivector3<T> r ) => l.Multiply( r );
	public static Bivector3<T> operator *( in Vector3<T> l, in Trivector3<T> r ) => l.Multiply( r );
	public static Multivector3<T> operator *( in Vector3<T> l, in Multivector3<T> r ) => l.Multiply( r );

	public static implicit operator Vector3<T>( (T x, T y, T z) tuple ) => new( tuple.x, tuple.y, tuple.z );

	public override string ToString() => $"[{X:N3}X, {Y:N3}Y, {Z:N3}Z]";
}
