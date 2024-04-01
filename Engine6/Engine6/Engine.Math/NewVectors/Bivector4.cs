using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector4<TScalar>( TScalar yz, TScalar zx, TScalar xy, TScalar yw, TScalar zw, TScalar xw ) :
		IVector<Bivector4<TScalar>, TScalar>,
		IMultivectorPart<Multivector4<TScalar>, Bivector4<TScalar>>,
		ILinearAlgebraOperators<Bivector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar YZ = yz;
	public readonly TScalar ZX = zx;
	public readonly TScalar XY = xy;
	public readonly TScalar YW = yw;
	public readonly TScalar ZW = zw;
	public readonly TScalar XW = xw;

	public static Bivector4<TScalar> AdditiveIdentity => Zero;
	public static Bivector4<TScalar> MultiplicativeIdentity => One;
	public static Bivector4<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Bivector4<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One );

	public static Multivector4<TScalar> GetMultivector( in Bivector4<TScalar> part ) => new( TScalar.Zero, Vector4<TScalar>.Zero, part, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );

	public static Bivector4<TScalar> Negate( in Bivector4<TScalar> l ) => new( -l.YZ, -l.ZX, -l.XY, -l.YW, -l.ZW, -l.XW );
	public static Bivector4<TScalar> Add( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => new( l.YZ + r.YZ, l.ZX + r.ZX, l.XY + r.XY, l.YW + r.YW, l.ZW + r.ZW, l.XW + r.XW );
	public static Bivector4<TScalar> Subtract( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => new( l.YZ - r.YZ, l.ZX - r.ZX, l.XY - r.XY, l.YW - r.YW, l.ZW - r.ZW, l.XW - r.XW );
	public static Bivector4<TScalar> ScalarMultiply( in Bivector4<TScalar> l, TScalar r ) => new( l.YZ * r, l.ZX * r, l.XY * r, l.YW * r, l.ZW * r, l.XW * r );
	public static Bivector4<TScalar> ScalarDivide( in Bivector4<TScalar> l, TScalar r ) => new( l.YZ / r, l.ZX / r, l.XY / r, l.YW / r, l.ZW / r, l.XW / r );
	public static Bivector4<TScalar> DivideScalar( TScalar l, in Bivector4<TScalar> r ) => new( l / r.YZ, l / r.ZX, l / r.XY, l / r.YW, l / r.ZW, l / r.XW );
	public static TScalar Dot( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => (-l.YZ * r.YZ) - (l.ZX * r.ZX) - (l.XY * r.XY) - (l.YW * r.YW) - (l.ZW * r.ZW) - (l.XW * r.XW);

	public static Bivector4<TScalar> operator -( in Bivector4<TScalar> l ) => Negate( l );
	public static Bivector4<TScalar> operator +( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => Add( l, r );
	public static Bivector4<TScalar> operator -( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => Subtract( l, r );
	public static Bivector4<TScalar> operator *( in Bivector4<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Bivector4<TScalar> operator *( TScalar l, in Bivector4<TScalar> r ) => ScalarMultiply( r, l );
	public static Bivector4<TScalar> operator /( in Bivector4<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Bivector4<TScalar> operator /( TScalar l, in Bivector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY && l.YW == r.YW && l.ZW == r.ZW && l.XW == r.XW;
	public static bool operator !=( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZ, ZX, XY, YW, ZW, XW );
	public override string ToString() => $"[{YZ:N3}YZ, {ZX:N3}ZX, {XY:N3}XY, {YW:N3}YW, {ZW:N3}ZW, {XW:N3}XW]";
}
