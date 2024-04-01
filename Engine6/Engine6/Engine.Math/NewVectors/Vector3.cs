using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector3<TScalar>( TScalar x, TScalar y, TScalar z ) :
		IVector<Vector3<TScalar>, TScalar>,
		IMultivectorPart<Multivector3<TScalar>, Vector3<TScalar>>,
		IEntrywiseProductOperations<Vector3<TScalar>>,
		IEntrywiseOperations<Vector3<TScalar>, TScalar>,
		ILinearAlgebraOperators<Vector3<TScalar>, TScalar>,
		IEntrywiseProductOperators<Vector3<TScalar>>,
		IEntrywiseMinMaxOperations<Vector3<TScalar>>,
		IVectorPartsOperations<Vector3<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar X = x;
	public readonly TScalar Y = y;
	public readonly TScalar Z = z;

	public static Vector3<TScalar> AdditiveIdentity => Zero;
	public static Vector3<TScalar> MultiplicativeIdentity => One;
	public static Vector3<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Vector3<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One );

	public static Multivector3<TScalar> GetMultivector( in Vector3<TScalar> part ) => new( TScalar.Zero, part, Bivector3<TScalar>.Zero, Trivector3<TScalar>.Zero );

	public static Vector3<TScalar> Negate( in Vector3<TScalar> l ) => new( -l.X, -l.Y, -l.Z );
	public static Vector3<TScalar> Add( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( l.X + r.X, l.Y + r.Y, l.Z + r.Z );
	public static Vector3<TScalar> Subtract( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( l.X - r.X, l.Y - r.Y, l.Z - r.Z );
	public static Vector3<TScalar> ScalarMultiply( in Vector3<TScalar> l, TScalar r ) => new( l.X * r, l.Y * r, l.Z * r );
	public static Vector3<TScalar> ScalarDivide( in Vector3<TScalar> l, TScalar r ) => new( l.X / r, l.Y / r, l.Z / r );
	public static Vector3<TScalar> DivideScalar( TScalar l, in Vector3<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z );
	public static Vector3<TScalar> MultiplyEntrywise( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( l.X * r.X, l.Y * r.Y, l.Z * r.Z );
	public static Vector3<TScalar> DivideEntrywise( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( l.X / r.X, l.Y / r.Y, l.Z / r.Z );
	public static Vector3<TScalar> EntrywiseOperation( in Vector3<TScalar> v, Func<TScalar, TScalar> operation ) => new( operation( v.X ), operation( v.Y ), operation( v.Z ) );
	public static TScalar Dot( in Vector3<TScalar> l, in Vector3<TScalar> r ) => (l.X * r.X) + (l.Y * r.Y) + (l.Z * r.Z);
	public static Vector3<TScalar> Min( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( TScalar.Min( l.X, r.X ), TScalar.Min( l.Y, r.Y ), TScalar.Min( l.Z, r.Z ) );
	public static Vector3<TScalar> Max( in Vector3<TScalar> l, in Vector3<TScalar> r ) => new( TScalar.Max( l.X, r.X ), TScalar.Max( l.Y, r.Y ), TScalar.Max( l.Z, r.Z ) );
	public static TScalar SumOfParts( in Vector3<TScalar> v ) => v.X + v.Y + v.Z;
	public static TScalar ProductOfParts( in Vector3<TScalar> v ) => v.X * v.Y * v.Z;

	public static Vector3<TScalar> operator -( in Vector3<TScalar> l ) => Negate( l );
	public static Vector3<TScalar> operator +( in Vector3<TScalar> l, in Vector3<TScalar> r ) => Add( l, r );
	public static Vector3<TScalar> operator -( in Vector3<TScalar> l, in Vector3<TScalar> r ) => Subtract( l, r );
	public static Vector3<TScalar> operator *( in Vector3<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Vector3<TScalar> operator *( TScalar l, in Vector3<TScalar> r ) => ScalarMultiply( r, l );
	public static Vector3<TScalar> operator /( in Vector3<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Vector3<TScalar> operator /( TScalar l, in Vector3<TScalar> r ) => DivideScalar( l, r );
	public static Vector3<TScalar> operator *( in Vector3<TScalar> l, in Vector3<TScalar> r ) => MultiplyEntrywise( l, r );
	public static Vector3<TScalar> operator /( in Vector3<TScalar> l, in Vector3<TScalar> r ) => DivideEntrywise( l, r );

	public static bool operator ==( in Vector3<TScalar> l, in Vector3<TScalar> r ) => l.X == r.X && l.Y == r.Y && l.Z == r.Z;
	public static bool operator !=( in Vector3<TScalar> l, in Vector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y, {Z:N3}Z]";
}
