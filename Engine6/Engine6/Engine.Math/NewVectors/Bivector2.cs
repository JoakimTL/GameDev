using Engine.Math.NewVectors.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector2<TScalar>( TScalar xy ) :
		IVector<Bivector2<TScalar>, TScalar>,
		IMultivectorPart<Multivector2<TScalar>, Bivector2<TScalar>>,
		ILinearAlgebraOperators<Bivector2<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar XY = xy;

	public static Bivector2<TScalar> AdditiveIdentity => Zero;
	public static Bivector2<TScalar> MultiplicativeIdentity => One;
	public static Bivector2<TScalar> Zero { get; } = new( TScalar.Zero );
	public static Bivector2<TScalar> One { get; } = new( TScalar.One );

	public static Multivector2<TScalar> GetMultivector( in Bivector2<TScalar> part ) => new( TScalar.Zero, Vector2<TScalar>.Zero, part );

	public static Bivector2<TScalar> Negate( in Bivector2<TScalar> l ) => new( -l.XY );
	public static Bivector2<TScalar> Add( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => new( l.XY + r.XY );
	public static Bivector2<TScalar> Subtract( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => new( l.XY - r.XY );
	public static Bivector2<TScalar> ScalarMultiply( in Bivector2<TScalar> l, TScalar r ) => new( l.XY * r );
	public static Bivector2<TScalar> ScalarDivide( in Bivector2<TScalar> l, TScalar r ) => new( l.XY / r );
	public static Bivector2<TScalar> DivideScalar( TScalar l, in Bivector2<TScalar> r ) => new( l / r.XY );
	public static TScalar Dot( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => -l.XY * r.XY;

	public static Bivector2<TScalar> operator -( in Bivector2<TScalar> l ) => Negate( l );
	public static Bivector2<TScalar> operator +( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => Add( l, r );
	public static Bivector2<TScalar> operator -( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => Subtract( l, r );
	public static Bivector2<TScalar> operator *( in Bivector2<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Bivector2<TScalar> operator *( TScalar l, in Bivector2<TScalar> r ) => ScalarMultiply( r, l );
	public static Bivector2<TScalar> operator /( in Bivector2<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Bivector2<TScalar> operator /( TScalar l, in Bivector2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => l.XY == r.XY;
	public static bool operator !=( in Bivector2<TScalar> l, in Bivector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( XY );
	public override string ToString() => $"[{XY:N3}XY]";

	public static implicit operator Bivector2<TScalar>( TScalar value ) => new( value );
}

