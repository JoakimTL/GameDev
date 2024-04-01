using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector3<TScalar>( TScalar yz, TScalar zx, TScalar xy ) :
		IVector<Bivector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Bivector3<TScalar>>,
		ILinearAlgebraOperators<Bivector3<TScalar>, TScalar>,
		IGeometricProduct<Bivector3<TScalar>, Vector3<TScalar>, Multivector3<TScalar>>,
		IGeometricProduct<Bivector3<TScalar>, Bivector3<TScalar>, Rotor3<TScalar>>,
		IGeometricProduct<Bivector3<TScalar>, Trivector3<TScalar>, Vector3<TScalar>>,
		IGeometricProduct<Bivector3<TScalar>, Rotor3<TScalar>, Rotor3<TScalar>>,
		IGeometricProduct<Bivector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar YZ = yz;
	public readonly TScalar ZX = zx;
	public readonly TScalar XY = xy;

	public static Bivector3<TScalar> AdditiveIdentity => Zero;
	public static Bivector3<TScalar> MultiplicativeIdentity => One;
	public static Bivector3<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Bivector3<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One );

	public Multivector3<TScalar> GetMultivector() => new( TScalar.Zero, Vector3<TScalar>.Zero, this, Trivector3<TScalar>.Zero );

	public Bivector3<TScalar> Negate() => new( -YZ, -ZX, -XY );
	public Bivector3<TScalar> Add( in Bivector3<TScalar> r ) => new( YZ + r.YZ, ZX + r.ZX, XY + r.XY );
	public Bivector3<TScalar> Subtract( in Bivector3<TScalar> r ) => new( YZ - r.YZ, ZX - r.ZX, XY - r.XY );
	public Bivector3<TScalar> ScalarMultiply( TScalar r ) => new( YZ * r, ZX * r, XY * r );
	public Bivector3<TScalar> ScalarDivide( TScalar r ) => new( YZ / r, ZX / r, XY / r );
	public static Bivector3<TScalar> DivideScalar( TScalar l, in Bivector3<TScalar> r ) => new( l / r.YZ, l / r.ZX, l / r.XY );
	public TScalar Dot( in Bivector3<TScalar> r ) => -(YZ * r.YZ) - (ZX * r.ZX) - (XY * r.XY);

	public Multivector3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Vector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );

	public static Bivector3<TScalar> operator -( in Bivector3<TScalar> l ) => l.Negate();
	public static Bivector3<TScalar> operator +( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => l.Add( r );
	public static Bivector3<TScalar> operator -( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => l.Subtract( r );
	public static Bivector3<TScalar> operator *( in Bivector3<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Bivector3<TScalar> operator *( TScalar l, in Bivector3<TScalar> r ) => r.ScalarMultiply( l );
	public static Bivector3<TScalar> operator /( in Bivector3<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Bivector3<TScalar> operator /( TScalar l, in Bivector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY;
	public static bool operator !=( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZ, ZX, XY );
	public override string ToString() => $"[{YZ:N3}YZ, {ZX:N3}ZX, {XY:N3}XY]";
}
