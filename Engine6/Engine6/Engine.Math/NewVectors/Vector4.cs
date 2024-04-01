using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector4<TScalar>( TScalar x, TScalar y, TScalar z, TScalar w ) :
		IVector<Vector4<TScalar>, TScalar>,
		IMultivectorPart<Multivector4<TScalar>, Vector4<TScalar>>,
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

	public static Multivector4<TScalar> GetMultivector( in Vector4<TScalar> part ) => new( TScalar.Zero, part, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );

	public static Vector4<TScalar> Negate( in Vector4<TScalar> l ) => new( -l.X, -l.Y, -l.Z, -l.W );
	public static Vector4<TScalar> Add( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W );
	public static Vector4<TScalar> Subtract( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W );
	public static Vector4<TScalar> ScalarMultiply( in Vector4<TScalar> l, TScalar r ) => new( l.X * r, l.Y * r, l.Z * r, l.W * r );
	public static Vector4<TScalar> ScalarDivide( in Vector4<TScalar> l, TScalar r ) => new( l.X / r, l.Y / r, l.Z / r, l.W / r );
	public static Vector4<TScalar> DivideScalar( TScalar l, in Vector4<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z, l / r.W );
	public static Vector4<TScalar> MultiplyEntrywise( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W );
	public static Vector4<TScalar> DivideEntrywise( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( l.X / r.X, l.Y / r.Y, l.Z / r.Z, l.W / r.W );
	public static Vector4<TScalar> EntrywiseOperation( in Vector4<TScalar> v, Func<TScalar, TScalar> operation ) => new( operation( v.X ), operation( v.Y ), operation( v.Z ), operation( v.W ) );
	public static TScalar Dot( in Vector4<TScalar> l, in Vector4<TScalar> r ) => (l.X * r.X) + (l.Y * r.Y) + (l.Z * r.Z) + (l.W * r.W);
	public static Vector4<TScalar> Min( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( TScalar.Min( l.X, r.X ), TScalar.Min( l.Y, r.Y ), TScalar.Min( l.Z, r.Z ), TScalar.Min( l.W, r.W ) );
	public static Vector4<TScalar> Max( in Vector4<TScalar> l, in Vector4<TScalar> r ) => new( TScalar.Max( l.X, r.X ), TScalar.Max( l.Y, r.Y ), TScalar.Max( l.Z, r.Z ), TScalar.Max( l.W, r.W ) );
	public static TScalar SumOfParts( in Vector4<TScalar> v ) => v.X + v.Y + v.Z + v.W;
	public static TScalar ProductOfParts( in Vector4<TScalar> v ) => v.X * v.Y * v.Z * v.W;

	public static Vector4<TScalar> operator -( in Vector4<TScalar> l ) => Negate( l );
	public static Vector4<TScalar> operator +( in Vector4<TScalar> l, in Vector4<TScalar> r ) => Add( l, r );
	public static Vector4<TScalar> operator -( in Vector4<TScalar> l, in Vector4<TScalar> r ) => Subtract( l, r );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Vector4<TScalar> operator *( TScalar l, in Vector4<TScalar> r ) => ScalarMultiply( r, l );
	public static Vector4<TScalar> operator /( in Vector4<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Vector4<TScalar> operator /( TScalar l, in Vector4<TScalar> r ) => DivideScalar( l, r );
	public static Vector4<TScalar> operator *( in Vector4<TScalar> l, in Vector4<TScalar> r ) => MultiplyEntrywise( l, r );
	public static Vector4<TScalar> operator /( in Vector4<TScalar> l, in Vector4<TScalar> r ) => DivideEntrywise( l, r );

	public static bool operator ==( in Vector4<TScalar> l, in Vector4<TScalar> r ) => l.X == r.X && l.Y == r.Y && l.Z == r.Z && l.W == r.W;
	public static bool operator !=( in Vector4<TScalar> l, in Vector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y, {Z:N3}Z, {W:N3}W]";
}