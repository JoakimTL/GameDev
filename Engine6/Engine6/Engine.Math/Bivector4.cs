using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector4<T>( T yz, T zx, T xy, T yw, T zw, T xw ) :
		IAdditiveIdentity<Bivector4<T>, Bivector4<T>>,
		IMultiplicativeIdentity<Bivector4<T>, Bivector4<T>>
	where T : 
		unmanaged, INumber<T> {

	public readonly T YZ = yz;
	public readonly T ZX = zx;
	public readonly T XY = xy;
	public readonly T YW = yw;
	public readonly T ZW = zw;
	public readonly T XW = xw;

	public static Bivector4<T> AdditiveIdentity => Zero;
	public static Bivector4<T> MultiplicativeIdentity => One;

	public static readonly Bivector4<T> Zero = new( T.Zero, T.Zero, T.Zero, T.Zero, T.Zero, T.Zero );
	public static readonly Bivector4<T> One = new( T.One, T.One, T.One, T.One, T.One, T.One );

	public static bool operator ==( in Bivector4<T> l, in Bivector4<T> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY && l.YW == r.YW && l.ZW == r.ZW && l.XW == r.XW;
	public static bool operator !=( in Bivector4<T> l, in Bivector4<T> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector4<T> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZ, ZX, XY, YW, ZW, XW );
	public override string ToString() => $"[{YZ:N3}YZ, {ZX:N3}ZX, {XY:N3}XY, {YW:N3}YW, {ZW:N3}ZW, {XW:N3}XW]";
}
