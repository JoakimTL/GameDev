using Engine.Math.Calculation;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Rotor3{T}"/>. Return types may vary.
/// </summary>
public static class Rotor2Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Negate<T>( in this Rotor2<T> l ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Add<T>( in this Rotor2<T> l, in Rotor2<T> r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Subtract<T>( in this Rotor2<T> l, in Rotor2<T> r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> ScalarMultiply<T>( in this Rotor2<T> l, T r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> ScalarDivide<T>( in this Rotor2<T> l, T r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> MultiplyEntrywise<T>( in this Rotor2<T> l, in Rotor2<T> r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> DivideEntrywise<T>( in this Rotor2<T> l, in Rotor2<T> r ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> GetConjugate<T>( in this Rotor2<T> l ) where T : unmanaged, INumber<T>
		=> new( l.Scalar, -l.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Multiply<T>( in this Rotor2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T>
		=> Vector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Rotor2<T> l, in Multivector2<T> r ) where T : unmanaged, INumber<T>
		=> Multivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> ToRotationMatrix<T>( in this Rotor2<T> l ) where T : unmanaged, INumber<T>
		=> Matrix2x2Factory.CreateRotationFromRotor( l );

	/// <summary>
	/// Linearly interpolates between two rotors. The factor is clamped between 0 and 1.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Lerp<T>( in this Rotor2<T> l, in Rotor2<T> r, T factor ) where T : unmanaged, INumber<T>
		=> Rotor2Math<T>.Interpolate( l, r, T.MinMagnitude( T.MaxMagnitude( factor, T.Zero ), T.One ) );

	/// <summary>
	/// Spherically interpolates between two rotors. The factor is clamped between 0 and 1.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Slerp<T>( in this Rotor2<T> l, in Rotor2<T> r, T factor ) where T : unmanaged, IFloatingPointIeee754<T>
		=> Rotor2Math<T>.Interpolate( l, r, (-T.Cos( T.MinMagnitude( T.MaxMagnitude( factor, T.Zero ), T.One ) * T.Pi ) + T.One) / (T.One + T.One) );
}