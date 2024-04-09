using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector2<TScalar>( TScalar x, TScalar y ) :
		IVector<Vector2<TScalar>, TScalar>,
		IOuterProduct<Vector2<TScalar>, Bivector2<TScalar>>,
		IPartOfMultivector<Multivector2<TScalar>, Vector2<TScalar>>,
		IEntrywiseProductOperations<Vector2<TScalar>>,
		IEntrywiseOperations<Vector2<TScalar>, TScalar>,
		ILinearAlgebraOperators<Vector2<TScalar>, TScalar>,
		IVectorPartsOperations<Vector2<TScalar>, TScalar>,
		IEntrywiseMinMaxOperations<Vector2<TScalar>>,
		IReflectable<Vector2<TScalar>, TScalar>,
		IProduct<Vector2<TScalar>, Vector2<TScalar>, Rotor2<TScalar>>,
		IProduct<Vector2<TScalar>, Bivector2<TScalar>, Vector2<TScalar>>,
		IProduct<Vector2<TScalar>, Rotor2<TScalar>, Vector2<TScalar>>,
		IProduct<Vector2<TScalar>, Multivector2<TScalar>, Multivector2<TScalar>>,
		IProduct<Vector2<TScalar>, Matrix2x2<TScalar>, Vector2<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar X = x;
	public readonly TScalar Y = y;

	public static Vector2<TScalar> AdditiveIdentity => Zero;
	public static Vector2<TScalar> MultiplicativeIdentity => One;
	public static Vector2<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero );
	public static Vector2<TScalar> One { get; } = new( TScalar.One, TScalar.One );

	public static Vector2<TScalar> UnitX { get; } = new( TScalar.One, TScalar.Zero );
	public static Vector2<TScalar> UnitY { get; } = new( TScalar.Zero, TScalar.One );

	public Multivector2<TScalar> GetMultivector() => new( TScalar.Zero, this, Bivector2<TScalar>.Zero );

	public Vector2<TScalar> Negate() => new( -X, -Y );
	public Vector2<TScalar> Add( in Vector2<TScalar> r ) => new( X + r.X, Y + r.Y );
	public Vector2<TScalar> Subtract( in Vector2<TScalar> r ) => new( X - r.X, Y - r.Y );
	public Vector2<TScalar> ScalarMultiply( TScalar r ) => new( X * r, Y * r );
	public Vector2<TScalar> ScalarDivide( TScalar r ) => new( X / r, Y / r );
	public static Vector2<TScalar> DivideScalar( TScalar l, in Vector2<TScalar> r ) => new( l / r.X, l / r.Y );
	public Vector2<TScalar> MultiplyEntrywise( in Vector2<TScalar> r ) => new( X * r.X, Y * r.Y );
	public Vector2<TScalar> DivideEntrywise( in Vector2<TScalar> r ) => new( X / r.X, Y / r.Y );
	public Vector2<TScalar> EntrywiseOperation( Func<TScalar, TScalar> operation ) => new( operation( X ), operation( Y ) );
	public TScalar Dot( in Vector2<TScalar> r ) => (X * r.X) + (Y * r.Y);
	public Bivector2<TScalar> Wedge( in Vector2<TScalar> r ) => new( (X * r.Y) - (Y * r.X) );
	public Vector2<TScalar> Min( in Vector2<TScalar> r ) => new( TScalar.Min( X, r.X ), TScalar.Min( Y, r.Y ) );
	public Vector2<TScalar> Max( in Vector2<TScalar> r ) => new( TScalar.Max( X, r.X ), TScalar.Max( Y, r.Y ) );
	public TScalar SumOfParts() => X + Y;
	public TScalar ProductOfParts() => X * Y;
	public Vector2<TScalar> ReflectNormal( in Vector2<TScalar> normal ) => normal.Multiply( this ).Multiply( normal );

	public Rotor2<TScalar> Multiply( in Vector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Vector2<TScalar> Multiply( in Bivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Vector2<TScalar> Multiply( in Rotor2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public Multivector2<TScalar> Multiply( in Multivector2<TScalar> r ) => GeometricAlgebraMath2.Multiply( this, r );
	public static Rotor2<TScalar> operator *( in Vector2<TScalar> l, in Vector2<TScalar> r ) => r.Multiply( l );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, in Bivector2<TScalar> r ) => r.Multiply( l );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, in Rotor2<TScalar> r ) => r.Multiply( l );
	public static Multivector2<TScalar> operator *( in Vector2<TScalar> l, in Multivector2<TScalar> r ) => r.Multiply( l );

	public Vector2<TScalar> Multiply( in Matrix2x2<TScalar> m ) => new( Dot( m.Col0 ), Dot( m.Col1 ) );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, in Matrix2x2<TScalar> r ) => l.Multiply( r );

	public static Vector2<TScalar> operator -( in Vector2<TScalar> l ) => l.Negate();
	public static Vector2<TScalar> operator +( in Vector2<TScalar> l, in Vector2<TScalar> r ) => l.Add( r );
	public static Vector2<TScalar> operator -( in Vector2<TScalar> l, in Vector2<TScalar> r ) => l.Subtract( r );
	public static Vector2<TScalar> operator *( in Vector2<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Vector2<TScalar> operator *( TScalar l, in Vector2<TScalar> r ) => r.ScalarMultiply( l );
	public static Vector2<TScalar> operator /( in Vector2<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Vector2<TScalar> operator /( TScalar l, in Vector2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Vector2<TScalar> l, in Vector2<TScalar> r ) => l.X == r.X && l.Y == r.Y;
	public static bool operator !=( in Vector2<TScalar> l, in Vector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() => $"[{X:N3}X, {Y:N3}Y]";
}
