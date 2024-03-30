using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Rotor3{T}"/> are implemented here."/>
/// </summary>
public sealed class Rotor3Math<T> :
		ILinearMath<Rotor3<T>, T>,
		IEntrywiseProduct<Rotor3<T>>,
		IGeometricProduct<Vector3<T>, Vector3<T>, Rotor3<T>>,       // 1*1
		IGeometricProduct<Bivector3<T>, Bivector3<T>, Rotor3<T>>,   // 2*2
		IGeometricProduct<Bivector3<T>, Rotor3<T>, Rotor3<T>>,      // 2*4
		IGeometricProduct<Rotor3<T>, Bivector3<T>, Rotor3<T>>,      // 4*2
		IGeometricProduct<Rotor3<T>, Rotor3<T>, Rotor3<T>>,         // 4*4
		IInterpolable<Rotor3<T>, T>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Negate( in Rotor3<T> l ) => new( -l.Scalar, -l.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Add( in Rotor3<T> l, in Rotor3<T> r ) => new( l.Scalar + r.Scalar, l.Bivector + r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Subtract( in Rotor3<T> l, in Rotor3<T> r ) => new( l.Scalar - r.Scalar, l.Bivector - r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Rotor3<T> l, T r ) => new( l.Scalar * r, l.Bivector * r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Divide( in Rotor3<T> l, T r ) => new( l.Scalar / r, l.Bivector / r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> MultiplyEntrywise( in Rotor3<T> l, in Rotor3<T> r ) => new( l.Scalar * r.Scalar, l.Bivector * r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> DivideEntrywise( in Rotor3<T> l, in Rotor3<T> r ) => new( l.Scalar / r.Scalar, l.Bivector / r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Vector3<T> l, in Vector3<T> r )
		=> new(
			l.X * r.X + l.Y * r.Y + l.Z * r.Z,
			new(
				l.Y * r.Z - l.Z * r.Y,
				l.Z * r.X - l.X * r.Z,
				l.X * r.Y - l.Y * r.X
			)
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Bivector3<T> l, in Bivector3<T> r )
		=> new(
			-l.XY * r.XY - l.YZ * r.YZ - l.ZX * r.ZX,
			new(
				l.XY * r.ZX - l.ZX * r.XY,
				l.YZ * r.XY - l.XY * r.YZ,
				l.ZX * r.YZ - l.YZ * r.ZX
			)
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Bivector3<T> l, in Rotor3<T> r )
		=> new(
			-l.XY * r.Bivector.XY - l.YZ * r.Bivector.YZ - l.ZX * r.Bivector.ZX,
			new(
				l.XY * r.Bivector.ZX + l.YZ * r.Scalar - l.ZX * r.Bivector.XY,
				l.YZ * r.Bivector.XY + l.ZX * r.Scalar - l.XY * r.Bivector.YZ,
				l.ZX * r.Bivector.YZ + l.XY * r.Scalar - l.YZ * r.Bivector.ZX
			)
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Rotor3<T> l, in Bivector3<T> r )
		=> new(
			-(l.Bivector.XY * r.XY) - l.Bivector.YZ * r.YZ - l.Bivector.ZX * r.ZX,
			new(
				l.Scalar * r.YZ + l.Bivector.XY * r.ZX - l.Bivector.ZX * r.XY,
				l.Scalar * r.ZX + l.Bivector.YZ * r.XY - l.Bivector.XY * r.YZ,
				l.Scalar * r.XY + l.Bivector.ZX * r.YZ - l.Bivector.YZ * r.ZX
			)
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply( in Rotor3<T> l, in Rotor3<T> r )
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY - l.Bivector.YZ * r.Bivector.YZ - l.Bivector.ZX * r.Bivector.ZX,
			new(
				l.Scalar * r.Bivector.YZ + l.Bivector.XY * r.Bivector.ZX + l.Bivector.YZ * r.Scalar - l.Bivector.ZX * r.Bivector.XY,
				l.Scalar * r.Bivector.ZX + l.Bivector.YZ * r.Bivector.XY + l.Bivector.ZX * r.Scalar - l.Bivector.XY * r.Bivector.YZ,
				l.Scalar * r.Bivector.XY + l.Bivector.ZX * r.Bivector.YZ + l.Bivector.XY * r.Scalar - l.Bivector.YZ * r.Bivector.ZX
			)
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Interpolate( in Rotor3<T> l, in Rotor3<T> r, T factor )
		=> l * (T.One - factor) + r * factor;
}
