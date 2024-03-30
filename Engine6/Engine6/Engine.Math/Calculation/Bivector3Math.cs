using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Bivector3{T}"/> are implemented here."/>
/// </summary>
public sealed class Bivector3Math<T> :
		ILinearMath<Bivector3<T>, T>,
		IEntrywiseProduct<Bivector3<T>>,
		IGeometricProduct<Vector3<T>, Trivector3<T>, Bivector3<T>>,
		IGeometricProduct<Trivector3<T>, Vector3<T>, Bivector3<T>>,
		IOuterProduct<Vector3<T>, Vector3<T>, Bivector3<T>>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Negate( in Bivector3<T> l ) => new( -l.YZ, -l.ZX, -l.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Add( in Bivector3<T> l, in Bivector3<T> r ) => new( l.YZ + r.YZ, l.ZX + r.ZX, l.XY + r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Subtract( in Bivector3<T> l, in Bivector3<T> r ) => new( l.YZ - r.YZ, l.ZX - r.ZX, l.XY - r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Multiply( in Bivector3<T> l, T r ) => new( l.YZ * r, l.ZX * r, l.XY * r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Divide( in Bivector3<T> l, T r ) => new( l.YZ / r, l.ZX / r, l.XY / r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> MultiplyEntrywise( in Bivector3<T> l, in Bivector3<T> r ) => new( l.YZ * r.YZ, l.ZX * r.ZX, l.XY * r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> DivideEntrywise( in Bivector3<T> l, in Bivector3<T> r ) => new( l.YZ / r.YZ, l.ZX / r.ZX, l.XY / r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Multiply( in Vector3<T> l, in Trivector3<T> r ) => new( l.X * r.XYZ, l.Y * r.XYZ, l.Z * r.XYZ );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Multiply( in Trivector3<T> l, in Vector3<T> r ) => new( l.XYZ * r.X, l.XYZ * r.Y, l.XYZ * r.Z );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Wedge( in Vector3<T> l, in Vector3<T> r ) => new( l.Y * r.Z - l.Z * r.Y, l.Z * r.X - l.X * r.Z, l.X * r.Y - l.Y * r.X );
}
