using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewVectors.Interfaces;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Multivector2<TScalar>( TScalar scalar, Vector2<TScalar> vector, Bivector2<TScalar> bivector ) :
		IVector<Multivector2<TScalar>, TScalar>,
		IMultivectorPart<Multivector2<TScalar>, Multivector2<TScalar>>,
		ILinearAlgebraOperators<Multivector2<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TScalar Scalar = scalar;
	public readonly Vector2<TScalar> Vector = vector;
	public readonly Bivector2<TScalar> Bivector = bivector;

	public Multivector2( TScalar scalar, TScalar x, TScalar y, TScalar xy )
		: this( scalar, new( x, y ), new( xy ) ) { }

	public static Multivector2<TScalar> AdditiveIdentity => Zero;
	public static Multivector2<TScalar> MultiplicativeIdentity => One;
	public static Multivector2<TScalar> Zero { get; } = new( TScalar.Zero, Vector2<TScalar>.Zero, Bivector2<TScalar>.Zero );
	public static Multivector2<TScalar> One { get; } = new( TScalar.One, Vector2<TScalar>.One, Bivector2<TScalar>.One );

	public static Multivector2<TScalar> GetMultivector( in Multivector2<TScalar> part ) => part;

	public static Multivector2<TScalar> Negate( in Multivector2<TScalar> l ) => new( -l.Scalar, -l.Vector, -l.Bivector );
	public static Multivector2<TScalar> Add( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => new( l.Scalar + r.Scalar, l.Vector + r.Vector, l.Bivector + r.Bivector );
	public static Multivector2<TScalar> Subtract( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => new( l.Scalar - r.Scalar, l.Vector - r.Vector, l.Bivector - r.Bivector );
	public static Multivector2<TScalar> ScalarMultiply( in Multivector2<TScalar> l, TScalar r ) => new( l.Scalar * r, l.Vector * r, l.Bivector * r );
	public static Multivector2<TScalar> ScalarDivide( in Multivector2<TScalar> l, TScalar r ) => new( l.Scalar / r, l.Vector / r, l.Bivector / r );
	public static Multivector2<TScalar> DivideScalar( TScalar l, in Multivector2<TScalar> r ) => new( l / r.Scalar, l / r.Vector, l / r.Bivector );
	public static TScalar Dot( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => (l.Scalar * r.Scalar) + Vector2<TScalar>.Dot( l.Vector, r.Vector ) + Bivector2<TScalar>.Dot( l.Bivector, r.Bivector );

	public static Multivector2<TScalar> operator -( in Multivector2<TScalar> l ) => Negate( l );
	public static Multivector2<TScalar> operator +( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => Add( l, r );
	public static Multivector2<TScalar> operator -( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => Subtract( l, r );
	public static Multivector2<TScalar> operator *( in Multivector2<TScalar> l, TScalar r ) => ScalarMultiply( l, r );
	public static Multivector2<TScalar> operator *( TScalar l, in Multivector2<TScalar> r ) => ScalarMultiply( r, l );
	public static Multivector2<TScalar> operator /( in Multivector2<TScalar> l, TScalar r ) => ScalarDivide( l, r );
	public static Multivector2<TScalar> operator /( TScalar l, in Multivector2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector;
	public static bool operator !=( in Multivector2<TScalar> l, in Multivector2<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Multivector2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Scalar, Vector, Bivector );
	public override string ToString() => $"[{Scalar:N3}+{Vector}+{Bivector}]";

}
