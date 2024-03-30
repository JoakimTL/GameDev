using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;
using Engine.Math.Operations;

namespace Engine.Math.Calculation;

public sealed class Matrix4x4Math<T> :
		ILinearMath<Matrix4x4<T>, T>,
		IEntrywiseProduct<Matrix4x4<T>>,
		IMatrixMultiplicationProduct<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>,
		IMatrixDeterminant<Matrix4x4<T>, T>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Negate( in Matrix4x4<T> l )
		=> new(
			-l.M00, -l.M01, -l.M02, -l.M03,
			-l.M10, -l.M11, -l.M12, -l.M13,
			-l.M20, -l.M21, -l.M22, -l.M23,
			-l.M30, -l.M31, -l.M32, -l.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Add( in Matrix4x4<T> l, in Matrix4x4<T> r )
		=> new(
			l.M00 + r.M00, l.M01 + r.M01, l.M02 + r.M02, l.M03 + r.M03,
			l.M10 + r.M10, l.M11 + r.M11, l.M12 + r.M12, l.M13 + r.M13,
			l.M20 + r.M20, l.M21 + r.M21, l.M22 + r.M22, l.M23 + r.M23,
			l.M30 + r.M30, l.M31 + r.M31, l.M32 + r.M32, l.M33 + r.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Subtract( in Matrix4x4<T> l, in Matrix4x4<T> r )
		=> new(
			l.M00 - r.M00, l.M01 - r.M01, l.M02 - r.M02, l.M03 - r.M03,
			l.M10 - r.M10, l.M11 - r.M11, l.M12 - r.M12, l.M13 - r.M13,
			l.M20 - r.M20, l.M21 - r.M21, l.M22 - r.M22, l.M23 - r.M23,
			l.M30 - r.M30, l.M31 - r.M31, l.M32 - r.M32, l.M33 - r.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Divide( in Matrix4x4<T> l, T r )
		=> new(
			l.M00 / r, l.M01 / r, l.M02 / r, l.M03 / r,
			l.M10 / r, l.M11 / r, l.M12 / r, l.M13 / r,
			l.M20 / r, l.M21 / r, l.M22 / r, l.M23 / r,
			l.M30 / r, l.M31 / r, l.M32 / r, l.M33 / r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Multiply( in Matrix4x4<T> l, T r )
		=> new(
			l.M00 * r, l.M01 * r, l.M02 * r, l.M03 * r,
			l.M10 * r, l.M11 * r, l.M12 * r, l.M13 * r,
			l.M20 * r, l.M21 * r, l.M22 * r, l.M23 * r,
			l.M30 * r, l.M31 * r, l.M32 * r, l.M33 * r
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> MultiplyEntrywise( in Matrix4x4<T> l, in Matrix4x4<T> r )
		=> new(
			l.M00 * r.M00, l.M01 * r.M01, l.M02 * r.M02, l.M03 * r.M03,
			l.M10 * r.M10, l.M11 * r.M11, l.M12 * r.M12, l.M13 * r.M13,
			l.M20 * r.M20, l.M21 * r.M21, l.M22 * r.M22, l.M23 * r.M23,
			l.M30 * r.M30, l.M31 * r.M31, l.M32 * r.M32, l.M33 * r.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> DivideEntrywise( in Matrix4x4<T> l, in Matrix4x4<T> r )
		=> new(
			l.M00 / r.M00, l.M01 / r.M01, l.M02 / r.M02, l.M03 / r.M03,
			l.M10 / r.M10, l.M11 / r.M11, l.M12 / r.M12, l.M13 / r.M13,
			l.M20 / r.M20, l.M21 / r.M21, l.M22 / r.M22, l.M23 / r.M23,
			l.M30 / r.M30, l.M31 / r.M31, l.M32 / r.M32, l.M33 / r.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Multiply( in Matrix4x4<T> l, in Matrix4x4<T> r )
		=> new(
			l.Row0.Dot( r.Col0 ), l.Row0.Dot( r.Col1 ), l.Row0.Dot( r.Col2 ), l.Row0.Dot( r.Col3 ),
			l.Row1.Dot( r.Col0 ), l.Row1.Dot( r.Col1 ), l.Row1.Dot( r.Col2 ), l.Row1.Dot( r.Col3 ),
			l.Row2.Dot( r.Col0 ), l.Row2.Dot( r.Col1 ), l.Row2.Dot( r.Col2 ), l.Row2.Dot( r.Col3 ),
			l.Row3.Dot( r.Col0 ), l.Row3.Dot( r.Col1 ), l.Row3.Dot( r.Col2 ), l.Row3.Dot( r.Col3 )
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant( in Matrix4x4<T> m )
		=> m.M00 * m.Excluding00.GetDeterminant()
		- m.M01 * m.Excluding01.GetDeterminant()
		+ m.M02 * m.Excluding02.GetDeterminant()
		- m.M03 * m.Excluding03.GetDeterminant();
}