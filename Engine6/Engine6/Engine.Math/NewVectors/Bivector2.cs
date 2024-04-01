using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector2<TScalar>( TScalar xy ) :
		IVector<Bivector2<TScalar>, TScalar>,
		IPartOfMultivector<Multivector2<TScalar>, Bivector2<TScalar>>,
		ILinearAlgebraOperators<Bivector2<TScalar>, TScalar>,
		IGeometricProduct<Bivector2<TScalar>, Vector2<TScalar>, Vector2<TScalar>>,
		IGeometricProduct<Bivector2<TScalar>, Bivector2<TScalar>, TScalar>,
		IGeometricProduct<Bivector2<TScalar>, Rotor2<TScalar>, Rotor2<TScalar>>,
		IGeometricProduct<Bivector2<TScalar>, Multivector2<TScalar>, Multivector2<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar XY = xy;

	public static Bivector2<TScalar> AdditiveIdentity => Zero;
	public static Bivector2<TScalar> MultiplicativeIdentity => One;
	public static Bivector2<TScalar> Zero { get; } = new( TScalar.Zero );
	public static Bivector2<TScalar> One { get; } = new( TScalar.One );

	public Multivector2<TScalar> GetMultivector() => new( TScalar.Zero, Vector2<TScalar>.Zero, this );

	public Bivector2<TScalar> Negate() => new( -XY );
	public Bivector2<TScalar> Add( in Bivector2<TScalar> r ) => new( XY + r.XY );
	public Bivector2<TScalar> Subtract( in Bivector2<TScalar> r ) => new( XY - r.XY );
	public Bivector2<TScalar> ScalarMultiply( TScalar r ) => new( XY * r );
	public Bivector2<TScalar> ScalarDivide( TScalar r ) => new( XY / r );
	public static Bivector2<TScalar> DivideScalar( TScalar l, in Bivector2<TScalar> r ) => new( l / r.XY );
	public TScalar Dot( in Bivector2<TScalar> r ) => -XY * r.XY;

	public Vector2<TScalar> Multiply( in Vector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public TScalar Multiply( in Bivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Rotor2<TScalar> Multiply( in Rotor2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Multivector2<TScalar> Multiply( in Multivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );

	public static Bivector2<TScalar> operator -( in Bivector2<TScalar> l ) => l.Negate();
	public static Bivector2<TScalar> operator +( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => l.Add( r );
	public static Bivector2<TScalar> operator -( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => l.Subtract( r );
	public static Bivector2<TScalar> operator *( in Bivector2<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Bivector2<TScalar> operator *( TScalar l, in Bivector2<TScalar> r ) => r.ScalarMultiply( l );
	public static Bivector2<TScalar> operator /( in Bivector2<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Bivector2<TScalar> operator /( TScalar l, in Bivector2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => l.XY == r.XY;
	public static bool operator !=( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( XY );
	public override string ToString() => $"[{XY:N3}XY]";

	public static implicit operator Bivector2<TScalar>( TScalar value ) => new( value );
}

