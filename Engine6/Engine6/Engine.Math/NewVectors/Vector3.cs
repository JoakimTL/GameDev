using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Engine.Math.NewVectors.Calculations;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Vector3<TScalar>( TScalar x, TScalar y, TScalar z ) :
		IVector<Vector3<TScalar>, TScalar>,
		IOuterProduct<Vector3<TScalar>, Bivector3<TScalar>>,
		IPartOfMultivector<Multivector3<TScalar>, Vector3<TScalar>>,
		IEntrywiseProductOperations<Vector3<TScalar>>,
		IEntrywiseOperations<Vector3<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Vector3<TScalar>>,
		ILinearAlgebraScalarOperators<Vector3<TScalar>, TScalar>,
		IEntrywiseMinMaxOperations<Vector3<TScalar>>,
		IVectorPartsOperations<Vector3<TScalar>, TScalar>,
		IReflectable<Vector3<TScalar>, TScalar>,
		IProduct<Vector3<TScalar>, Vector3<TScalar>, Rotor3<TScalar>>,
		IProduct<Vector3<TScalar>, Bivector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Vector3<TScalar>, Trivector3<TScalar>, Bivector3<TScalar>>,
		IProduct<Vector3<TScalar>, Rotor3<TScalar>, Multivector3<TScalar>>,
		IProduct<Vector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>,
		IProduct<Vector3<TScalar>, Matrix3x3<TScalar>, Vector3<TScalar>>,
		IEntrywiseComparisonOperations<Vector3<TScalar>>,
		IEntrywiseComparisonOperators<Vector3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar X = x;
	public readonly TScalar Y = y;
	public readonly TScalar Z = z;

	public static Vector3<TScalar> AdditiveIdentity => Zero;
	public static Vector3<TScalar> MultiplicativeIdentity => One;
	public static Vector3<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Vector3<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One );
	public static Vector3<TScalar> Two { get; } = One + One;

	public static Vector3<TScalar> UnitX { get; } = new( TScalar.One, TScalar.Zero, TScalar.Zero );
	public static Vector3<TScalar> UnitY { get; } = new( TScalar.Zero, TScalar.One, TScalar.Zero );
	public static Vector3<TScalar> UnitZ { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.One );

	public Multivector3<TScalar> GetMultivector() => new( TScalar.Zero, this, Bivector3<TScalar>.Zero, Trivector3<TScalar>.Zero );

	public Vector3<TScalar> Negate() => new( -X, -Y, -Z );
	public Vector3<TScalar> Add( in Vector3<TScalar> r ) => new( X + r.X, Y + r.Y, Z + r.Z );
	public Vector3<TScalar> Subtract( in Vector3<TScalar> r ) => new( X - r.X, Y - r.Y, Z - r.Z );
	public Vector3<TScalar> ScalarMultiply( TScalar r ) => new( X * r, Y * r, Z * r );
	public Vector3<TScalar> ScalarDivide( TScalar r ) => new( X / r, Y / r, Z / r );
	public static Vector3<TScalar> DivideScalar( TScalar l, in Vector3<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z );
	public Vector3<TScalar> MultiplyEntrywise( in Vector3<TScalar> r ) => new( X * r.X, Y * r.Y, Z * r.Z );
	public Vector3<TScalar> DivideEntrywise( in Vector3<TScalar> r ) => new( X / r.X, Y / r.Y, Z / r.Z );
	public Vector3<TScalar> EntrywiseOperation( Func<TScalar, TScalar> operation ) => new( operation( X ), operation( Y ), operation( Z ) );
	public TScalar Dot( in Vector3<TScalar> r ) => (X * r.X) + (Y * r.Y) + (Z * r.Z);
	public Bivector3<TScalar> Wedge( in Vector3<TScalar> r ) => new( (Y * r.Z) - (Z * r.Y), (Z * r.X) - (X * r.Z), (X * r.Y) - (Y * r.X) );
	public Vector3<TScalar> Min( in Vector3<TScalar> r ) => new( TScalar.Min( X, r.X ), TScalar.Min( Y, r.Y ), TScalar.Min( Z, r.Z ) );
	public Vector3<TScalar> Max( in Vector3<TScalar> r ) => new( TScalar.Max( X, r.X ), TScalar.Max( Y, r.Y ), TScalar.Max( Z, r.Z ) );
	public TScalar SumOfParts() => X + Y + Z;
	public TScalar ProductOfParts() => X * Y * Z;
	public TScalar SumOfUnitBasisAreas() => X * Y + Y * Z + Z * X;
	public TScalar SumOfUnitBasisVolumes() => X * Y * Z;
	public Vector3<TScalar> ReflectNormal( in Vector3<TScalar> normal ) => normal.Multiply( this ).Multiply( normal ).Vector;

	public Rotor3<TScalar> Multiply( in Vector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Bivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Bivector3<TScalar> Multiply( in Trivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Rotor3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public Multivector3<TScalar> Multiply( in Multivector3<TScalar> r ) => GeometricAlgebraMath3.Multiply( this, r );
	public static Rotor3<TScalar> operator *( in Vector3<TScalar> l, in Vector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Vector3<TScalar> l, in Bivector3<TScalar> r ) => l.Multiply( r );
	public static Bivector3<TScalar> operator *( in Vector3<TScalar> l, in Trivector3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Vector3<TScalar> l, in Rotor3<TScalar> r ) => l.Multiply( r );
	public static Multivector3<TScalar> operator *( in Vector3<TScalar> l, in Multivector3<TScalar> r ) => l.Multiply( r );

	public Vector3<TScalar> Multiply( in Matrix3x3<TScalar> m ) => new( Dot( m.Col0 ), Dot( m.Col1 ), Dot( m.Col2 ) );
	public static Vector3<TScalar> operator *( in Vector3<TScalar> l, in Matrix3x3<TScalar> r ) => l.Multiply( r );

	public static Vector3<TScalar> operator -( in Vector3<TScalar> l ) => l.Negate();
	public static Vector3<TScalar> operator +( in Vector3<TScalar> l, in Vector3<TScalar> r ) => l.Add( r );
	public static Vector3<TScalar> operator -( in Vector3<TScalar> l, in Vector3<TScalar> r ) => l.Subtract( r );
	public static Vector3<TScalar> operator *( in Vector3<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Vector3<TScalar> operator *( TScalar l, in Vector3<TScalar> r ) => r.ScalarMultiply( l );
	public static Vector3<TScalar> operator /( in Vector3<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Vector3<TScalar> operator /( TScalar l, in Vector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Vector3<TScalar> l, in Vector3<TScalar> r ) => l.X == r.X && l.Y == r.Y && l.Z == r.Z;
	public static bool operator !=( in Vector3<TScalar> l, in Vector3<TScalar> r ) => !(l == r);

	public bool GreaterThanEntrywise( in Vector3<TScalar> other ) => this.X > other.X && Y > other.Y && Z > other.Z;
	public bool GreaterThanOrEqualEntrywise( in Vector3<TScalar> other ) => this.X >= other.X && Y >= other.Y && Z >= other.Z;
	public static bool operator <( in Vector3<TScalar> left, in Vector3<TScalar> right ) => right.GreaterThanEntrywise( left );
	public static bool operator >( in Vector3<TScalar> left, in Vector3<TScalar> right ) => left.GreaterThanEntrywise( right );
	public static bool operator <=( in Vector3<TScalar> left, in Vector3<TScalar> right ) => right.GreaterThanOrEqualEntrywise( left );
	public static bool operator >=( in Vector3<TScalar> left, in Vector3<TScalar> right ) => left.GreaterThanOrEqualEntrywise( right );

	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( X, Y );
	public override string ToString() 
		=> string.Create( CultureInfo.InvariantCulture, 
			$"[{X:#,##0.###}X {Y.SignCharacter()} {TScalar.Abs( Y ):#,##0.###}Y {Z.SignCharacter()} {TScalar.Abs( Z ):#,##0.###}Z]" );
}
