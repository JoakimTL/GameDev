using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector2<TScalar>( TScalar scalar, Vector2<TScalar> vector, Bivector2<TScalar> bivector ) :
		IVector<Multivector2<TScalar>, TScalar>,
		IPartOfMultivector<Multivector2<TScalar>, Multivector2<TScalar>>,
		ILinearAlgebraVectorOperators<Multivector2<TScalar>>,
		ILinearAlgebraScalarOperators<Multivector2<TScalar>, TScalar>,
		IProduct<Multivector2<TScalar>, Vector2<TScalar>, Multivector2<TScalar>>,
		IProduct<Multivector2<TScalar>, Bivector2<TScalar>, Multivector2<TScalar>>,
		IProduct<Multivector2<TScalar>, Rotor2<TScalar>, Multivector2<TScalar>>,
		IProduct<Multivector2<TScalar>, Multivector2<TScalar>, Multivector2<TScalar>>,
		IExplicitCast<Multivector2<TScalar>, TScalar>,
		IExplicitCast<Multivector2<TScalar>, Vector2<TScalar>>,
		IExplicitCast<Multivector2<TScalar>, Bivector2<TScalar>>,
		IExplicitCast<Multivector2<TScalar>, Rotor2<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Vector2<TScalar> Vector = vector;
	public readonly Bivector2<TScalar> Bivector = bivector;

	public Multivector2( TScalar scalar, TScalar x, TScalar y, TScalar xy )
		: this( scalar, new( x, y ), new( xy ) ) { }

	public static Multivector2<TScalar> AdditiveIdentity => Zero;
	public static Multivector2<TScalar> MultiplicativeIdentity => One;
	public static Multivector2<TScalar> Zero { get; } = new( TScalar.Zero, Vector2<TScalar>.Zero, Bivector2<TScalar>.Zero );
	public static Multivector2<TScalar> One { get; } = new( TScalar.One, Vector2<TScalar>.One, Bivector2<TScalar>.One );
	public static Multivector2<TScalar> Two { get; } = One + One;

	public Multivector2<TScalar> GetMultivector() => this;

	public Multivector2<TScalar> Negate() => new( -Scalar, -Vector, -Bivector );
	public Multivector2<TScalar> Add( in Multivector2<TScalar> r ) => new( Scalar + r.Scalar, Vector + r.Vector, Bivector + r.Bivector );
	public Multivector2<TScalar> Subtract( in Multivector2<TScalar> r ) => new( Scalar - r.Scalar, Vector - r.Vector, Bivector - r.Bivector );
	public Multivector2<TScalar> ScalarMultiply( TScalar r ) => new( Scalar * r, Vector * r, Bivector * r );
	public Multivector2<TScalar> ScalarDivide( TScalar r ) => new( Scalar / r, Vector / r, Bivector / r );
	public static Multivector2<TScalar> DivideScalar( TScalar l, in Multivector2<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector );
	public TScalar Dot( in Multivector2<TScalar> r ) => (Scalar * r.Scalar) + Vector.Dot( r.Vector ) + Bivector.Dot( r.Bivector );
	public TScalar MagnitudeSquared() => (Scalar * Scalar) + Vector.MagnitudeSquared() + Bivector.MagnitudeSquared();

	public Multivector2<TScalar> Multiply( in Vector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Multivector2<TScalar> Multiply( in Bivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Multivector2<TScalar> Multiply( in Rotor2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Multivector2<TScalar> Multiply( in Multivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, in Vector2<TScalar> r ) => l.Multiply( r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, in Bivector2<TScalar> r ) => l.Multiply( r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, in Rotor2<TScalar> r ) => l.Multiply( r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => l.Multiply( r );

	public static Multivector2<TScalar> operator -( in Multivector2<TScalar> l ) => l.Negate();
	public static Multivector2<TScalar> operator +( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => l.Add( r );
	public static Multivector2<TScalar> operator -( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => l.Subtract( r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Multivector2<TScalar> operator *( TScalar l, in Multivector2<TScalar> r ) => r.ScalarMultiply( l );
	public static Multivector2<TScalar> operator /( in Multivector2<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Multivector2<TScalar> operator /( TScalar l, in Multivector2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector;
	public static bool operator !=( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Multivector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Vector, Bivector );
	public override string ToString()
		=> $"<{Scalar.ToFormattedString( true )} + {Vector} + {Bivector}>";

	public static explicit operator TScalar( in Multivector2<TScalar> part ) => part.Scalar;
	public static explicit operator Vector2<TScalar>( in Multivector2<TScalar> part ) => part.Vector;
	public static explicit operator Bivector2<TScalar>( in Multivector2<TScalar> part ) => part.Bivector;
	public static explicit operator Rotor2<TScalar>( in Multivector2<TScalar> part ) => new( part.Scalar, part.Bivector );

}
