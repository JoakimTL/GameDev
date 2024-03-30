using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Bivector3{T}"/>. Return types may vary.
/// </summary>
public static class Bivector3Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Negate<T>( in this Bivector3<T> l ) where T : unmanaged, INumber<T> => Bivector3Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Add<T>( in this Bivector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Subtract<T>( in this Bivector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> ScalarMultiply<T>( in this Bivector3<T> l, T r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> ScalarDivide<T>( in this Bivector3<T> l, T r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> MultiplyEntrywise<T>( in this Bivector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> DivideEntrywise<T>( in this Bivector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumber<T> => Bivector3Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Bivector3<T> l, in Vector3<T> r ) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor3<T> Multiply<T>( in this Bivector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumber<T> => Rotor3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Multiply<T>( in this Bivector3<T> l, in Trivector3<T> r ) where T : unmanaged, INumber<T> => Vector3Math<T>.Multiply( l, r );

}
