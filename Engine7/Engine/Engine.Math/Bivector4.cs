using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector4<TScalar>( TScalar yz, TScalar zx, TScalar xy, TScalar yw, TScalar zw, TScalar xw ) :
		IVector<Bivector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Bivector4<TScalar>>,
		ILinearAlgebraVectorOperators<Bivector4<TScalar>>,
		ILinearAlgebraScalarOperators<Bivector4<TScalar>, TScalar>
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
	public static Bivector4<TScalar> Two { get; } = One + One;

	public Multivector4<TScalar> GetMultivector() => new( TScalar.Zero, Vector4<TScalar>.Zero, this, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );

	public Bivector4<TScalar> Negate() => new( -this.YZ, -this.ZX, -this.XY, -this.YW, -this.ZW, -this.XW );
	public Bivector4<TScalar> Add( in Bivector4<TScalar> r ) => new( this.YZ + r.YZ, this.ZX + r.ZX, this.XY + r.XY, this.YW + r.YW, this.ZW + r.ZW, this.XW + r.XW );
	public Bivector4<TScalar> Subtract( in Bivector4<TScalar> r ) => new( this.YZ - r.YZ, this.ZX - r.ZX, this.XY - r.XY, this.YW - r.YW, this.ZW - r.ZW, this.XW - r.XW );
	public Bivector4<TScalar> ScalarMultiply( TScalar r ) => new( this.YZ * r, this.ZX * r, this.XY * r, this.YW * r, this.ZW * r, this.XW * r );
	public Bivector4<TScalar> ScalarDivide( TScalar r ) => new( this.YZ / r, this.ZX / r, this.XY / r, this.YW / r, this.ZW / r, this.XW / r );
	public static Bivector4<TScalar> DivideScalar( TScalar l, in Bivector4<TScalar> r ) => new( l / r.YZ, l / r.ZX, l / r.XY, l / r.YW, l / r.ZW, l / r.XW );
	public TScalar Dot( in Bivector4<TScalar> r ) => (-this.YZ * r.YZ) - (this.ZX * r.ZX) - (this.XY * r.XY) - (this.YW * r.YW) - (this.ZW * r.ZW) - (this.XW * r.XW);
	public TScalar MagnitudeSquared() => (this.YZ * this.YZ) + (this.ZX * this.ZX) + (this.XY * this.XY) + (this.YW * this.YW) + (this.ZW * this.ZW) + (this.XW * this.XW);

	public static Bivector4<TScalar> operator -( in Bivector4<TScalar> l ) => l.Negate();
	public static Bivector4<TScalar> operator +( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => l.Add( r );
	public static Bivector4<TScalar> operator -( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => l.Subtract( r );
	public static Bivector4<TScalar> operator *( in Bivector4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Bivector4<TScalar> operator *( TScalar l, in Bivector4<TScalar> r ) => r.ScalarMultiply( l );
	public static Bivector4<TScalar> operator /( in Bivector4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Bivector4<TScalar> operator /( TScalar l, in Bivector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY && l.YW == r.YW && l.ZW == r.ZW && l.XW == r.XW;
	public static bool operator !=( in Bivector4<TScalar> l, in Bivector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( this.YZ, this.ZX, this.XY, this.YW, this.ZW, this.XW );
	public override string ToString()
		=> $"[{this.YZ.ToFormattedString( true )}YZ {this.ZX.ToFormattedString()}ZX {this.XY.ToFormattedString()}XY {this.YW.ToFormattedString()}YW {this.ZW.ToFormattedString()}ZW {this.XW.ToFormattedString()}XW]";
}
