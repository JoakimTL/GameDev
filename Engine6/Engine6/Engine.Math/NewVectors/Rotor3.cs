using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Rotor3<TScalar>( TScalar scalar, Bivector3<TScalar> bivector ) :
		IVector<Rotor3<TScalar>, TScalar>,
		IMultivectorPart<Multivector3<TScalar>, Rotor3<TScalar>>,
		ILinearAlgebraOperators<Rotor3<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Bivector3<TScalar> Bivector = bivector;

	public Rotor3( TScalar scalar, TScalar yz, TScalar zx, TScalar xy ) : this( scalar, new( yz, zx, xy ) ) { }

	public static Rotor3<TScalar> AdditiveIdentity => Zero;
	public static Rotor3<TScalar> MultiplicativeIdentity => One;
	public static Rotor3<TScalar> Zero { get; } = new( TScalar.Zero, Bivector3<TScalar>.Zero );
	public static Rotor3<TScalar> One { get; } = new( TScalar.One, Bivector3<TScalar>.One );

	public static Multivector3<TScalar> GetMultivector( in Rotor3<TScalar> part ) => new( part.Scalar, Vector3<TScalar>.Zero, part.Bivector, Trivector3<TScalar>.Zero );

	public static Rotor3<TScalar> Negate( in Rotor3<TScalar> l ) => new( -l.Scalar, -l.Bivector );
	public static Rotor3<TScalar> Add( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => new( l.Scalar + r.Scalar, l.Bivector + r.Bivector );
	public static Rotor3<TScalar> Subtract( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => new( l.Scalar - r.Scalar, l.Bivector - r.Bivector );
	public static Rotor3<TScalar> ScalarMultiply( in Rotor3<TScalar> l, TScalar r ) => new( l.Scalar * r, l.Bivector * r );
	public static Rotor3<TScalar> ScalarDivide( in Rotor3<TScalar> l, TScalar r ) => new( l.Scalar / r, l.Bivector / r );
	public static Rotor3<TScalar> DivideScalar( TScalar l, in Rotor3<TScalar> r ) => new( l / r.Scalar, l / r.Bivector );
	public static TScalar Dot( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => (l.Scalar * r.Scalar) + Bivector3<TScalar>.Dot( l.Bivector, r.Bivector );

	public static Rotor3<TScalar> operator -( in Rotor3<TScalar> l ) => Negate( l );
	public static Rotor3<TScalar> operator +( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => Add( l, r );
	public static Rotor3<TScalar> operator -( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => Subtract( l, r );
	public static Rotor3<TScalar> operator *( in Rotor3<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Rotor3<TScalar> operator *( TScalar l, in Rotor3<TScalar> r ) => ScalarMultiply( r, l );
	public static Rotor3<TScalar> operator /( in Rotor3<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Rotor3<TScalar> operator /( TScalar l, in Rotor3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
	public static bool operator !=( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotor3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Bivector );
	public override string ToString() => $"[{Scalar:N3}+{Bivector}]";
}
