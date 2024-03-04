using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Old;

namespace Engine.Math.Old.InternalMath;

public static class MathMatrix2x2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T GetDeterminant<T>(in Matrix2x2<T> l) where T : INumberBase<T>
        => l.M00 * l.M11 - l.M01 * l.M10;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix2x2<T> Multiply<T>(in Vector2Column<T> l, in Vector2<T> r) where T : INumberBase<T>
        => new(
            l.X * r.X, l.X * r.Y,
            l.Y * r.X, l.Y * r.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> Multiply<T>(in Vector2<T> l, in Matrix2x2<T> r) where T : INumberBase<T>
        => new(
            l.X * r.M00 + l.Y * r.M10,
            l.X * r.M01 + l.Y * r.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix2x2<T> Multiply<T>(in Matrix2x2<T> l, in Matrix2x2<T> r) where T : INumberBase<T>
        => new(
            l.M00 * r.M00 + l.M01 * r.M10,
            l.M00 * r.M01 + l.M01 * r.M11,
            l.M10 * r.M00 + l.M11 * r.M10,
            l.M10 * r.M01 + l.M11 * r.M11
        );
}

public static class MathMatrix3x3
{
    //[MethodImpl( MethodImplOptions.AggressiveInlining )]
    //internal static T GetDeterminant<T>( in Matrix3x3<T> l ) where T : INumberBase<T>
    //	=> (l.M11 * l.M22 - l.M12 * l.M21) * l.M00
    //	 - (l.M01 * l.M22 - l.M02 * l.M21) * l.M10
    //	 + (l.M01 * l.M12 - l.M02 * l.M11) * l.M20;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T GetDeterminantByExpansionOfMinors<T>(in Matrix3x3<T> l) where T : INumberBase<T>
        => l.Excluding00.Determinant * l.M00 - l.Excluding10.Determinant * l.M10 + l.Excluding20.Determinant * l.M20;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> Multiply<T>(in Vector3<T> l, in Matrix3x3<T> r) where T : INumberBase<T>
        => new(
            l.X * r.M00 + l.Y * r.M10 + l.Z * r.M20,
            l.X * r.M01 + l.Y * r.M11 + l.Z * r.M21,
            l.X * r.M02 + l.Y * r.M12 + l.Z * r.M22
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix3x3<T> Multiply<T>(in Vector3Column<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(
            l.X * r.X, l.X * r.Y, l.X * r.Z,
            l.Y * r.X, l.Y * r.Y, l.Y * r.Z,
            l.Z * r.X, l.Z * r.Y, l.Z * r.Z
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix3x3<T> Multiply<T>(in Matrix3x3<T> l, in Matrix3x3<T> r) where T : INumberBase<T>
        => new(
            l.M00 * r.M00 + l.M01 * r.M10 + l.M02 * r.M20,
            l.M00 * r.M01 + l.M01 * r.M11 + l.M02 * r.M21,
            l.M00 * r.M02 + l.M01 * r.M12 + l.M02 * r.M22,
            l.M10 * r.M00 + l.M11 * r.M10 + l.M12 * r.M20,
            l.M10 * r.M01 + l.M11 * r.M11 + l.M12 * r.M21,
            l.M10 * r.M02 + l.M11 * r.M12 + l.M12 * r.M22,
            l.M20 * r.M00 + l.M21 * r.M10 + l.M22 * r.M20,
            l.M20 * r.M01 + l.M21 * r.M11 + l.M22 * r.M21,
            l.M20 * r.M02 + l.M21 * r.M12 + l.M22 * r.M22
        );
}

public static class MathMatrix4x4
{

    //[MethodImpl( MethodImplOptions.AggressiveInlining )]
    //internal static T GetDeterminant<T>( Matrix4x4<T> l ) where T : INumberBase<T> {
    //	var l22l33 = l.M22 * l.M33;
    //	var l23l32 = l.M23 * l.M32;
    //	var l12l33 = l.M12 * l.M33;
    //	var l13l32 = l.M13 * l.M32;
    //	var l12l23 = l.M12 * l.M23;
    //	var l13l22 = l.M13 * l.M22;
    //	var l02l33 = l.M02 * l.M33;
    //	var l03l32 = l.M03 * l.M32;
    //	var l02l23 = l.M02 * l.M23;
    //	var l03l22 = l.M03 * l.M22;
    //	var l02l13 = l.M02 * l.M13;
    //	var l03l12 = l.M03 * l.M12;

    //	var l22l33_l23l32 = l22l33 - l23l32;
    //	var l12l33_l13l32 = l12l33 - l13l32;
    //	var l12l23_l13l22 = l12l23 - l13l22;
    //	var l02l33_l03l32 = l02l33 - l03l32;
    //	var l02l23_l03l22 = l02l23 - l03l22;
    //	var l02l13_l03l12 = l02l13 - l03l12;

    //	return
    //		  (l22l33_l23l32 * l.M11 - l12l33_l13l32 * l.M21 + l12l23_l13l22 * l.M31) * l.M00
    //		- (l22l33_l23l32 * l.M01 - l02l33_l03l32 * l.M21 + l02l23_l03l22 * l.M31) * l.M10
    //		+ (l12l33_l13l32 * l.M01 - l02l33_l03l32 * l.M11 + l02l13_l03l12 * l.M31) * l.M20
    //		- (l12l23_l13l22 * l.M01 - l02l23_l03l22 * l.M11 + l02l13_l03l12 * l.M21) * l.M30;

    //	/*
    //		(
    //		   (l.M22 * l.M33 - l.M23 * l.M32) * l.M11
    //		 - (l.M12 * l.M33 - l.M13 * l.M32) * l.M21
    //		 + (l.M12 * l.M23 - l.M13 * l.M22) * l.M31
    //		) * l.M00 - (
    //		   (l.M22 * l.M33 - l.M23 * l.M32) * l.M01
    //		 - (l.M02 * l.M33 - l.M03 * l.M32) * l.M21
    //		 + (l.M02 * l.M23 - l.M03 * l.M22) * l.M31
    //		) * l.M10 + (
    //		   (l.M12 * l.M33 - l.M13 * l.M32) * l.M01
    //		 - (l.M02 * l.M33 - l.M03 * l.M32) * l.M11
    //		 + (l.M02 * l.M13 - l.M03 * l.M12) * l.M31
    //		) * l.M20 - (
    //		   (l.M12 * l.M23 - l.M13 * l.M22) * l.M01
    //		 - (l.M02 * l.M23 - l.M03 * l.M22) * l.M11
    //		 + (l.M02 * l.M13 - l.M03 * l.M12) * l.M21
    //		) * l.M30;
    //	 */
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T GetDeterminantByExpansionOfMinors<T>(Matrix4x4<T> l) where T : INumberBase<T>
        => l.Excluding00.GetDeterminantByExpansionOfMinors() * l.M00
         - l.Excluding10.GetDeterminantByExpansionOfMinors() * l.M10
         + l.Excluding20.GetDeterminantByExpansionOfMinors() * l.M20
         - l.Excluding30.GetDeterminantByExpansionOfMinors() * l.M30;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryGetDeterminantByGaussianElimination<T>(Matrix4x4<T> l, out T determinant) where T : INumberBase<T>
    {
        determinant = T.AdditiveIdentity;
        if (TryGetUpperTriangular(l, out Matrix4x4<T> result, out bool negative))
        {
            determinant = result.M00 * result.M11 * result.M22 * result.M33 * (negative ? -T.MultiplicativeIdentity : T.MultiplicativeIdentity);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryGetUpperTriangular<T>(Matrix4x4<T> l, out Matrix4x4<T> result, out bool negative) where T : INumberBase<T>
    {
        //https://www.youtube.com/watch?v=htYh-Tq7ZBI Add bivectors! Actually watch it again and add a lot of what is mentioned. Like 2d cross product, and change names of operations to correct ignorance.
        result = l;
        Vector4<T> row0 = l.Row0;
        Vector4<T> row1 = l.Row1;
        Vector4<T> row2 = l.Row2;
        Vector4<T> row3 = l.Row3;

        negative = false;

        if (row0.X == T.AdditiveIdentity)
        {
            Vector4<T> temp = row0;
            negative = !negative;
            if (row1.X != T.AdditiveIdentity)
            {
                row0 = row1;
                row1 = temp;
            }
            else if (row2.X != T.AdditiveIdentity)
            {
                row0 = row2;
                row2 = temp;
            }
            else if (row3.X != T.AdditiveIdentity)
            {
                row0 = row3;
                row3 = temp;
            }
            else
            {
                return false;
            }
        }

        row1 -= row0 * (row1.X / row0.X);
        row2 -= row0 * (row2.X / row0.X);
        row3 -= row0 * (row3.X / row0.X);

        if (row1.Y == T.AdditiveIdentity)
        {
            Vector4<T> temp = row1;
            negative = !negative;
            if (row2.Y != T.AdditiveIdentity)
            {
                row1 = row2;
                row2 = temp;
            }
            else if (row3.Y != T.AdditiveIdentity)
            {
                row1 = row3;
                row3 = temp;
            }
            else
            {
                return false;
            }
        }

        row2 -= row1 * (row2.Y / row1.Y);
        row3 -= row1 * (row3.Y / row1.Y);

        if (row2.Z == T.AdditiveIdentity)
        {
            Vector4<T> temp = row2;
            negative = !negative;
            if (row3.Z != T.AdditiveIdentity)
            {
                row2 = row3;
                row3 = temp;
            }
            else
            {
                return false;
            }
        }

        row3 -= row2 * (row3.Z / row2.Z);

        result = new Matrix4x4<T>(row0, row1, row2, row3);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Multiply<T>(in Vector4Column<T> l, in Vector4<T> r) where T : INumberBase<T>
        => new(
            l.X * r.X, l.X * r.Y, l.X * r.Z, l.X * r.W,
            l.Y * r.X, l.Y * r.Y, l.Y * r.Z, l.Y * r.W,
            l.Z * r.X, l.Z * r.Y, l.Z * r.Z, l.Z * r.W,
            l.W * r.X, l.W * r.Y, l.W * r.Z, l.W * r.W
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Add<T>(Matrix4x4<T> l, Matrix4x4<T> r) where T : INumberBase<T>
        => new(
            l.M00 + r.M00, l.M01 + r.M01, l.M02 + r.M02, l.M03 + r.M03,
            l.M10 + r.M10, l.M11 + r.M11, l.M12 + r.M12, l.M13 + r.M13,
            l.M20 + r.M20, l.M21 + r.M21, l.M22 + r.M22, l.M23 + r.M23,
            l.M30 + r.M30, l.M31 + r.M31, l.M32 + r.M32, l.M33 + r.M33
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Subtract<T>(Matrix4x4<T> l, Matrix4x4<T> r) where T : INumberBase<T>
        => new(
            l.M00 - r.M00, l.M01 - r.M01, l.M02 - r.M02, l.M03 - r.M03,
            l.M10 - r.M10, l.M11 - r.M11, l.M12 - r.M12, l.M13 - r.M13,
            l.M20 - r.M20, l.M21 - r.M21, l.M22 - r.M22, l.M23 - r.M23,
            l.M30 - r.M30, l.M31 - r.M31, l.M32 - r.M32, l.M33 - r.M33
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> Multiply<T>(in Vector4<T> l, in Matrix4x4<T> r) where T : INumberBase<T>
        => new(
            l.X * r.M00 + l.Y * r.M10 + l.Z * r.M20 + l.W * r.M30,
            l.X * r.M01 + l.Y * r.M11 + l.Z * r.M21 + l.W * r.M31,
            l.X * r.M02 + l.Y * r.M12 + l.Z * r.M22 + l.W * r.M32,
            l.X * r.M03 + l.Y * r.M13 + l.Z * r.M23 + l.W * r.M33
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Multiply<T>(Matrix4x4<T> l, T r) where T : INumberBase<T>
        => new(
            l.M00 * r, l.M01 * r, l.M02 * r, l.M03 * r,
            l.M10 * r, l.M11 * r, l.M12 * r, l.M13 * r,
            l.M20 * r, l.M21 * r, l.M22 * r, l.M23 * r,
            l.M30 * r, l.M31 * r, l.M32 * r, l.M33 * r
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Multiply<T>(in Matrix4x4<T> l, in Matrix4x4<T> r) where T : INumberBase<T>
        => new(
            l.M00 * r.M00 + l.M01 * r.M10 + l.M02 * r.M20 + l.M03 * r.M30,
            l.M00 * r.M01 + l.M01 * r.M11 + l.M02 * r.M21 + l.M03 * r.M31,
            l.M00 * r.M02 + l.M01 * r.M12 + l.M02 * r.M22 + l.M03 * r.M32,
            l.M00 * r.M03 + l.M01 * r.M13 + l.M02 * r.M23 + l.M03 * r.M33,
            l.M10 * r.M00 + l.M11 * r.M10 + l.M12 * r.M20 + l.M13 * r.M30,
            l.M10 * r.M01 + l.M11 * r.M11 + l.M12 * r.M21 + l.M13 * r.M31,
            l.M10 * r.M02 + l.M11 * r.M12 + l.M12 * r.M22 + l.M13 * r.M32,
            l.M10 * r.M03 + l.M11 * r.M13 + l.M12 * r.M23 + l.M13 * r.M33,
            l.M20 * r.M00 + l.M21 * r.M10 + l.M22 * r.M20 + l.M23 * r.M30,
            l.M20 * r.M01 + l.M21 * r.M11 + l.M22 * r.M21 + l.M23 * r.M31,
            l.M20 * r.M02 + l.M21 * r.M12 + l.M22 * r.M22 + l.M23 * r.M32,
            l.M20 * r.M03 + l.M21 * r.M13 + l.M22 * r.M23 + l.M23 * r.M33,
            l.M30 * r.M00 + l.M31 * r.M10 + l.M32 * r.M20 + l.M33 * r.M30,
            l.M30 * r.M01 + l.M31 * r.M11 + l.M32 * r.M21 + l.M33 * r.M31,
            l.M30 * r.M02 + l.M31 * r.M12 + l.M32 * r.M22 + l.M33 * r.M32,
            l.M30 * r.M03 + l.M31 * r.M13 + l.M32 * r.M23 + l.M33 * r.M33
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Matrix4x4<T> Divide<T>(Matrix4x4<T> l, T r) where T : INumberBase<T>
        => new(
            l.M00 / r, l.M01 / r, l.M02 / r, l.M03 / r,
            l.M10 / r, l.M11 / r, l.M12 / r, l.M13 / r,
            l.M20 / r, l.M21 / r, l.M22 / r, l.M23 / r,
            l.M30 / r, l.M31 / r, l.M32 / r, l.M33 / r
        );
}
