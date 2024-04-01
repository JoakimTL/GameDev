using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector3<TScalar>( TScalar yz, TScalar zx, TScalar xy ) :
		IVector<Bivector3<TScalar>, TScalar>,
		IMultivectorPart<Multivector3<TScalar>, Bivector3<TScalar>>,
		ILinearAlgebraOperators<Bivector3<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar YZ = yz;
	public readonly TScalar ZX = zx;
	public readonly TScalar XY = xy;

	public static Bivector3<TScalar> AdditiveIdentity => Zero;
	public static Bivector3<TScalar> MultiplicativeIdentity => One;
	public static Bivector3<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Bivector3<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One );

	public static Multivector3<TScalar> GetMultivector( in Bivector3<TScalar> part ) => new( TScalar.Zero, Vector3<TScalar>.Zero, part, Trivector3<TScalar>.Zero );

	public static Bivector3<TScalar> Negate( in Bivector3<TScalar> l ) => new( -l.YZ, -l.ZX, -l.XY );
	public static Bivector3<TScalar> Add( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => new( l.YZ + r.YZ, l.ZX + r.ZX, l.XY + r.XY );
	public static Bivector3<TScalar> Subtract( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => new( l.YZ - r.YZ, l.ZX - r.ZX, l.XY - r.XY );
	public static Bivector3<TScalar> ScalarMultiply( in Bivector3<TScalar> l, TScalar r ) => new( l.YZ * r, l.ZX * r, l.XY * r );
	public static Bivector3<TScalar> ScalarDivide( in Bivector3<TScalar> l, TScalar r ) => new( l.YZ / r, l.ZX / r, l.XY / r );
	public static Bivector3<TScalar> DivideScalar( TScalar l, in Bivector3<TScalar> r ) => new( l / r.YZ, l / r.ZX, l / r.XY );
	public static TScalar Dot( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => (-l.YZ * r.YZ) - (l.ZX * r.ZX) - (l.XY * r.XY);

	public static Bivector3<TScalar> operator -( in Bivector3<TScalar> l ) => Negate( l );
	public static Bivector3<TScalar> operator +( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => Add( l, r );
	public static Bivector3<TScalar> operator -( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => Subtract( l, r );
	public static Bivector3<TScalar> operator *( in Bivector3<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Bivector3<TScalar> operator *( TScalar l, in Bivector3<TScalar> r ) => ScalarMultiply( r, l );
	public static Bivector3<TScalar> operator /( in Bivector3<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Bivector3<TScalar> operator /( TScalar l, in Bivector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY;
	public static bool operator !=( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZ, ZX, XY );
	public override string ToString() => $"[{YZ:N3}YZ, {ZX:N3}ZX, {XY:N3}XY]";
}
