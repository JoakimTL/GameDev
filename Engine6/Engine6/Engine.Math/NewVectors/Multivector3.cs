using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector3<TScalar>( TScalar scalar, Vector3<TScalar> vector, Bivector3<TScalar> bivector, Trivector3<TScalar> trivector ) :
		IVector<Multivector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Multivector3<TScalar>>,
		ILinearAlgebraOperators<Multivector3<TScalar>, TScalar>,
		IGeometricProduct<Multivector3<TScalar>, Vector3<TScalar>, Multivector3<TScalar>>,
		IGeometricProduct<Multivector3<TScalar>, Bivector3<TScalar>, Multivector3<TScalar>>,
		IGeometricProduct<Multivector3<TScalar>, Trivector3<TScalar>, Multivector3<TScalar>>,
		IGeometricProduct<Multivector3<TScalar>, Rotor3<TScalar>, Multivector3<TScalar>>,
		IGeometricProduct<Multivector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>,
		IContainsMultivectorPart<Multivector3<TScalar>, TScalar>,
		IContainsMultivectorPart<Multivector3<TScalar>, Vector3<TScalar>>,
		IContainsMultivectorPart<Multivector3<TScalar>, Bivector3<TScalar>>,
		IContainsMultivectorPart<Multivector3<TScalar>, Trivector3<TScalar>>,
		IContainsMultivectorPart<Multivector3<TScalar>, Rotor3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Vector3<TScalar> Vector = vector;
	public readonly Bivector3<TScalar> Bivector = bivector;
	public readonly Trivector3<TScalar> Trivector = trivector;

	public Multivector3( TScalar scalar, TScalar x, TScalar y, TScalar z, TScalar yz, TScalar zx, TScalar xy, TScalar xyz )
		: this( scalar, new( x, y, z ), new( yz, zx, xy ), new( xyz ) ) { }

	public static Multivector3<TScalar> AdditiveIdentity => Zero;
	public static Multivector3<TScalar> MultiplicativeIdentity => One;
	public static Multivector3<TScalar> Zero { get; } = new( TScalar.Zero, Vector3<TScalar>.Zero, Bivector3<TScalar>.Zero, Trivector3<TScalar>.Zero );
	public static Multivector3<TScalar> One { get; } = new( TScalar.One, Vector3<TScalar>.One, Bivector3<TScalar>.One, Trivector3<TScalar>.One );

	public Multivector3<TScalar> GetMultivector() => this;

	public Multivector3<TScalar> Negate() => new( -Scalar, -Vector, -Bivector, -Trivector );
	public Multivector3<TScalar> Add( in Multivector3<TScalar> r ) => new( Scalar + r.Scalar, Vector + r.Vector, Bivector + r.Bivector, Trivector + r.Trivector );
	public Multivector3<TScalar> Subtract( in Multivector3<TScalar> r ) => new( Scalar - r.Scalar, Vector - r.Vector, Bivector - r.Bivector, Trivector - r.Trivector );
	public Multivector3<TScalar> ScalarMultiply( TScalar r ) => new( Scalar * r, Vector * r, Bivector * r, Trivector * r );
	public Multivector3<TScalar> ScalarDivide( TScalar r ) => new( Scalar / r, Vector / r, Bivector / r, Trivector / r );
	public static Multivector3<TScalar> DivideScalar( TScalar l, in Multivector3<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector, l / r.Trivector );
	public TScalar Dot( in Multivector3<TScalar> r ) => (Scalar * r.Scalar) + Vector.Dot( r.Vector ) + Bivector.Dot( r.Bivector ) + Trivector.Dot( r.Trivector );

	public Multivector3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );

	public static Multivector3<TScalar> operator -( in Multivector3<TScalar> l ) => l.Negate();
	public static Multivector3<TScalar> operator +( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Add( r );
	public static Multivector3<TScalar> operator -( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Subtract( r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Multivector3<TScalar> operator *( TScalar l, in Multivector3<TScalar> r ) => r.ScalarMultiply( l );
	public static Multivector3<TScalar> operator /( in Multivector3<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Multivector3<TScalar> operator /( TScalar l, in Multivector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector;
	public static bool operator !=( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Multivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Vector, Bivector, Trivector );
	public override string ToString() => $"[{Scalar:N3}+{Vector}+{Bivector}+{Trivector}]";

	public static explicit operator TScalar( in Multivector3<TScalar> part ) => part.Scalar;
	public static explicit operator Vector3<TScalar>( in Multivector3<TScalar> part ) => part.Vector;
	public static explicit operator Bivector3<TScalar>( in Multivector3<TScalar> part ) => part.Bivector;
	public static explicit operator Trivector3<TScalar>( in Multivector3<TScalar> part ) => part.Trivector;
	public static explicit operator Rotor3<TScalar>( in Multivector3<TScalar> part ) => new( part.Scalar, part.Bivector );
}
