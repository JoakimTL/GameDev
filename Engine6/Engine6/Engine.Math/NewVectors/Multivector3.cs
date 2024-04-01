using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector3<TScalar>( TScalar scalar, Vector3<TScalar> vector, Bivector3<TScalar> bivector, Trivector3<TScalar> trivector ) :
		IVector<Multivector3<TScalar>, TScalar>,
		IMultivectorPart<Multivector3<TScalar>, Multivector3<TScalar>>,
		ILinearAlgebraOperators<Multivector3<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Vector3<TScalar> Vector = vector;
	public readonly Bivector3<TScalar> Bivector = bivector;
	public readonly Trivector3<TScalar> Trivector = trivector;

	public Multivector3(TScalar scalar, TScalar x, TScalar y, TScalar z, TScalar yz, TScalar zx, TScalar xy, TScalar xyz) 
		: this( scalar, new( x, y, z ), new( yz, zx, xy ), new( xyz ) ) { }

	public static Multivector3<TScalar> AdditiveIdentity => Zero;
	public static Multivector3<TScalar> MultiplicativeIdentity => One;
	public static Multivector3<TScalar> Zero { get; } = new( TScalar.Zero, Vector3<TScalar>.Zero, Bivector3<TScalar>.Zero, Trivector3<TScalar>.Zero );
	public static Multivector3<TScalar> One { get; } = new( TScalar.One, Vector3<TScalar>.One, Bivector3<TScalar>.One, Trivector3<TScalar>.One );

	public static Multivector3<TScalar> GetMultivector( in Multivector3<TScalar> part ) => part;

	public static Multivector3<TScalar> Negate( in Multivector3<TScalar> l ) => new( -l.Scalar, -l.Vector, -l.Bivector, -l.Trivector );
	public static Multivector3<TScalar> Add( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => new( l.Scalar + r.Scalar, l.Vector + r.Vector, l.Bivector + r.Bivector, l.Trivector + r.Trivector );
	public static Multivector3<TScalar> Subtract( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => new( l.Scalar - r.Scalar, l.Vector - r.Vector, l.Bivector - r.Bivector, l.Trivector - r.Trivector );
	public static Multivector3<TScalar> ScalarMultiply( in Multivector3<TScalar> l, TScalar r ) => new( l.Scalar * r, l.Vector * r, l.Bivector * r, l.Trivector * r );
	public static Multivector3<TScalar> ScalarDivide( in Multivector3<TScalar> l, TScalar r ) => new( l.Scalar / r, l.Vector / r, l.Bivector / r, l.Trivector / r );
	public static Multivector3<TScalar> DivideScalar( TScalar l, in Multivector3<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector, l / r.Trivector );
	public static TScalar Dot( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => (l.Scalar * r.Scalar) + Vector3<TScalar>.Dot( l.Vector, r.Vector ) + Bivector3<TScalar>.Dot( l.Bivector, r.Bivector ) + Trivector3<TScalar>.Dot( l.Trivector, r.Trivector );

	public static Multivector3<TScalar> operator -( in Multivector3<TScalar> l ) => Negate( l );
	public static Multivector3<TScalar> operator +( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => Add( l, r );
	public static Multivector3<TScalar> operator -( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => Subtract( l, r );
	public static Multivector3<TScalar> operator *( in Multivector3<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Multivector3<TScalar> operator *( TScalar l, in Multivector3<TScalar> r ) => ScalarMultiply( r, l );
	public static Multivector3<TScalar> operator /( in Multivector3<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Multivector3<TScalar> operator /( TScalar l, in Multivector3<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector;
	public static bool operator !=( in Multivector3<TScalar> l, in Multivector3<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Multivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Vector, Bivector, Trivector );
	public override string ToString() => $"[{Scalar:N3}+{Vector}+{Bivector}+{Trivector}]";
}
