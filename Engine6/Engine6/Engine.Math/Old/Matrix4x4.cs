using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Math.Old;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Matrix4x4<T>(T m00, T m01, T m02, T m03, T m10, T m11, T m12, T m13, T m20, T m21, T m22, T m23, T m30, T m31, T m32, T m33) where T : INumber<T>
{
    public readonly T M00 = m00;
    public readonly T M01 = m01;
    public readonly T M02 = m02;
    public readonly T M03 = m03;
    public readonly T M10 = m10;
    public readonly T M11 = m11;
    public readonly T M12 = m12;
    public readonly T M13 = m13;
    public readonly T M20 = m20;
    public readonly T M21 = m21;
    public readonly T M22 = m22;
    public readonly T M23 = m23;
    public readonly T M30 = m30;
    public readonly T M31 = m31;
    public readonly T M32 = m32;
    public readonly T M33 = m33;

    public Matrix4x4(Vector4<T> row0, Vector4<T> row1, Vector4<T> row2, Vector4<T> row3)
        : this(row0.X, row0.Y, row0.Z, row0.W, row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W) { }

    public Matrix4x4(Vector4Column<T> col0, Vector4Column<T> col1, Vector4Column<T> col2, Vector4Column<T> col3)
        : this(col0.X, col1.X, col2.X, col3.X, col0.Y, col1.Y, col2.Y, col3.Y, col0.Z, col1.Z, col2.Z, col3.Z, col0.W, col1.W, col2.W, col3.W) { }

    public Vector4<T> Row0 => new(M00, M01, M02, M03);
    public Vector4<T> Row1 => new(M10, M11, M12, M13);
    public Vector4<T> Row2 => new(M20, M21, M22, M23);
    public Vector4<T> Row3 => new(M30, M31, M32, M33);

    public Vector4Column<T> Col0 => new(M00, M10, M20, M30);
    public Vector4Column<T> Col1 => new(M01, M11, M21, M31);
    public Vector4Column<T> Col2 => new(M02, M12, M22, M32);
    public Vector4Column<T> Col3 => new(M03, M13, M23, M33);

    public static readonly Matrix4x4<T> Identity = new(
        T.MultiplicativeIdentity, T.AdditiveIdentity, T.AdditiveIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.MultiplicativeIdentity, T.AdditiveIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.AdditiveIdentity, T.MultiplicativeIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.AdditiveIdentity, T.AdditiveIdentity, T.MultiplicativeIdentity
    );


    public static Matrix4x4<T> operator +(in Matrix4x4<T> l, in Matrix4x4<T> r)
        => l.Add(r);
    public static Matrix4x4<T> operator -(in Matrix4x4<T> l, in Matrix4x4<T> r)
        => l.Subtract(r);
    public static Matrix4x4<T> operator *(in Matrix4x4<T> l, in Matrix4x4<T> r)
        => l.Multiply(r);
    public static Matrix4x4<T> operator *(in Matrix4x4<T> l, in T r)
        => l.Multiply(r);
    public static Matrix4x4<T> operator /(in Matrix4x4<T> l, in T r)
        => l.Divide(r);

    public T Determinant
        => this.GetDeterminantByExpansionOfMinors();

    #region Determinant Parts
    /// <summary>
    /// A 3x3 matrix exluding r0 and c0, used for determinants.
    /// </summary>
    public Matrix3x3<T> Excluding00 => new(
        M11, M12, M13,
        M21, M22, M23,
        M31, M32, M33
    );

    /// <summary>
    /// A 3x3 matrix exluding r1 and c0, used for determinants.
    /// </summary>
    public Matrix3x3<T> Excluding10 => new(
        M01, M02, M03,
        M21, M22, M23,
        M31, M32, M33
    );

    /// <summary>
    /// A 3x3 matrix exluding r2 and c0, used for determinants.
    /// </summary>
    public Matrix3x3<T> Excluding20 => new(
        M01, M02, M03,
        M11, M12, M13,
        M31, M32, M33
    );

    /// <summary>
    /// A 3x3 matrix exluding r3 and c0, used for determinants.
    /// </summary>
    public Matrix3x3<T> Excluding30 => new(
        M01, M02, M03,
        M11, M12, M13,
        M21, M22, M23
    );
    #endregion

    public override string ToString() => $"[{M00}, {M01}, {M02}, {M03}]\r\n[{M10}, {M11}, {M12}, {M13}]\r\n[{M20}, {M21}, {M22}, {M23}]\r\n[{M30}, {M31}, {M32}, {M33}]";
}