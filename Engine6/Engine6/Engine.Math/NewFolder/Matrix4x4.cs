using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder.Interfaces;

namespace Engine.Math.NewFolder;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Matrix4x4<T>(T m00, T m01, T m02, T m03, T m10, T m11, T m12, T m13, T m20, T m21, T m22, T m23, T m30, T m31, T m32, T m33) :
        ILinearOperators<Matrix4x4<T>, T>,
        IProductOperator<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>
    where T :
        unmanaged, INumber<T>
{

    /// <summary>First row, first column</summary>
    public readonly T M00 = m00;
    /// <summary>First row, second column</summary>
    public readonly T M01 = m01;
    /// <summary>First row, third column</summary>
    public readonly T M02 = m02;
    /// <summary>First row, fourth column</summary>
    public readonly T M03 = m03;
    /// <summary>Second row, first column</summary>
    public readonly T M10 = m10;
    /// <summary>Second row, second column</summary>
    public readonly T M11 = m11;
    /// <summary>Second row, third column</summary>
    public readonly T M12 = m12;
    /// <summary>Second row, fourth column</summary>
    public readonly T M13 = m13;
    /// <summary>Third row, first column</summary>
    public readonly T M20 = m20;
    /// <summary>Third row, second column</summary>
    public readonly T M21 = m21;
    /// <summary>Third row, third column</summary>
    public readonly T M22 = m22;
    /// <summary>Third row, fourth column</summary>
    public readonly T M23 = m23;
    /// <summary>Fourth row, first column</summary>
    public readonly T M30 = m30;
    /// <summary>Fourth row, second column</summary>
    public readonly T M31 = m31;
    /// <summary>Fourth row, third column</summary>
    public readonly T M32 = m32;
    /// <summary>Fourth row, fourth column</summary>
    public readonly T M33 = m33;

    public Vector4<T> Row0 => new(M00, M01, M02, M03);
    public Vector4<T> Row1 => new(M10, M11, M12, M13);
    public Vector4<T> Row2 => new(M20, M21, M22, M23);
    public Vector4<T> Row3 => new(M30, M31, M32, M33);
    public Vector4<T> Col0 => new(M00, M10, M20, M30);
    public Vector4<T> Col1 => new(M01, M11, M21, M31);
    public Vector4<T> Col2 => new(M02, M12, M22, M32);
    public Vector4<T> Col3 => new(M03, M13, M23, M33);

    public static Matrix4x4<T> Identity => new(
        T.One, T.Zero, T.Zero, T.Zero,
        T.Zero, T.One, T.Zero, T.Zero,
        T.Zero, T.Zero, T.One, T.Zero,
        T.Zero, T.Zero, T.Zero, T.One
    );

    public Matrix3x3<T> Excluding00 => new(M11, M12, M13, M21, M22, M23, M31, M32, M33);
    public Matrix3x3<T> Excluding01 => new(M10, M12, M13, M20, M22, M23, M30, M32, M33);
    public Matrix3x3<T> Excluding02 => new(M10, M11, M13, M20, M21, M23, M30, M31, M33);
    public Matrix3x3<T> Excluding03 => new(M10, M11, M12, M20, M21, M22, M30, M31, M32);
    public Matrix3x3<T> Excluding10 => new(M01, M02, M03, M21, M22, M23, M31, M32, M33);
    public Matrix3x3<T> Excluding11 => new(M00, M02, M03, M20, M22, M23, M30, M32, M33);
    public Matrix3x3<T> Excluding12 => new(M00, M01, M03, M20, M21, M23, M30, M31, M33);
    public Matrix3x3<T> Excluding13 => new(M00, M01, M02, M20, M21, M22, M30, M31, M32);
    public Matrix3x3<T> Excluding20 => new(M01, M02, M03, M11, M12, M13, M31, M32, M33);
    public Matrix3x3<T> Excluding21 => new(M00, M02, M03, M10, M12, M13, M30, M32, M33);
    public Matrix3x3<T> Excluding22 => new(M00, M01, M03, M10, M11, M13, M30, M31, M33);
    public Matrix3x3<T> Excluding23 => new(M00, M01, M02, M10, M11, M12, M30, M31, M32);
    public Matrix3x3<T> Excluding30 => new(M01, M02, M03, M11, M12, M13, M21, M22, M23);
    public Matrix3x3<T> Excluding31 => new(M00, M02, M03, M10, M12, M13, M20, M22, M23);
    public Matrix3x3<T> Excluding32 => new(M00, M01, M03, M10, M11, M13, M20, M21, M23);
    public Matrix3x3<T> Excluding33 => new(M00, M01, M02, M10, M11, M12, M20, M21, M22);

    public static Matrix4x4<T> operator -(in Matrix4x4<T> l) => l.Negate();
    public static Matrix4x4<T> operator -(in Matrix4x4<T> l, in Matrix4x4<T> r) => l.Subtract(r);
    public static Matrix4x4<T> operator +(in Matrix4x4<T> l, in Matrix4x4<T> r) => l.Add(r);
    public static Matrix4x4<T> operator *(in Matrix4x4<T> l, T r) => l.ScalarMultiply(r);
    public static Matrix4x4<T> operator *(T l, in Matrix4x4<T> r) => r.ScalarMultiply(l);
    public static Matrix4x4<T> operator /(in Matrix4x4<T> l, T r) => l.ScalarDivide(r);
    public static Matrix4x4<T> operator *(in Matrix4x4<T> l, in Matrix4x4<T> r) => l.Multiply(r);

    public static bool operator ==(in Matrix4x4<T> l, in Matrix4x4<T> r) => l.Row0 == r.Row0 && l.Row1 == r.Row1 && l.Row2 == r.Row2 && l.Row3 == r.Row3;
    public static bool operator !=(in Matrix4x4<T> l, in Matrix4x4<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix4x4<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Row0, Row1, Row2, Row3);
    public override string ToString() => $"[{Row0}]/[{Row1}]/[{Row2}]/[{Row3}]";
}
