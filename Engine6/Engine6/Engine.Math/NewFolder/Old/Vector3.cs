using System.Runtime.InteropServices;

namespace Engine.Math.NewFolder.Old;

/// <summary>
/// All vectors start as rows, but can be converted to columns.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector3<T>(T x, T y, T z) where T : System.Numerics.INumber<T>
{
    public readonly T X = x;
    public readonly T Y = y;
    public readonly T Z = z;

    public Vector3(T s) : this(s, s, s) { }

    public static readonly Vector3<T> Zero = new(T.AdditiveIdentity);
    public static readonly Vector3<T> One = new(T.MultiplicativeIdentity);

    public static implicit operator Vector3<T>((T x, T y, T z) tuple) => new(tuple.x, tuple.y, tuple.z);

    public static Vector3<T> operator +(Vector3<T> l, Vector3<T> r) => l.Add(r);
    public static Vector3<T> operator -(Vector3<T> l, Vector3<T> r) => l.Subtract(r);
    public static Quaternion<T> operator *(Vector3<T> l, Vector3<T> r) => l.Multiply(r);
    public static Vector3<T> operator *(Vector3<T> l, T r) => l.ScalarMultiplication(r);
    //public static Vector3<T> operator /( Vector3<T> l, Vector3<T> r ) => l.Divide( r );
    public static Vector3<T> operator /(Vector3<T> l, T r) => l.ScalarDivision(r);
    public static Vector3<T> operator *(Vector3<T> l, Matrix3x3<T> r) => l.Multiply(r);
    public static T operator *(Vector3<T> l, Vector3Column<T> r) => l.Multiply(r);

    public Vector4<T> WorldTransformVector => new(X, Y, Z, T.MultiplicativeIdentity);
    public Vector4<T> NormalTransformVector => new(X, Y, Z, T.AdditiveIdentity);

    public Vector3Column<T> AsColumn => new(X, Y, Z);

    public Vector2<T> XY => new(X, Y);
    public Vector2<T> XZ => new(X, Z);
    public Vector2<T> YZ => new(Y, Z);

    public override string ToString() => $"[{X}, {Y}, {Z}]";
}
