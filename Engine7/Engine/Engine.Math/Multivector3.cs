using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector3<TScalar>( TScalar scalar, Vector3<TScalar> vector, Bivector3<TScalar> bivector, Trivector3<TScalar> trivector ) :
		IVector<Multivector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Multivector3<TScalar>>,
		ILinearAlgebraVectorOperators<Multivector3<TScalar>>,
		ILinearAlgebraScalarOperators<Multivector3<TScalar>, TScalar>,
		IProduct<Multivector3<TScalar>, Vector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Multivector3<TScalar>, Bivector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Multivector3<TScalar>, Trivector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Multivector3<TScalar>, Rotor3<TScalar>, Multivector3<TScalar>>,
		IProduct<Multivector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>,
		IExplicitCast<Multivector3<TScalar>, TScalar>,
		IExplicitCast<Multivector3<TScalar>, Vector3<TScalar>>,
		IExplicitCast<Multivector3<TScalar>, Bivector3<TScalar>>,
		IExplicitCast<Multivector3<TScalar>, Trivector3<TScalar>>,
		IExplicitCast<Multivector3<TScalar>, Rotor3<TScalar>>
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
	public static Multivector3<TScalar> Two { get; } = One + One;

	public Multivector3<TScalar> GetMultivector() => this;

	public Multivector3<TScalar> Negate() => new( -this.Scalar, -this.Vector, -this.Bivector, -this.Trivector );
	public Multivector3<TScalar> Add( in Multivector3<TScalar> r ) => new( this.Scalar + r.Scalar, this.Vector + r.Vector, this.Bivector + r.Bivector, this.Trivector + r.Trivector );
	public Multivector3<TScalar> Subtract( in Multivector3<TScalar> r ) => new( this.Scalar - r.Scalar, this.Vector - r.Vector, this.Bivector - r.Bivector, this.Trivector - r.Trivector );
	public Multivector3<TScalar> ScalarMultiply( TScalar r ) => new( this.Scalar * r, this.Vector * r, this.Bivector * r, this.Trivector * r );
	public Multivector3<TScalar> ScalarDivide( TScalar r ) => new( this.Scalar / r, this.Vector / r, this.Bivector / r, this.Trivector / r );
	public static Multivector3<TScalar> DivideScalar( TScalar l, in Multivector3<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector, l / r.Trivector );
	public TScalar Dot( in Multivector3<TScalar> r ) => this.Scalar * r.Scalar + this.Vector.Dot( r.Vector ) + this.Bivector.Dot( r.Bivector ) + this.Trivector.Dot( r.Trivector );
	public TScalar MagnitudeSquared() => this.Scalar * this.Scalar + this.Vector.MagnitudeSquared() + this.Bivector.MagnitudeSquared() + this.Trivector.MagnitudeSquared();

	public Multivector3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, in Vector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, in Bivector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, in Trivector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, in Rotor3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Multiply( r );

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
	public override int GetHashCode() => HashCode.Combine( this.Scalar, this.Vector, this.Bivector, this.Trivector );
	public override string ToString()
		=> $"<{this.Scalar.ToFormattedString( true )} + {this.Vector} + {this.Bivector} + {this.Trivector}>";

	public static explicit operator TScalar( in Multivector3<TScalar> part ) => part.Scalar;
	public static explicit operator Vector3<TScalar>( in Multivector3<TScalar> part ) => part.Vector;
	public static explicit operator Bivector3<TScalar>( in Multivector3<TScalar> part ) => part.Bivector;
	public static explicit operator Trivector3<TScalar>( in Multivector3<TScalar> part ) => part.Trivector;
	public static explicit operator Rotor3<TScalar>( in Multivector3<TScalar> part ) => new( part.Scalar, part.Bivector );
}
