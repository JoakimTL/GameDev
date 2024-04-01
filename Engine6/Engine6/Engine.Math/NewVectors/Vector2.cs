using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector2<TScalar>( TScalar x, TScalar y ) :
		IVector<Vector2<TScalar>, TScalar>,
		IMultivectorPart<Multivector2<TScalar>, Vector2<TScalar>>,
		IEntrywiseProductOperations<Vector2<TScalar>>,
		IEntrywiseOperations<Vector2<TScalar>, TScalar>,
		ILinearAlgebraOperators<Vector2<TScalar>, TScalar>,
		IEntrywiseProductOperators<Vector2<TScalar>>,
		IEntrywiseMinMaxOperations<Vector2<TScalar>>,
		IVectorPartsOperations<Vector2<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar X = x;
	public readonly TScalar Y = y;

	public static Vector2<TScalar> AdditiveIdentity => Zero;
	public static Vector2<TScalar> MultiplicativeIdentity => One;
	public static Vector2<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero );
	public static Vector2<TScalar> One { get; } = new( TScalar.One, TScalar.One );

	public static Multivector2<TScalar> GetMultivector( in Vector2<TScalar> part ) => new( TScalar.Zero, part, Bivector2<TScalar>.Zero );

	public static Vector2<TScalar> Negate( in Vector2<TScalar> l ) => new( -l.X, -l.Y );
	public static Vector2<TScalar> Add( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( l.X + r.X, l.Y + r.Y );
	public static Vector2<TScalar> Subtract( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( l.X - r.X, l.Y - r.Y );
	public static Vector2<TScalar> ScalarMultiply( in Vector2<TScalar> l, TScalar r ) => new( l.X * r, l.Y * r );
	public static Vector2<TScalar> ScalarDivide( in Vector2<TScalar> l, TScalar r ) => new( l.X / r, l.Y / r );
	public static Vector2<TScalar> DivideScalar( TScalar l, in Vector2<TScalar> r ) => new( l / r.X, l / r.Y );
	public static Vector2<TScalar> MultiplyEntrywise( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( l.X * r.X, l.Y * r.Y );
	public static Vector2<TScalar> DivideEntrywise( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( l.X / r.X, l.Y / r.Y );
	public static Vector2<TScalar> EntrywiseOperation( in Vector2<TScalar> v, Func<TScalar, TScalar> operation ) => new( operation( v.X ), operation( v.Y ) );
	public static TScalar Dot( in Vector2<TScalar> l, in Vector2<TScalar> r ) => (l.X * r.X) + (l.Y * r.Y);
	public static Vector2<TScalar> Min( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( TScalar.Min( l.X, r.X ), TScalar.Min( l.Y, r.Y ) );
	public static Vector2<TScalar> Max( in Vector2<TScalar> l, in Vector2<TScalar> r ) => new( TScalar.Max( l.X, r.X ), TScalar.Max( l.Y, r.Y ) );
	public static TScalar SumOfParts( in Vector2<TScalar> v ) => v.X + v.Y;
	public static TScalar ProductOfParts( in Vector2<TScalar> v ) => v.X * v.Y;

	public static Vector2<TScalar> operator -( in Vector2<TScalar> l ) => Negate( l );
	public static Vector2<TScalar> operator +( in Vector2<TScalar> l, in Vector2<TScalar> r ) => Add( l, r );
	public static Vector2<TScalar> operator -( in Vector2<TScalar> l, in Vector2<TScalar> r ) => Subtract( l, r );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Vector2<TScalar> operator *( TScalar l, in Vector2<TScalar> r ) => ScalarMultiply( r, l );
	public static Vector2<TScalar> operator /( in Vector2<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Vector2<TScalar> operator /( TScalar l, in Vector2<TScalar> r ) => DivideScalar( l, r );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, in Vector2<TScalar> r ) => MultiplyEntrywise( l, r );
	public static Vector2<TScalar> operator /( in Vector2<TScalar> l, in Vector2<TScalar> r ) => DivideEntrywise( l, r );

	public static bool operator ==( in Vector2<TScalar> l, in Vector2<TScalar> r ) => l.X == r.X && l.Y == r.Y;
	public static bool operator !=( in Vector2<TScalar> l, in Vector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y]";
}
