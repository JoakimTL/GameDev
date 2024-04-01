using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Quadvector4<TScalar>( TScalar xyzw ) :
		IVector<Quadvector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Quadvector4<TScalar>>,
		ILinearAlgebraOperators<Quadvector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar XYZW = xyzw;

	public static Quadvector4<TScalar> AdditiveIdentity => Zero;
	public static Quadvector4<TScalar> MultiplicativeIdentity => One;
	public static Quadvector4<TScalar> Zero { get; } = new( TScalar.Zero );
	public static Quadvector4<TScalar> One { get; } = new( TScalar.One );

	public Multivector4<TScalar> GetMultivector(  ) => new( TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, this );

	public Quadvector4<TScalar> Negate() => new( -XYZW );
	public Quadvector4<TScalar> Add( in Quadvector4<TScalar> r ) => new( XYZW + r.XYZW );
	public Quadvector4<TScalar> Subtract( in Quadvector4<TScalar> r ) => new( XYZW - r.XYZW );
	public Quadvector4<TScalar> ScalarMultiply( TScalar r ) => new( XYZW * r );
	public Quadvector4<TScalar> ScalarDivide( TScalar r ) => new( XYZW / r );
	public static Quadvector4<TScalar> DivideScalar( TScalar l, in Quadvector4<TScalar> r ) => new( l / r.XYZW );
	public TScalar Dot( in Quadvector4<TScalar> r ) => XYZW * r.XYZW;

	public static Quadvector4<TScalar> operator -( in Quadvector4<TScalar> l ) => l.Negate();
	public static Quadvector4<TScalar> operator +( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => l.Add( r );
	public static Quadvector4<TScalar> operator -( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => l.Subtract( r );
	public static Quadvector4<TScalar> operator *( in Quadvector4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Quadvector4<TScalar> operator *( TScalar l, in Quadvector4<TScalar> r ) => r.ScalarMultiply( l );
	public static Quadvector4<TScalar> operator /( in Quadvector4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Quadvector4<TScalar> operator /( TScalar l, in Quadvector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => l.XYZW == r.XYZW;
	public static bool operator !=( in Quadvector4<TScalar> l, in Quadvector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Quadvector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( XYZW );
	public override string ToString() => $"[{XYZW:N3}XYZW]";
}
