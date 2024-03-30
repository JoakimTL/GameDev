using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Vector2{T}"/> are implemented here."/>
/// </summary>
public sealed class Multivector2Math<T> :
		ILinearMath<Multivector2<T>, T>,
		IEntrywiseProduct<Multivector2<T>>,
		IGeometricProduct<Vector2<T>, Multivector2<T>, Multivector2<T>>,
		IGeometricProduct<Bivector2<T>, Multivector2<T>, Multivector2<T>>,
		IGeometricProduct<Rotor2<T>, Multivector2<T>, Multivector2<T>>,
		IGeometricProduct<Multivector2<T>, Vector2<T>, Multivector2<T>>,
		IGeometricProduct<Multivector2<T>, Bivector2<T>, Multivector2<T>>,
		IGeometricProduct<Multivector2<T>, Rotor2<T>, Multivector2<T>>,
		IGeometricProduct<Multivector2<T>, Multivector2<T>, Multivector2<T>>
	where T :
		unmanaged, INumber<T> {

	#region Linear Algebra

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Negate( in Multivector2<T> l )
		=> new(
			-l.Scalar,
			-l.Vector,
			-l.Bivector
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Add( in Multivector2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar + r.Scalar,
			l.Vector + r.Vector,
			l.Bivector + r.Bivector
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Subtract( in Multivector2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar - r.Scalar,
			l.Vector - r.Vector,
			l.Bivector - r.Bivector
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Multivector2<T> l, T r )
		=> new(
			l.Scalar * r,
			l.Vector * r,
			l.Bivector * r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Divide( in Multivector2<T> l, T r )
		=> new(
			l.Scalar / r,
			l.Vector / r,
			l.Bivector / r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> MultiplyEntrywise( in Multivector2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar * r.Scalar,
			l.Vector * r.Vector,
			l.Bivector * r.Bivector
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> DivideEntrywise( in Multivector2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar / r.Scalar,
			l.Vector / r.Vector,
			l.Bivector / r.Bivector
		);

	#endregion

	#region Geometric Products

	/*
	 * 0 - scalar
	 * 1 - vec2
	 * 2 - bivec2
	 * 3 - rotor2
	 * 4 - multivec2
	 * 
	 ---------------------* 1 * 1, // returns 3
	 * 1 * 2, // returns 1
	 * 1 * 3, // returns 1
	 * 1 * 4, // returns 4
	 * 2 * 1, // returns 4
	 ---------------------* 2 * 2, // returns 0
	 ---------------------* 2 * 3, // returns 3
	 * 2 * 4, // returns 4
	 * 3 * 1, // returns 1
	 ---------------------* 3 * 2, // returns 3
	 ---------------------* 3 * 3, // returns 3
	 * 3 * 4, // returns 4
	 * 4 * 1, // returns 4
	 * 4 * 2, // returns 4
	 * 4 * 3, // returns 4
	 * 4 * 4, // returns 4
	 */
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Vector2<T> l, in Multivector2<T> r )
		=> new(
			l.X * r.Vector.X + l.Y * r.Vector.Y,
			l.X * r.Scalar - l.Y * r.Bivector.XY,
			l.X * r.Bivector.XY + l.Y * r.Scalar,
			l.X * r.Vector.Y - l.Y * r.Vector.X
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Bivector2<T> l, in Multivector2<T> r )
		=> new(
			 -l.XY * r.Bivector.XY,
			 l.XY * r.Vector.Y,
			 -l.XY * r.Vector.X,
			 l.XY * r.Scalar
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Rotor2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY,
			l.Scalar * r.Vector.X + l.Bivector.XY * r.Vector.Y,
			l.Scalar * r.Vector.Y - l.Bivector.XY * r.Vector.X,
			l.Scalar * r.Bivector.XY + l.Bivector.XY * r.Scalar
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Multivector2<T> l, in Vector2<T> r )
		=> new(
			l.Vector.X * r.X + l.Vector.Y * r.Y,
			l.Scalar * r.X + l.Bivector.XY * r.Y,
			l.Scalar * r.Y - l.Bivector.XY * r.X,
			l.Vector.X * r.Y - l.Vector.Y * r.X
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Multivector2<T> l, in Bivector2<T> r )
		=> new(
			-l.Bivector.XY * r.XY,
			-l.Vector.Y * r.XY,
			l.Vector.X * r.XY,
			l.Scalar * r.XY
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Multivector2<T> l, in Rotor2<T> r )
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY,
			l.Vector.X * r.Scalar - l.Vector.Y * r.Bivector.XY,
			l.Vector.Y * r.Scalar + l.Vector.X * r.Bivector.XY,
			l.Scalar * r.Bivector.XY + l.Bivector.XY * r.Scalar
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply( in Multivector2<T> l, in Multivector2<T> r )
		=> new(
			l.Scalar * r.Scalar + l.Vector.X * r.Vector.X + l.Vector.Y * r.Vector.Y - l.Bivector.XY * r.Bivector.XY,
			l.Scalar * r.Vector.X + l.Vector.X * r.Scalar + l.Bivector.XY * r.Vector.Y - l.Vector.Y * r.Bivector.XY,
			l.Scalar * r.Vector.Y + l.Vector.Y * r.Scalar + l.Vector.X * r.Bivector.XY - l.Bivector.XY * r.Vector.X,
			l.Scalar * r.Bivector.XY + l.Bivector.XY * r.Scalar + l.Vector.X * r.Vector.Y - l.Vector.Y * r.Vector.X
		);

	#endregion
}
