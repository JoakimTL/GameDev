using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector4<TScalar>( TScalar scalar, Vector4<TScalar> vector, Bivector4<TScalar> bivector, Trivector4<TScalar> trivector, Quadvector4<TScalar> quadvector ) :
		IVector<Multivector4<TScalar>, TScalar>,
		IMultivectorPart<Multivector4<TScalar>, Multivector4<TScalar>>,
		ILinearAlgebraOperators<Multivector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Vector4<TScalar> Vector = vector;
	public readonly Bivector4<TScalar> Bivector = bivector;
	public readonly Trivector4<TScalar> Trivector = trivector;
	public readonly Quadvector4<TScalar> Quadvector = quadvector;

	public static Multivector4<TScalar> AdditiveIdentity => Zero;
	public static Multivector4<TScalar> MultiplicativeIdentity => One;
	public static Multivector4<TScalar> Zero { get; } = new( TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero );
	public static Multivector4<TScalar> One { get; } = new( TScalar.One, Vector4<TScalar>.One, Bivector4<TScalar>.One, Trivector4<TScalar>.One, Quadvector4<TScalar>.One );

	public static Multivector4<TScalar> GetMultivector( in Multivector4<TScalar> part ) => part;

	public static Multivector4<TScalar> Negate( in Multivector4<TScalar> l ) => new( -l.Scalar, -l.Vector, -l.Bivector, -l.Trivector, -l.Quadvector );
	public static Multivector4<TScalar> Add( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => new( l.Scalar + r.Scalar, l.Vector + r.Vector, l.Bivector + r.Bivector, l.Trivector + r.Trivector, l.Quadvector + r.Quadvector );
	public static Multivector4<TScalar> Subtract( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => new( l.Scalar - r.Scalar, l.Vector - r.Vector, l.Bivector - r.Bivector, l.Trivector - r.Trivector, l.Quadvector - r.Quadvector );
	public static Multivector4<TScalar> ScalarMultiply( in Multivector4<TScalar> l, TScalar r ) => new( l.Scalar * r, l.Vector * r, l.Bivector * r, l.Trivector * r, l.Quadvector * r );
	public static Multivector4<TScalar> ScalarDivide( in Multivector4<TScalar> l, TScalar r ) => new( l.Scalar / r, l.Vector / r, l.Bivector / r, l.Trivector / r, l.Quadvector / r );
	public static Multivector4<TScalar> DivideScalar( TScalar l, in Multivector4<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector, l / r.Trivector, l / r.Quadvector );
	public static TScalar Dot( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => (l.Scalar * r.Scalar) + Vector4<TScalar>.Dot( l.Vector, r.Vector ) + Bivector4<TScalar>.Dot( l.Bivector, r.Bivector ) + Trivector4<TScalar>.Dot( l.Trivector, r.Trivector ) + Quadvector4<TScalar>.Dot( l.Quadvector, r.Quadvector );

	public static Multivector4<TScalar> operator -( in Multivector4<TScalar> l ) => Negate( l );
	public static Multivector4<TScalar> operator +( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => Add( l, r );
	public static Multivector4<TScalar> operator -( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => Subtract( l, r );
	public static Multivector4<TScalar> operator *( in Multivector4<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Multivector4<TScalar> operator *( TScalar l, in Multivector4<TScalar> r ) => ScalarMultiply( r, l );
	public static Multivector4<TScalar> operator /( in Multivector4<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Multivector4<TScalar> operator /( TScalar l, in Multivector4<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector && l.Quadvector == r.Quadvector;
	public static bool operator !=( in Multivector4<TScalar> l, in Multivector4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Multivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Vector, Bivector, Trivector, Quadvector );
	public override string ToString() => $"[{Scalar:N3}+{Vector}+{Bivector}+{Trivector}+{Quadvector}]";
}
