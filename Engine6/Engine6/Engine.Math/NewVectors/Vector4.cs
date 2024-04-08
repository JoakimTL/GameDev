using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector4<TScalar>( TScalar x, TScalar y, TScalar z, TScalar w ) :
		IVector<Vector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Vector4<TScalar>>,
		IEntrywiseProductOperations<Vector4<TScalar>>,
		IEntrywiseOperations<Vector4<TScalar>, TScalar>,
		ILinearAlgebraOperators<Vector4<TScalar>, TScalar>,
		IEntrywiseProductOperators<Vector4<TScalar>>,
		IEntrywiseMinMaxOperations<Vector4<TScalar>>,
		IVectorPartsOperations<Vector4<TScalar>, TScalar>
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

	public static Vector4<TScalar> UnitX { get; } = new( TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Vector4<TScalar> UnitY { get; } = new( TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero );
	public static Vector4<TScalar> UnitZ { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero );
	public static Vector4<TScalar> UnitW { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One );

	public Multivector4<TScalar> GetMultivector() => new( TScalar.Zero, this, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );

	public Vector4<TScalar> Negate() => new( -X, -Y, -Z, -W );
	public Vector4<TScalar> Add( in Vector4<TScalar> r ) => new( X + r.X, Y + r.Y, Z + r.Z, W + r.W );
	public Vector4<TScalar> Subtract( in Vector4<TScalar> r ) => new( X - r.X, Y - r.Y, Z - r.Z, W - r.W );
	public Vector4<TScalar> ScalarMultiply( TScalar r ) => new( X * r, Y * r, Z * r, W * r );
	public Vector4<TScalar> ScalarDivide( TScalar r ) => new( X / r, Y / r, Z / r, W / r );
	public static Vector4<TScalar> DivideScalar( TScalar l, in Vector4<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z, l / r.W );
	public Vector4<TScalar> MultiplyEntrywise( in Vector4<TScalar> r ) => new( X * r.X, Y * r.Y, Z * r.Z, W * r.W );
	public Vector4<TScalar> DivideEntrywise( in Vector4<TScalar> r ) => new( X / r.X, Y / r.Y, Z / r.Z, W / r.W );
	public Vector4<TScalar> EntrywiseOperation( Func<TScalar, TScalar> operation ) => new( operation( X ), operation( Y ), operation( Z ), operation( W ) );
	public TScalar Dot( in Vector4<TScalar> r ) => (X * r.X) + (Y * r.Y) + (Z * r.Z) + (W * r.W);
	public Vector4<TScalar> Min( in Vector4<TScalar> r ) => new( TScalar.Min( X, r.X ), TScalar.Min( Y, r.Y ), TScalar.Min( Z, r.Z ), TScalar.Min( W, r.W ) );
	public Vector4<TScalar> Max( in Vector4<TScalar> r ) => new( TScalar.Max( X, r.X ), TScalar.Max( Y, r.Y ), TScalar.Max( Z, r.Z ), TScalar.Max( W, r.W ) );
	public TScalar SumOfParts() => X + Y + Z + W;
	public TScalar ProductOfParts() => X * Y * Z * W;

	public static Vector4<TScalar> operator -( in Vector4<TScalar> l ) => l.Negate();
	public static Vector4<TScalar> operator +( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.Add( r );
	public static Vector4<TScalar> operator -( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.Subtract( r );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Vector4<TScalar> operator *( TScalar l, in Vector4<TScalar> r ) => r.ScalarMultiply( l );
	public static Vector4<TScalar> operator /( in Vector4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Vector4<TScalar> operator /( TScalar l, in Vector4<TScalar> r ) => DivideScalar( l, r );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.MultiplyEntrywise( r );
	public static Vector4<TScalar> operator /( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.DivideEntrywise( r );

	public static bool operator ==( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.X == r.X && l.Y == r.Y && l.Z == r.Z && l.W == r.W;
	public static bool operator !=( in Vector4<TScalar> l, in Vector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y, {Z:N3}Z, {W:N3}W]";
}