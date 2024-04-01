using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Quadvector4<TScalar>( TScalar xyzw ) :
		IVector<Quadvector4<TScalar>, TScalar>,
		IMultivectorPart<Multivector4<TScalar>, Quadvector4<TScalar>>,
		ILinearAlgebraOperators<Quadvector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar XYZW = xyzw;

	public static Quadvector4<TScalar> AdditiveIdentity => Zero;
	public static Quadvector4<TScalar> MultiplicativeIdentity => One;
	public static Quadvector4<TScalar> Zero { get; } = new( TScalar.Zero );
	public static Quadvector4<TScalar> One { get; } = new( TScalar.One );

	public static Multivector4<TScalar> GetMultivector( in Quadvector4<TScalar> part ) => new( TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, part );

	public static Quadvector4<TScalar> Negate( in Quadvector4<TScalar> l ) => new( -l.XYZW );
	public static Quadvector4<TScalar> Add( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => new( l.XYZW + r.XYZW );
	public static Quadvector4<TScalar> Subtract( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => new( l.XYZW - r.XYZW );
	public static Quadvector4<TScalar> ScalarMultiply( in Quadvector4<TScalar> l, TScalar r ) => new( l.XYZW * r );
	public static Quadvector4<TScalar> ScalarDivide( in Quadvector4<TScalar> l, TScalar r ) => new( l.XYZW / r );
	public static Quadvector4<TScalar> DivideScalar( TScalar l, in Quadvector4<TScalar> r ) => new( l / r.XYZW );
	public static TScalar Dot( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => l.XYZW * r.XYZW;

	public static Quadvector4<TScalar> operator -( in Quadvector4<TScalar> l ) => Negate( l );
	public static Quadvector4<TScalar> operator +( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => Add( l, r );
	public static Quadvector4<TScalar> operator -( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => Subtract( l, r );
	public static Quadvector4<TScalar> operator *( in Quadvector4<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Quadvector4<TScalar> operator *( TScalar l, in Quadvector4<TScalar> r ) => ScalarMultiply( r, l );
	public static Quadvector4<TScalar> operator /( in Quadvector4<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Quadvector4<TScalar> operator /( TScalar l, in Quadvector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => l.XYZW == r.XYZW;
	public static bool operator !=( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Quadvector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( XYZW );
	public override string ToString() => $"[{XYZW:N3}XYZW]";
}
