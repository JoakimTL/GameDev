using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Rotor2<TScalar>( TScalar scalar, Bivector2<TScalar> bivector ) :
		IVector<Rotor2<TScalar>, TScalar>,
		IMultivectorPart<Multivector2<TScalar>, Rotor2<TScalar>>,
		ILinearAlgebraOperators<Rotor2<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Bivector2<TScalar> Bivector = bivector;

	public static Rotor2<TScalar> AdditiveIdentity => Zero;
	public static Rotor2<TScalar> MultiplicativeIdentity => One;
	public static Rotor2<TScalar> Zero { get; } = new( TScalar.Zero, Bivector2<TScalar>.Zero );
	public static Rotor2<TScalar> One { get; } = new( TScalar.One, Bivector2<TScalar>.One );

	public static Multivector2<TScalar> GetMultivector( in Rotor2<TScalar> part ) => new( part.Scalar, Vector2<TScalar>.Zero, part.Bivector );

	public static Rotor2<TScalar> Negate( in Rotor2<TScalar> l ) => new( -l.Scalar, -l.Bivector );
	public static Rotor2<TScalar> Add( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => new( l.Scalar + r.Scalar, l.Bivector + r.Bivector );
	public static Rotor2<TScalar> Subtract( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => new( l.Scalar - r.Scalar, l.Bivector - r.Bivector );
	public static Rotor2<TScalar> ScalarMultiply( in Rotor2<TScalar> l, TScalar r ) => new( l.Scalar * r, l.Bivector * r );
	public static Rotor2<TScalar> ScalarDivide( in Rotor2<TScalar> l, TScalar r ) => new( l.Scalar / r, l.Bivector / r );
	public static Rotor2<TScalar> DivideScalar( TScalar l, in Rotor2<TScalar> r ) => new( l / r.Scalar, l / r.Bivector );
	public static TScalar Dot( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => (l.Scalar * r.Scalar) + Bivector2<TScalar>.Dot( l.Bivector, r.Bivector );

	public static Rotor2<TScalar> operator -( in Rotor2<TScalar> l ) => Negate( l );
	public static Rotor2<TScalar> operator +( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => Add( l, r );
	public static Rotor2<TScalar> operator -( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => Subtract( l, r );
	public static Rotor2<TScalar> operator *( in Rotor2<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Rotor2<TScalar> operator *( TScalar l, in Rotor2<TScalar> r ) => ScalarMultiply( r, l );
	public static Rotor2<TScalar> operator /( in Rotor2<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Rotor2<TScalar> operator /( TScalar l, in Rotor2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
	public static bool operator !=( in Rotor2<TScalar> l, in Rotor2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotor2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Bivector );
	public override string ToString() => $"[{Scalar:N3}+{Bivector}]";
}
