using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder.Interfaces;

namespace Engine.Math.NewFolder;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Matrix3x3<T>(T m00, T m01, T m02, T m10, T m11, T m12, T m20, T m21, T m22) :
        ILinearOperators<Matrix3x3<T>, T>,
        IProductOperator<Matrix3x3<T>, Matrix3x3<T>, Matrix3x3<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T M00 = m00;
    public readonly T M01 = m01;
    public readonly T M02 = m02;
    public readonly T M10 = m10;
    public readonly T M11 = m11;
    public readonly T M12 = m12;
    public readonly T M20 = m20;
    public readonly T M21 = m21;
    public readonly T M22 = m22;

    public Vector3<T> Row0 => new(M00, M01, M02);
    public Vector3<T> Row1 => new(M10, M11, M12);
    public Vector3<T> Row2 => new(M20, M21, M22);
    public Vector3<T> Col0 => new(M00, M10, M20);
    public Vector3<T> Col1 => new(M01, M11, M21);
    public Vector3<T> Col2 => new(M02, M12, M22);

    public static Matrix3x3<T> Identity => new(
        T.One, T.Zero, T.Zero,
        T.Zero, T.One, T.Zero,
        T.Zero, T.Zero, T.One
    );

    public Matrix2x2<T> Excluding00 => new(M11, M12, M21, M22);
    public Matrix2x2<T> Excluding01 => new(M10, M12, M20, M22);
    public Matrix2x2<T> Excluding02 => new(M10, M11, M20, M21);
    public Matrix2x2<T> Excluding10 => new(M01, M02, M21, M22);
    public Matrix2x2<T> Excluding11 => new(M00, M02, M20, M22);
    public Matrix2x2<T> Excluding12 => new(M00, M01, M20, M21);
    public Matrix2x2<T> Excluding20 => new(M01, M02, M11, M12);
    public Matrix2x2<T> Excluding21 => new(M00, M02, M10, M12);
    public Matrix2x2<T> Excluding22 => new(M00, M01, M10, M11);

    public static Matrix3x3<T> operator -(in Matrix3x3<T> l) => l.Negate();
    public static Matrix3x3<T> operator -(in Matrix3x3<T> l, in Matrix3x3<T> r) => l.Subtract(r);
    public static Matrix3x3<T> operator +(in Matrix3x3<T> l, in Matrix3x3<T> r) => l.Add(r);
    public static Matrix3x3<T> operator *(in Matrix3x3<T> l, T r) => l.ScalarMultiply(r);
    public static Matrix3x3<T> operator *(T l, in Matrix3x3<T> r) => r.ScalarMultiply(l);
    public static Matrix3x3<T> operator /(in Matrix3x3<T> l, T r) => l.ScalarDivide(r);
    public static Matrix3x3<T> operator *(in Matrix3x3<T> l, in Matrix3x3<T> r) => l.Multiply(r);

    public static bool operator ==(in Matrix3x3<T> l, in Matrix3x3<T> r) => l.Row0 == r.Row0 && l.Row1 == r.Row1 && l.Row2 == r.Row2;
    public static bool operator !=(in Matrix3x3<T> l, in Matrix3x3<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix3x3<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Row0, Row1, Row2);
    public override string ToString() => $"[{Row0}]/[{Row1}]/[{Row2}]";
}
