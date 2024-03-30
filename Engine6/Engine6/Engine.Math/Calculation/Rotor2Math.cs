using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Rotor2{T}"/> are implemented here."/>
/// </summary>
public sealed class Rotor2Math<T> :
		ILinearMath<Rotor2<T>, T>,
		IEntrywiseProduct<Rotor2<T>>,
		IGeometricProduct<Vector2<T>, Vector2<T>, Rotor2<T>>,       // 1*1
		IGeometricProduct<Bivector2<T>, Rotor2<T>, Rotor2<T>>,      // 2*3
		IGeometricProduct<Rotor2<T>, Bivector2<T>, Rotor2<T>>,      // 3*2
		IGeometricProduct<Rotor2<T>, Rotor2<T>, Rotor2<T>>,         // 3*3
		IInterpolable<Rotor2<T>, T>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Negate( in Rotor2<T> l ) => new( -l.Scalar, -l.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Add( in Rotor2<T> l, in Rotor2<T> r ) => new( l.Scalar + r.Scalar, l.Bivector + r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Subtract( in Rotor2<T> l, in Rotor2<T> r ) => new( l.Scalar - r.Scalar, l.Bivector - r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply( in Rotor2<T> l, T r ) => new( l.Scalar * r, l.Bivector * r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Divide( in Rotor2<T> l, T r ) => new( l.Scalar / r, l.Bivector / r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> MultiplyEntrywise( in Rotor2<T> l, in Rotor2<T> r ) => new( l.Scalar * r.Scalar, l.Bivector * r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> DivideEntrywise( in Rotor2<T> l, in Rotor2<T> r ) => new( l.Scalar / r.Scalar, l.Bivector / r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply( in Vector2<T> l, in Vector2<T> r )
		=> new(
			l.X * r.X + l.Y * r.Y,
			l.X * r.Y - l.Y * r.X
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply( in Bivector2<T> l, in Rotor2<T> r )
		=> new(
			-l.XY * r.Bivector.XY,
			l.XY * r.Scalar
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply( in Rotor2<T> l, in Bivector2<T> r )
		=> new(
			-l.Bivector.XY * r.XY,
			l.Scalar * r.XY
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply( in Rotor2<T> l, in Rotor2<T> r )
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY,
			l.Scalar * r.Bivector.XY + l.Bivector.XY * r.Scalar
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Interpolate( in Rotor2<T> l, in Rotor2<T> r, T factor )
		=> l * (T.One - factor) + r * factor;

}
