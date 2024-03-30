using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.Interfaces;
using Engine.Math.Operations;

namespace Engine.Math;
public readonly struct Rotor3<T>( T scalar, in Bivector3<T> bivector ) :
		ILinearOperators<Rotor3<T>, T>,
		IVectorEntrywiseOperators<Rotor3<T>, T>,
		IMultivectorPart<Multivector3<T>>,
		IProductOperator<Rotor3<T>, Vector3<T>, Multivector3<T>>,
		IProductOperator<Rotor3<T>, Multivector3<T>, Multivector3<T>>,
		IAdditiveIdentity<Rotor3<T>, Rotor3<T>>,
		IMultiplicativeIdentity<Rotor3<T>, Rotor3<T>>
	where T :
		unmanaged, INumber<T> {

	public readonly T Scalar = scalar;
	public readonly Bivector3<T> Bivector = bivector;

	public Multivector3<T> GetMultivector() => new( Scalar, Vector3<T>.Zero, Bivector, T.Zero );

	public static Rotor3<T> AdditiveIdentity => new( T.Zero, Bivector3<T>.Zero );
	public static Rotor3<T> MultiplicativeIdentity => new( T.Zero, Bivector3<T>.One );

	public static Rotor3<T> operator -( in Rotor3<T> l ) => l.Negate();
	public static Rotor3<T> operator +( in Rotor3<T> l, in Rotor3<T> r ) => l.Add( r );
	public static Rotor3<T> operator -( in Rotor3<T> l, in Rotor3<T> r ) => l.Subtract( r );
	public static Rotor3<T> operator *( in Rotor3<T> l, T r ) => l.ScalarMultiply( r );
	public static Rotor3<T> operator *( T l, in Rotor3<T> r ) => r.ScalarMultiply( l );
	public static Rotor3<T> operator /( in Rotor3<T> l, T r ) => l.ScalarDivide( r );
	public static Rotor3<T> operator *( in Rotor3<T> l, in Rotor3<T> r ) => l.MultiplyEntrywise( r );
	public static Rotor3<T> operator /( in Rotor3<T> l, in Rotor3<T> r ) => l.DivideEntrywise( r );
	public static Multivector3<T> operator *( in Rotor3<T> l, in Vector3<T> r ) => l.Multiply( r );
	public static Multivector3<T> operator *( in Rotor3<T> l, in Multivector3<T> r ) => l.Multiply( r );

	public static bool operator ==( in Rotor3<T> l, in Rotor3<T> r ) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
	public static bool operator !=( in Rotor3<T> l, in Rotor3<T> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotor3<T> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Bivector );
	public override string ToString() => $"[{Scalar:N3} + {Bivector}]";
}
