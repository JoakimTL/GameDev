﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector3<TScalar>( TScalar yz, TScalar zx, TScalar xy ) :
		IVector<Bivector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Bivector3<TScalar>>,
		ILinearAlgebraVectorOperators<Bivector3<TScalar>>,
		ILinearAlgebraScalarOperators<Bivector3<TScalar>, TScalar>,
		IProduct<Bivector3<TScalar>, Vector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Bivector3<TScalar>, Bivector3<TScalar>, Rotor3<TScalar>>,
		IProduct<Bivector3<TScalar>, Trivector3<TScalar>, Vector3<TScalar>>,
		IProduct<Bivector3<TScalar>, Rotor3<TScalar>, Rotor3<TScalar>>,
		IProduct<Bivector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar YZ = yz;
	public readonly TScalar ZX = zx;
	public readonly TScalar XY = xy;

	public static Bivector3<TScalar> AdditiveIdentity => Zero;
	public static Bivector3<TScalar> MultiplicativeIdentity => One;
	public static Bivector3<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Bivector3<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One );
	public static Bivector3<TScalar> Two { get; } = One + One;

	public Multivector3<TScalar> GetMultivector() => new( TScalar.Zero, Vector3<TScalar>.Zero, this, Trivector3<TScalar>.Zero );

	public Bivector3<TScalar> Negate() => new( -this.YZ, -this.ZX, -this.XY );
	public Bivector3<TScalar> Add( in Bivector3<TScalar> r ) => new( this.YZ + r.YZ, this.ZX + r.ZX, this.XY + r.XY );
	public Bivector3<TScalar> Subtract( in Bivector3<TScalar> r ) => new( this.YZ - r.YZ, this.ZX - r.ZX, this.XY - r.XY );
	public Bivector3<TScalar> ScalarMultiply( TScalar r ) => new( this.YZ * r, this.ZX * r, this.XY * r );
	public Bivector3<TScalar> ScalarDivide( TScalar r ) => new( this.YZ / r, this.ZX / r, this.XY / r );
	public static Bivector3<TScalar> DivideScalar( TScalar l, in Bivector3<TScalar> r ) => new( l / r.YZ, l / r.ZX, l / r.XY );
	public TScalar Dot( in Bivector3<TScalar> r ) => -(this.YZ * r.YZ) - (this.ZX * r.ZX) - (this.XY * r.XY);
	public TScalar MagnitudeSquared() => (this.YZ * this.YZ) + (this.ZX * this.ZX) + (this.XY * this.XY);

	public Multivector3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Vector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Rotor3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public static Multivector3<TScalar> operator *( in Bivector3<TScalar> l, in Vector3<TScalar> r ) => l.Multiply( r );
	public static Rotor3<TScalar> operator *( in Bivector3<TScalar> l, in Bivector3<TScalar> r ) => l.Multiply( r );
	public static Vector3<TScalar> operator *( in Bivector3<TScalar> l, in Trivector3<TScalar> r ) => l.Multiply( r );
	public static Rotor3<TScalar> operator *( in Bivector3<TScalar> l, in Rotor3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Bivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Multiply( r );

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
	public override int GetHashCode() => HashCode.Combine( this.YZ, this.ZX, this.XY );
	public override string ToString()
		=> $"[{this.YZ.ToFormattedString( true )}YZ {this.ZX.ToFormattedString()}ZX {this.XY.ToFormattedString()}XY]";
}
