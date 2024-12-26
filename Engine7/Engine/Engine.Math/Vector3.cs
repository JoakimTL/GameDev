using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine;

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
		IEntrywiseComparisonOperators<Vector3<TScalar>>,
		ITransformableVector<Matrix4x4<TScalar>, Vector3<TScalar>>
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

	public Vector3<TScalar> Negate() => new( -this.X, -this.Y, -this.Z );
	public Vector3<TScalar> Add( in Vector3<TScalar> r ) => new( this.X + r.X, this.Y + r.Y, this.Z + r.Z );
	public Vector3<TScalar> Subtract( in Vector3<TScalar> r ) => new( this.X - r.X, this.Y - r.Y, this.Z - r.Z );
	public Vector3<TScalar> ScalarMultiply( TScalar r ) => new( this.X * r, this.Y * r, this.Z * r );
	public Vector3<TScalar> ScalarDivide( TScalar r ) => new( this.X / r, this.Y / r, this.Z / r );
	public static Vector3<TScalar> DivideScalar( TScalar l, in Vector3<TScalar> r ) => new( l / r.X, l / r.Y, l / r.Z );
	public Vector3<TScalar> MultiplyEntrywise( in Vector3<TScalar> r ) => new( this.X * r.X, this.Y * r.Y, this.Z * r.Z );
	public Vector3<TScalar> DivideEntrywise( in Vector3<TScalar> r ) => new( this.X / r.X, this.Y / r.Y, this.Z / r.Z );
	public Vector3<TScalar> EntrywiseOperation( Func<TScalar, TScalar> operation ) => new( operation( this.X ), operation( this.Y ), operation( this.Z ) );
	public TScalar Dot( in Vector3<TScalar> r ) => (this.X * r.X) + (this.Y * r.Y) + (this.Z * r.Z);
	public TScalar MagnitudeSquared() => Dot( this );
	public Bivector3<TScalar> Wedge( in Vector3<TScalar> r ) => new( (this.Y * r.Z) - (this.Z * r.Y), (this.Z * r.X) - (this.X * r.Z), (this.X * r.Y) - (this.Y * r.X) );
	public Vector3<TScalar> Min( in Vector3<TScalar> r ) => new( TScalar.Min( this.X, r.X ), TScalar.Min( this.Y, r.Y ), TScalar.Min( this.Z, r.Z ) );
	public Vector3<TScalar> Max( in Vector3<TScalar> r ) => new( TScalar.Max( this.X, r.X ), TScalar.Max( this.Y, r.Y ), TScalar.Max( this.Z, r.Z ) );
	public TScalar SumOfParts() => this.X + this.Y + this.Z;
	public TScalar ProductOfParts() => this.X * this.Y * this.Z;
	public TScalar SumOfUnitBasisAreas() => (this.X * this.Y) + (this.Y * this.Z) + (this.Z * this.X);
	public TScalar SumOfUnitBasisVolumes() => this.X * this.Y * this.Z;
	public Vector3<TScalar> ReflectNormal( in Vector3<TScalar> normal ) => normal.Multiply( this ).Multiply( normal ).Vector;
	public Vector3<TScalar>? TransformWorld( in Matrix4x4<TScalar> l ) => l.TransformWorld( this );
	public Vector3<TScalar> TransformNormal( in Matrix4x4<TScalar> l ) => l.TransformNormal( this );

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

	public bool GreaterThanEntrywise( in Vector3<TScalar> other ) => this.X > other.X && this.Y > other.Y && this.Z > other.Z;
	public bool GreaterThanOrEqualEntrywise( in Vector3<TScalar> other ) => this.X >= other.X && this.Y >= other.Y && this.Z >= other.Z;
	public static bool operator <( in Vector3<TScalar> left, in Vector3<TScalar> right ) => right.GreaterThanEntrywise( left );
	public static bool operator >( in Vector3<TScalar> left, in Vector3<TScalar> right ) => left.GreaterThanEntrywise( right );
	public static bool operator <=( in Vector3<TScalar> left, in Vector3<TScalar> right ) => right.GreaterThanOrEqualEntrywise( left );
	public static bool operator >=( in Vector3<TScalar> left, in Vector3<TScalar> right ) => left.GreaterThanOrEqualEntrywise( right );

	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( this.X, this.Y );
	public override string ToString()
		=> $"[{this.X.ToFormattedString( true )}X {this.Y.ToFormattedString()}Y {this.Z.ToFormattedString()}Z]";

	public static implicit operator Vector3<TScalar>( TScalar s ) => new( s, s, s );
	public static implicit operator Vector3<TScalar>( (TScalar x, TScalar y, TScalar z) tuple ) => new( tuple.x, tuple.y, tuple.z );
}
