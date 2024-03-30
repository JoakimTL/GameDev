using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Bivector4{T}"/> are implemented here."/>
/// </summary>
public sealed class Bivector4Math<T> :
		ILinearMath<Bivector4<T>, T>,
		IEntrywiseProduct<Bivector4<T>>,
		IOuterProduct<Vector4<T>, Vector4<T>, Bivector4<T>>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Negate( in Bivector4<T> l ) => new( -l.YZ, -l.ZX, -l.XY, -l.YW, -l.ZW, -l.XW );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Add( in Bivector4<T> l, in Bivector4<T> r ) => new( l.YZ + r.YZ, l.ZX + r.ZX, l.XY + r.XY, l.YW + r.YW, l.ZW + r.ZW, l.XW + r.XW );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Subtract( in Bivector4<T> l, in Bivector4<T> r ) => new( l.YZ - r.YZ, l.ZX - r.ZX, l.XY - r.XY, l.YW - r.YW, l.ZW - r.ZW, l.XW - r.XW );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Multiply( in Bivector4<T> l, T r ) => new( l.YZ * r, l.ZX * r, l.XY * r, l.YW * r, l.ZW * r, l.XW * r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Divide( in Bivector4<T> l, T r ) => new( l.YZ / r, l.ZX / r, l.XY / r, l.YW / r, l.ZW / r, l.XW / r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> MultiplyEntrywise( in Bivector4<T> l, in Bivector4<T> r ) => new( l.YZ * r.YZ, l.ZX * r.ZX, l.XY * r.XY, l.YW * r.YW, l.ZW * r.ZW, l.XW * r.XW );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> DivideEntrywise( in Bivector4<T> l, in Bivector4<T> r ) => new( l.YZ / r.YZ, l.ZX / r.ZX, l.XY / r.XY, l.YW / r.YW, l.ZW / r.ZW, l.XW / r.XW );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Wedge( in Vector4<T> l, in Vector4<T> r ) => new( l.Y * r.Z - l.Z * r.Y, l.Z * r.X - l.X * r.Z, l.X * r.Y - l.Y * r.X, l.Y * r.W - l.W * r.Y, l.Z * r.W - l.W * r.Z, l.X * r.W - l.W * r.X );
}