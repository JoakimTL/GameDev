using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Trivector4<TScalar>( TScalar yzw, TScalar xzw, TScalar xyw, TScalar xyz ) :
		IVector<Trivector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Trivector4<TScalar>>,
		ILinearAlgebraVectorOperators<Trivector4<TScalar>>,
		ILinearAlgebraScalarOperators<Trivector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar YZW = yzw;
	public readonly TScalar XZW = xzw;
	public readonly TScalar XYW = xyw;
	public readonly TScalar XYZ = xyz;

	public static Trivector4<TScalar> AdditiveIdentity => Zero;
	public static Trivector4<TScalar> MultiplicativeIdentity => One;
	public static Trivector4<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Trivector4<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One, TScalar.One );
	public static Trivector4<TScalar> Two { get; } = One + One;

	public Multivector4<TScalar> GetMultivector() => new( TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, this, Quadvector4<TScalar>.Zero );

	public Trivector4<TScalar> Negate() => new( -YZW, -XZW, -XYW, -XYZ );
	public Trivector4<TScalar> Add( in Trivector4<TScalar> r ) => new( YZW + r.YZW, XZW + r.XZW, XYW + r.XYW, XYZ + r.XYZ );
	public Trivector4<TScalar> Subtract( in Trivector4<TScalar> r ) => new( YZW - r.YZW, XZW - r.XZW, XYW - r.XYW, XYZ - r.XYZ );
	public Trivector4<TScalar> ScalarMultiply( TScalar r ) => new( YZW * r, XZW * r, XYW * r, XYZ * r );
	public Trivector4<TScalar> ScalarDivide( TScalar r ) => new( YZW / r, XZW / r, XYW / r, XYZ / r );
	public static Trivector4<TScalar> DivideScalar( TScalar l, in Trivector4<TScalar> r ) => new( l / r.YZW, l / r.XZW, l / r.XYW, l / r.XYZ );
	public TScalar Dot( in Trivector4<TScalar> r ) => (-YZW * r.YZW) - (XZW * r.XZW) - (XYW * r.XYW) - (XYZ * r.XYZ);
	public TScalar MagnitudeSquared() => (YZW * YZW) + (XZW * XZW) + (XYW * XYW) + (XYZ * XYZ);

	public static Trivector4<TScalar> operator -( in Trivector4<TScalar> l ) => l.Negate();
	public static Trivector4<TScalar> operator +( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => l.Add( r );
	public static Trivector4<TScalar> operator -( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => l.Subtract( r );
	public static Trivector4<TScalar> operator *( in Trivector4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Trivector4<TScalar> operator *( TScalar l, in Trivector4<TScalar> r ) => r.ScalarMultiply( l );
	public static Trivector4<TScalar> operator /( in Trivector4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Trivector4<TScalar> operator /( TScalar l, in Trivector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => l.YZW == r.YZW && l.XZW == r.XZW && l.XYW == r.XYW && l.XYZ == r.XYZ;
	public static bool operator !=( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Trivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZW, XZW, XYW, XYZ );
	public override string ToString()
		=> $"[{YZW.ToFormattedString( true )}YZW {XZW.ToFormattedString()}XZW {XYW.ToFormattedString()}XYW {XYZ.ToFormattedString()}XYZ]";
}
