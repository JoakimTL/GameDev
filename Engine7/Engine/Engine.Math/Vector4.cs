﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector4<TScalar>( TScalar x, TScalar y, TScalar z, TScalar w ) :
		IVector<Vector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Vector4<TScalar>>,
		IEntrywiseProductOperations<Vector4<TScalar>>,
		IEntrywiseOperations<Vector4<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Vector4<TScalar>>,
		ILinearAlgebraScalarOperators<Vector4<TScalar>, TScalar>,
		IEntrywiseMinMaxOperations<Vector4<TScalar>>,
		IVectorPartsOperations<Vector4<TScalar>, TScalar>,
		IProduct<Vector4<TScalar>, Matrix4x4<TScalar>, Vector4<TScalar>>,
		IEntrywiseComparisonOperations<Vector4<TScalar>>,
		IEntrywiseComparisonOperators<Vector4<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar X = x;
	public readonly TScalar Y = y;
	public readonly TScalar Z = z;
	public readonly TScalar W = w;

	public static Vector4<TScalar> AdditiveIdentity => Zero;
	public static Vector4<TScalar> MultiplicativeIdentity => One;
	public static Vector4<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Vector4<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One, TScalar.One );
	public static Vector4<TScalar> Two { get; } = One + One;

	public static Vector4<TScalar> UnitX { get; } = new( TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Vector4<TScalar> UnitY { get; } = new( TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero );
	public static Vector4<TScalar> UnitZ { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero );
	public static Vector4<TScalar> UnitW { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One );

	public Multivector4<TScalar> GetMultivector() => new( TScalar.Zero, this, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );

	public Vector4<TScalar> Negate() => new( -this.X, -this.Y, -this.Z, -this.W );
	public Vector4<TScalar> Add( in Vector4<TScalar> r ) => new( this.X + r.X, this.Y + r.Y, this.Z + r.Z, this.W + r.W );
	public Vector4<TScalar> Subtract( in Vector4<TScalar> r ) => new( this.X - r.X, this.Y - r.Y, this.Z - r.Z, this.W - r.W );
	public Vector4<TScalar> ScalarMultiply( TScalar r ) => new( this.X * r, this.Y * r, this.Z * r, this.W * r );
	public Vector4<TScalar> ScalarDivide( TScalar r ) => new( this.X / r, this.Y / r, this.Z / r, this.W / r );
	public static Vector4<TScalar> DivideScalar( TScalar l, in Vector4<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z, l / r.W );
	public Vector4<TScalar> MultiplyEntrywise( in Vector4<TScalar> r ) => new( this.X * r.X, this.Y * r.Y, this.Z * r.Z, this.W * r.W );
	public Vector4<TScalar> DivideEntrywise( in Vector4<TScalar> r ) => new( this.X / r.X, this.Y / r.Y, this.Z / r.Z, this.W / r.W );
	public Vector4<TScalar> EntrywiseOperation( Func<TScalar, TScalar> operation ) => new( operation( this.X ), operation( this.Y ), operation( this.Z ), operation( this.W ) );
	public TScalar Dot( in Vector4<TScalar> r ) => (this.X * r.X) + (this.Y * r.Y) + (this.Z * r.Z) + (this.W * r.W);
	public TScalar MagnitudeSquared() => Dot( this );
	public Vector4<TScalar> Min( in Vector4<TScalar> r ) => new( TScalar.Min( this.X, r.X ), TScalar.Min( this.Y, r.Y ), TScalar.Min( this.Z, r.Z ), TScalar.Min( this.W, r.W ) );
	public Vector4<TScalar> Max( in Vector4<TScalar> r ) => new( TScalar.Max( this.X, r.X ), TScalar.Max( this.Y, r.Y ), TScalar.Max( this.Z, r.Z ), TScalar.Max( this.W, r.W ) );
	public TScalar SumOfParts() => this.X + this.Y + this.Z + this.W;
	public TScalar ProductOfParts() => this.X * this.Y * this.Z * this.W;
	public TScalar SumOfUnitBasisAreas() => (this.X * this.Y) + (this.Y * this.Z) + (this.Z * this.X) + (this.W * this.X) + (this.W * this.Y) + (this.W * this.Z);
	public TScalar SumOfUnitBasisVolumes() => (this.X * this.Y * this.Z) + (this.Y * this.Z * this.W) + (this.Z * this.W * this.X) + (this.W * this.X * this.Y);

	public Vector4<TScalar> Multiply( in Matrix4x4<TScalar> m ) => new( Dot( m.Col0 ), Dot( m.Col1 ), Dot( m.Col2 ), Dot( m.Col3 ) );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, in Matrix4x4<TScalar> r ) => l.Multiply( r );

	public static Vector4<TScalar> operator -( in Vector4<TScalar> l ) => l.Negate();
	public static Vector4<TScalar> operator +( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.Add( r );
	public static Vector4<TScalar> operator -( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.Subtract( r );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Vector4<TScalar> operator *( TScalar l, in Vector4<TScalar> r ) => r.ScalarMultiply( l );
	public static Vector4<TScalar> operator /( in Vector4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Vector4<TScalar> operator /( TScalar l, in Vector4<TScalar> r ) => DivideScalar( l, r );

	public bool GreaterThanEntrywise( in Vector4<TScalar> other ) => this.X > other.X && this.Y > other.Y && this.Z > other.Z && this.W > other.W;
	public bool GreaterThanOrEqualEntrywise( in Vector4<TScalar> other ) => this.X >= other.X && this.Y >= other.Y && this.Z >= other.Z && this.W >= other.W;
	public static bool operator <( in Vector4<TScalar> left, in Vector4<TScalar> right ) => right.GreaterThanEntrywise( left );
	public static bool operator >( in Vector4<TScalar> left, in Vector4<TScalar> right ) => left.GreaterThanEntrywise( right );
	public static bool operator <=( in Vector4<TScalar> left, in Vector4<TScalar> right ) => right.GreaterThanOrEqualEntrywise( left );
	public static bool operator >=( in Vector4<TScalar> left, in Vector4<TScalar> right ) => left.GreaterThanOrEqualEntrywise( right );

	public static bool operator ==( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.X == r.X && l.Y == r.Y && l.Z == r.Z && l.W == r.W;
	public static bool operator !=( in Vector4<TScalar> l, in Vector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( this.X, this.Y );
	public override string ToString()
		=> $"[{this.X.ToFormattedString( true )}X {this.Y.ToFormattedString()}Y {this.Z.ToFormattedString()}Z {this.W.ToFormattedString()}W]";

	public static implicit operator Vector4<TScalar>( TScalar s ) => new( s, s, s, s );
	public static implicit operator Vector4<TScalar>( (TScalar x, TScalar y, TScalar z, TScalar w) tuple ) => new( tuple.x, tuple.y, tuple.z, tuple.w );
}