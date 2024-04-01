using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Trivector3<TScalar>( TScalar xyz ) :
		IVector<Trivector3<TScalar>, TScalar>,
		IMultivectorPart<Multivector3<TScalar>, Trivector3<TScalar>>,
		ILinearAlgebraOperators<Trivector3<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar XYZ = xyz;

	public static Trivector3<TScalar> AdditiveIdentity => Zero;
	public static Trivector3<TScalar> MultiplicativeIdentity => One;
	public static Trivector3<TScalar> Zero { get; } = new( TScalar.Zero );
	public static Trivector3<TScalar> One { get; } = new( TScalar.One );

	public static Multivector3<TScalar> GetMultivector( in Trivector3<TScalar> part ) => new( TScalar.Zero, Vector3<TScalar>.Zero, Bivector3<TScalar>.Zero, part );

	public static Trivector3<TScalar> Negate( in Trivector3<TScalar> l ) => new( -l.XYZ );
	public static Trivector3<TScalar> Add( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => new( l.XYZ + r.XYZ );
	public static Trivector3<TScalar> Subtract( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => new( l.XYZ - r.XYZ );
	public static Trivector3<TScalar> ScalarMultiply( in Trivector3<TScalar> l, TScalar r ) => new( l.XYZ * r );
	public static Trivector3<TScalar> ScalarDivide( in Trivector3<TScalar> l, TScalar r ) => new( l.XYZ / r );
	public static Trivector3<TScalar> DivideScalar( TScalar l, in Trivector3<TScalar> r ) => new( l / r.XYZ );
	public static TScalar Dot( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => -l.XYZ * r.XYZ;

	public static Trivector3<TScalar> operator -( in Trivector3<TScalar> l ) => Negate( l );
	public static Trivector3<TScalar> operator +( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => Add( l, r );
	public static Trivector3<TScalar> operator -( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => Subtract( l, r );
	public static Trivector3<TScalar> operator *( in Trivector3<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Trivector3<TScalar> operator *( TScalar l, in Trivector3<TScalar> r ) => ScalarMultiply( r, l );
	public static Trivector3<TScalar> operator /( in Trivector3<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Trivector3<TScalar> operator /( TScalar l, in Trivector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => l.XYZ == r.XYZ;
	public static bool operator !=( in Trivector3<TScalar> l, in Trivector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Trivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( XYZ );
	public override string ToString() => $"[{XYZ:N3}XYZ]";
}
