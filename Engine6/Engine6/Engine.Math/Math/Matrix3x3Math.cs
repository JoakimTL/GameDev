using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Math;

public sealed class Matrix3x3Math<T> :
		ILinearMath<Matrix3x3<T>, T>,
		IEntrywiseProduct<Matrix3x3<T>>,
		IMatrixMultiplicationProduct<Matrix3x3<T>, Matrix3x3<T>, Matrix3x3<T>>
	where T :
		unmanaged, INumberBase<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Negate( in Matrix3x3<T> l )
		=> new(
			-l.M00, -l.M01, -l.M02,
			-l.M10, -l.M11, -l.M12,
			-l.M20, -l.M21, -l.M22
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Add( in Matrix3x3<T> l, in Matrix3x3<T> r )
		=> new(
			l.M00 + r.M00, l.M01 + r.M01, l.M02 + r.M02,
			l.M10 + r.M10, l.M11 + r.M11, l.M12 + r.M12,
			l.M20 + r.M20, l.M21 + r.M21, l.M22 + r.M22
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Subtract( in Matrix3x3<T> l, in Matrix3x3<T> r )
		=> new(
			l.M00 - r.M00, l.M01 - r.M01, l.M02 - r.M02,
			l.M10 - r.M10, l.M11 - r.M11, l.M12 - r.M12,
			l.M20 - r.M20, l.M21 - r.M21, l.M22 - r.M22
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Divide( in Matrix3x3<T> l, T r )
		=> new(
			l.M00 / r, l.M01 / r, l.M02 / r,
			l.M10 / r, l.M11 / r, l.M12 / r,
			l.M20 / r, l.M21 / r, l.M22 / r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Multiply( in Matrix3x3<T> l, T r )
		=> new(
			l.M00 * r, l.M01 * r, l.M02 * r,
			l.M10 * r, l.M11 * r, l.M12 * r,
			l.M20 * r, l.M21 * r, l.M22 * r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> MultiplyEntrywise( in Matrix3x3<T> l, in Matrix3x3<T> r )
		=> new(
			l.M00 * r.M00, l.M01 * r.M01, l.M02 * r.M02,
			l.M10 * r.M10, l.M11 * r.M11, l.M12 * r.M12,
			l.M20 * r.M20, l.M21 * r.M21, l.M22 * r.M22
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> DivideEntrywise( in Matrix3x3<T> l, in Matrix3x3<T> r )
		=> new(
			l.M00 / r.M00, l.M01 / r.M01, l.M02 / r.M02,
			l.M10 / r.M10, l.M11 / r.M11, l.M12 / r.M12,
			l.M20 / r.M20, l.M21 / r.M21, l.M22 / r.M22
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix3x3<T> Multiply( in Matrix3x3<T> l, in Matrix3x3<T> r )
		=> new(
			l.Row0.Dot( r.Col0 ), l.Row0.Dot( r.Col1 ), l.Row0.Dot( r.Col2 ),
			l.Row1.Dot( r.Col0 ), l.Row1.Dot( r.Col1 ), l.Row1.Dot( r.Col2 ),
			l.Row2.Dot( r.Col0 ), l.Row2.Dot( r.Col1 ), l.Row2.Dot( r.Col2 )
		);
}
