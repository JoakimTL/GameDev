using Engine.Math.Calculation;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Rotor3{T}"/>. Return types may vary.
/// </summary>
public static class Rotor3Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Negate<T>( in this Rotor3<T> l ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Add<T>( in this Rotor3<T> l, in Rotor3<T> r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Subtract<T>( in this Rotor3<T> l, in Rotor3<T> r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> ScalarMultiply<T>( in this Rotor3<T> l, T r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> ScalarDivide<T>( in this Rotor3<T> l, T r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> MultiplyEntrywise<T>( in this Rotor3<T> l, in Rotor3<T> r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> DivideEntrywise<T>( in this Rotor3<T> l, in Rotor3<T> r ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> GetConjugate<T>( in this Rotor3<T> l ) where T : unmanaged, INumber<T>
		=> new( l.Scalar, -l.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Rotor3<T> l, in Vector3<T> r ) where T : unmanaged, INumber<T>
		=> Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Rotor3<T> l, in Multivector3<T> r ) where T : unmanaged, INumber<T>
		=> Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> ToRotationMatrix3x3<T>( in this Rotor3<T> l ) where T : unmanaged, INumber<T>
		=> Matrix3x3Factory.CreateRotationFromRotor( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> ToRotationMatrix4x4<T>( in this Rotor3<T> l ) where T : unmanaged, INumber<T>
		=> Matrix4x4Factory.CreateRotationFromRotor( l );

	/// <summary>
	/// Linearly interpolates between two rotors. The factor is clamped between 0 and 1.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Lerp<T>( in this Rotor3<T> l, in Rotor3<T> r, T factor ) where T : unmanaged, INumber<T>
		=> Rotor3Math<T>.Interpolate( l, r, T.MinMagnitude( T.MaxMagnitude( factor, T.Zero ), T.One ) );

	/// <summary>
	/// Spherically interpolates between two rotors. The factor is clamped between 0 and 1.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Slerp<T>( in this Rotor3<T> l, in Rotor3<T> r, T factor ) where T : unmanaged, IFloatingPointIeee754<T>
		=> Rotor3Math<T>.Interpolate( l, r, (-T.Cos( T.MinMagnitude( T.MaxMagnitude( factor, T.Zero ), T.One ) * T.Pi ) + T.One) / (T.One + T.One) );
}
