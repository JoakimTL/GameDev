using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Rotor3<TScalar>( TScalar scalar, Bivector3<TScalar> bivector ) :
		IVector<Rotor3<TScalar>, TScalar>,
		IRotor<Rotor3<TScalar>, Vector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Rotor3<TScalar>>,
		ILinearAlgebraVectorOperators<Rotor3<TScalar>>,
		ILinearAlgebraScalarOperators<Rotor3<TScalar>, TScalar>,
		IProduct<Rotor3<TScalar>, Vector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Rotor3<TScalar>, Bivector3<TScalar>, Rotor3<TScalar>>,
		IProduct<Rotor3<TScalar>, Trivector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Rotor3<TScalar>, Rotor3<TScalar>, Rotor3<TScalar>>,
		IProduct<Rotor3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>,
		IExplicitCast<Rotor3<TScalar>, TScalar>,
		IExplicitCast<Rotor3<TScalar>, Bivector3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Bivector3<TScalar> Bivector = bivector;

	public Rotor3( TScalar scalar, TScalar yz, TScalar zx, TScalar xy ) : this( scalar, new( yz, zx, xy ) ) { }

	public static Rotor3<TScalar> AdditiveIdentity => Zero;
	public static Rotor3<TScalar> MultiplicativeIdentity => One;
	public static Rotor3<TScalar> Zero { get; } = new( TScalar.Zero, Bivector3<TScalar>.Zero );
	public static Rotor3<TScalar> One { get; } = new( TScalar.One, Bivector3<TScalar>.One );
	public static Rotor3<TScalar> Two { get; } = One + One;

	public Multivector3<TScalar> GetMultivector() => new( Scalar, Vector3<TScalar>.Zero, Bivector, Trivector3<TScalar>.Zero );

	public Rotor3<TScalar> Negate() => new( -Scalar, -Bivector );
	public Rotor3<TScalar> Add( in Rotor3<TScalar> r ) => new( Scalar + r.Scalar, Bivector + r.Bivector );
	public Rotor3<TScalar> Subtract( in Rotor3<TScalar> r ) => new( Scalar - r.Scalar, Bivector - r.Bivector );
	public Rotor3<TScalar> ScalarMultiply( TScalar r ) => new( Scalar * r, Bivector * r );
	public Rotor3<TScalar> ScalarDivide( TScalar r ) => new( Scalar / r, Bivector / r );
	public static Rotor3<TScalar> DivideScalar( TScalar l, in Rotor3<TScalar> r ) => new( l / r.Scalar, l / r.Bivector );
	public TScalar Dot( in Rotor3<TScalar> r ) => (Scalar * r.Scalar) + Bivector.Dot( r.Bivector );

	public Rotor3<TScalar> Conjugate() => new( Scalar, -Bivector );
	public Vector3<TScalar> Rotate( in Vector3<TScalar> v ) => this.Multiply( v ).Multiply( Conjugate() ).Vector;

	public Multivector3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public static Multivector3<TScalar> operator *( in Rotor3<TScalar> l, in Vector3<TScalar> r ) => l.Multiply( r );
	public static Rotor3<TScalar> operator *( in Rotor3<TScalar> l, in Bivector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Rotor3<TScalar> l, in Trivector3<TScalar> r ) => l.Multiply( r );
	public static Rotor3<TScalar> operator *( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Rotor3<TScalar> l, in Multivector3<TScalar> r ) => l.Multiply( r );

	public static Rotor3<TScalar> operator -( in Rotor3<TScalar> l ) => l.Negate();
	public static Rotor3<TScalar> operator +( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => l.Add( r );
	public static Rotor3<TScalar> operator -( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => l.Subtract( r );
	public static Rotor3<TScalar> operator *( in Rotor3<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Rotor3<TScalar> operator *( TScalar l, in Rotor3<TScalar> r ) => r.ScalarMultiply( l );
	public static Rotor3<TScalar> operator /( in Rotor3<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Rotor3<TScalar> operator /( TScalar l, in Rotor3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
	public static bool operator !=( in Rotor3<TScalar> l, in Rotor3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotor3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Bivector );
	public override string ToString() => string.Create( CultureInfo.InvariantCulture, $"<{Scalar:0.###} + {Bivector}>");

	public static explicit operator TScalar( in Rotor3<TScalar> part ) => part.Scalar;
	public static explicit operator Bivector3<TScalar>( in Rotor3<TScalar> part ) => part.Bivector;
}