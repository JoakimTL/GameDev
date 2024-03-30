using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.Interfaces;
using Engine.Math.Operations;

namespace Engine.Math;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Bivector3<T>( T yz, T zx, T xy ) :
		ILinearOperators<Bivector3<T>, T>,
		IVectorEntrywiseOperators<Bivector3<T>, T>,
		IMultivectorPart<Multivector3<T>>,
		IProductOperator<Bivector3<T>, Vector3<T>, Multivector3<T>>,
		IProductOperator<Bivector3<T>, Trivector3<T>, Vector3<T>>,
		IAdditiveIdentity<Bivector3<T>, Bivector3<T>>,
		IMultiplicativeIdentity<Bivector3<T>, Bivector3<T>>
	where T :
		unmanaged, INumber<T> {

	public readonly T YZ = yz;
	public readonly T ZX = zx;
	public readonly T XY = xy;

	public Multivector3<T> GetMultivector() => new( T.Zero, Vector3<T>.Zero, this, T.Zero );

	public static Bivector3<T> AdditiveIdentity => Zero;
	public static Bivector3<T> MultiplicativeIdentity => One;

	public static readonly Bivector3<T> Zero = new( T.Zero, T.Zero, T.Zero );
	public static readonly Bivector3<T> One = new( T.One, T.One, T.One );

	public static Bivector3<T> operator -( in Bivector3<T> l ) => l.Negate();
	public static Bivector3<T> operator +( in Bivector3<T> l, in Bivector3<T> r ) => l.Add( r );
	public static Bivector3<T> operator -( in Bivector3<T> l, in Bivector3<T> r ) => l.Subtract( r );
	public static Bivector3<T> operator *( in Bivector3<T> l, T r ) => l.ScalarMultiply( r );
	public static Bivector3<T> operator *( T l, in Bivector3<T> r ) => r.ScalarMultiply( l );
	public static Bivector3<T> operator /( in Bivector3<T> l, T r ) => l.ScalarDivide( r );
	public static Bivector3<T> operator *( in Bivector3<T> l, in Bivector3<T> r ) => l.MultiplyEntrywise( r );
	public static Bivector3<T> operator /( in Bivector3<T> l, in Bivector3<T> r ) => l.DivideEntrywise( r );
	public static Multivector3<T> operator *( in Bivector3<T> l, in Vector3<T> r ) => l.Multiply( r );
	public static Vector3<T> operator *( in Bivector3<T> l, in Trivector3<T> r ) => l.Multiply( r );

	public static bool operator ==( in Bivector3<T> l, in Bivector3<T> r ) => l.YZ == r.YZ && l.ZX == r.ZX && l.XY == r.XY;
	public static bool operator !=( in Bivector3<T> l, in Bivector3<T> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Bivector3<T> v && this == v;
	public override int GetHashCode() => HashCode.Combine( YZ, ZX, XY );
	public override string ToString() => $"[{YZ:N3}YZ, {ZX:N3}ZX, {XY:N3}XY]";
}
