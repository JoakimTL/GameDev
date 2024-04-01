using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Trivector4<TScalar>( TScalar yzw, TScalar xzw, TScalar xyw, TScalar xyz ) :
		IVector<Trivector4<TScalar>, TScalar>,
		IMultivectorPart<Multivector4<TScalar>, Trivector4<TScalar>>,
		ILinearAlgebraOperators<Trivector4<TScalar>, TScalar>
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

	public static Multivector4<TScalar> GetMultivector( in Trivector4<TScalar> part ) => new( TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, part, Quadvector4<TScalar>.Zero );

	public static Trivector4<TScalar> Negate( in Trivector4<TScalar> l ) => new( -l.YZW, -l.XZW, -l.XYW, -l.XYZ );
	public static Trivector4<TScalar> Add( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => new( l.YZW + r.YZW, l.XZW + r.XZW, l.XYW + r.XYW, l.XYZ + r.XYZ );
	public static Trivector4<TScalar> Subtract( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => new( l.YZW - r.YZW, l.XZW - r.XZW, l.XYW - r.XYW, l.XYZ - r.XYZ );
	public static Trivector4<TScalar> ScalarMultiply( in Trivector4<TScalar> l, TScalar r ) => new( l.YZW * r, l.XZW * r, l.XYW * r, l.XYZ * r );
	public static Trivector4<TScalar> ScalarDivide( in Trivector4<TScalar> l, TScalar r ) => new( l.YZW / r, l.XZW / r, l.XYW / r, l.XYZ / r );
	public static Trivector4<TScalar> DivideScalar( TScalar l, in Trivector4<TScalar> r ) => new( l / r.YZW, l / r.XZW, l / r.XYW, l / r.XYZ );
	public static TScalar Dot( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => (-l.YZW * r.YZW) - (l.XZW * r.XZW) - (l.XYW * r.XYW) - (l.XYZ * r.XYZ);

	public static Trivector4<TScalar> operator -( in Trivector4<TScalar> l ) => Negate( l );
	public static Trivector4<TScalar> operator +( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => Add( l, r );
	public static Trivector4<TScalar> operator -( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => Subtract( l, r );
	public static Trivector4<TScalar> operator *( in Trivector4<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Trivector4<TScalar> operator *( TScalar l, in Trivector4<TScalar> r ) => ScalarMultiply( r, l );
	public static Trivector4<TScalar> operator /( in Trivector4<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Trivector4<TScalar> operator /( TScalar l, in Trivector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => l.YZW == r.YZW && l.XZW == r.XZW && l.XYW == r.XYW && l.XYZ == r.XYZ;
	public static bool operator !=( in Trivector4<TScalar> l, in Trivector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Trivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZW, XZW, XYW, XYZ );
	public override string ToString() => $"[{YZW:N3}YZW, {XZW:N3}XZW, {XYW:N3}XYW, {XYZ:N3}XYZ]";
}
