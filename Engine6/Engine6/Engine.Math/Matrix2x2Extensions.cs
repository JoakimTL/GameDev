using Engine.Math.Operations;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math;

public static class Matrix2x2Extensions {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Negate<T>( in this Matrix2x2<T> l ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Add<T>( in this Matrix2x2<T> l, in Matrix2x2<T> r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Subtract<T>( in this Matrix2x2<T> l, in Matrix2x2<T> r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> ScalarMultiply<T>( in this Matrix2x2<T> l, T r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.ScalarMultiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> ScalarDivide<T>( in this Matrix2x2<T> l, T r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.ScalarDivide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> MultiplyEntrywise<T>( in this Matrix2x2<T> l, in Matrix2x2<T> r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> DivideEntrywise<T>( in this Matrix2x2<T> l, in Matrix2x2<T> r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Multiply<T>( in this Matrix2x2<T> l, in Matrix2x2<T> r ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant<T>( in this Matrix2x2<T> m ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.GetDeterminant( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> GetTransposed<T>( in this Matrix2x2<T> m ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.GetTransposed( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> GetInverse<T>( in this Matrix2x2<T> m ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.GetInverse( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Expand<T>( in this Matrix2x2<T> m, out Matrix3x3<T> result ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Expand( m, out result );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Expand<T>( in this Matrix2x2<T> m, out Matrix4x4<T> result ) where T : unmanaged, INumber<T> => Matrix2x2Ops<T>.Expand( m, out result );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool TryFillRowMajor<T, TData>( in this Matrix2x2<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix2x2Ops<T>.TryFillRowMajor( m, resultStorage, destinationOffsetBytes );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool TryFillColumnMajor<T, TData>( in this Matrix2x2<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix2x2Ops<T>.TryFillColumnMajor( m, resultStorage, destinationOffsetBytes );
}

public static class Matrix3x3Extensions {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Negate<T>( in this Matrix3x3<T> l ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Add<T>( in this Matrix3x3<T> l, in Matrix3x3<T> r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Subtract<T>( in this Matrix3x3<T> l, in Matrix3x3<T> r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> ScalarMultiply<T>( in this Matrix3x3<T> l, T r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.ScalarMultiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> ScalarDivide<T>( in this Matrix3x3<T> l, T r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.ScalarDivide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> MultiplyEntrywise<T>( in this Matrix3x3<T> l, in Matrix3x3<T> r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> DivideEntrywise<T>( in this Matrix3x3<T> l, in Matrix3x3<T> r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Multiply<T>( in this Matrix3x3<T> l, in Matrix3x3<T> r ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant<T>( in this Matrix3x3<T> m ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.GetDeterminant( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> GetTransposed<T>( in this Matrix3x3<T> m ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.GetTransposed( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> GetInverse<T>( in this Matrix3x3<T> m ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.GetInverse( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Expand<T>( in this Matrix3x3<T> m, out Matrix4x4<T> result ) where T : unmanaged, INumber<T> => Matrix3x3Ops<T>.Expand( m, out result );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool TryFillRowMajor<T, TData>( in this Matrix3x3<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix3x3Ops<T>.TryFillRowMajor( m, resultStorage, destinationOffsetBytes );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool TryFillColumnMajor<T, TData>( in this Matrix3x3<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix3x3Ops<T>.TryFillColumnMajor( m, resultStorage, destinationOffsetBytes );
}

public static class Matrix4x4Extensions {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Negate<T>( in this Matrix4x4<T> l ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Add<T>( in this Matrix4x4<T> l, in Matrix4x4<T> r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Subtract<T>( in this Matrix4x4<T> l, in Matrix4x4<T> r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> ScalarMultiply<T>( in this Matrix4x4<T> l, T r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.ScalarMultiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> ScalarDivide<T>( in this Matrix4x4<T> l, T r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.ScalarDivide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> MultiplyEntrywise<T>( in this Matrix4x4<T> l, in Matrix4x4<T> r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> DivideEntrywise<T>( in this Matrix4x4<T> l, in Matrix4x4<T> r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Multiply<T>( in this Matrix4x4<T> l, in Matrix4x4<T> r ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant<T>( in this Matrix4x4<T> m ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.GetDeterminant( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> GetTransposed<T>( in this Matrix4x4<T> m ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.GetTransposed( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> GetInverse<T>( in this Matrix4x4<T> m ) where T : unmanaged, INumber<T> => Matrix4x4Ops<T>.GetInverse( m );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillRowMajor<T, TData>( in this Matrix4x4<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix4x4Ops<T>.TryFillRowMajor( m, resultStorage, destinationOffsetBytes );

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillColumnMajor<T, TData>( in this Matrix4x4<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where T : unmanaged, INumber<T> where TData : unmanaged
		=> Matrix4x4Ops<T>.TryFillColumnMajor( m, resultStorage, destinationOffsetBytes );
}