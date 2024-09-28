using System.Runtime.InteropServices;

namespace Engine.Math.NewFolder.Old;

/// <summary>
/// All vectors start as rows, but can be converted to columns.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector4<T>(T x, T y, T z, T w) where T : System.Numerics.INumber<T>
{
    public readonly T X = x;
    public readonly T Y = y;
    public readonly T Z = z;
    public readonly T W = w;

    public Vector4(T s) : this(s, s, s, s) { }

    public static readonly Vector4<T> Zero = new(T.AdditiveIdentity);
    public static readonly Vector4<T> One = new(T.MultiplicativeIdentity);

    public static implicit operator Vector4<T>((T x, T y, T z, T w) tuple) => new(tuple.x, tuple.y, tuple.z, tuple.w);

    public static Vector4<T> operator +(Vector4<T> l, Vector4<T> r) => l.Add(r);
    public static Vector4<T> operator -(Vector4<T> l, Vector4<T> r) => l.Subtract(r);
    //public static Quaternion<T> operator *( Vector4<T> l, Vector4<T> r ) => l.Multiply( r );
    public static Vector4<T> operator *(Vector4<T> l, T r) => l.ScalarMultiplication(r);
    //public static Vector4<T> operator /( Vector4<T> l, Vector4<T> r ) => l.Divide( r );
    public static Vector4<T> operator /(Vector4<T> l, T r) => l.ScalarDivision(r);
    public static Vector4<T> operator *(Vector4<T> l, Matrix4x4<T> r) => l.Multiply(r);
    public static T operator *(Vector4<T> l, Vector4Column<T> r) => l.Multiply(r);

    public Vector4Column<T> AsColumn => new(X, Y, Z, W);


    public Vector2<T> XY => new(X, Y);
    public Vector2<T> XZ => new(X, Z);
    public Vector2<T> XW => new(X, W);
    public Vector2<T> YZ => new(Y, Z);
    public Vector2<T> YW => new(Y, W);
    public Vector2<T> ZW => new(Z, W);
    public Vector3<T> XYZ => new(X, Y, Z);
    public Vector3<T> XYW => new(X, Y, W);
    public Vector3<T> XZW => new(X, Z, W);
    public Vector3<T> YZW => new(Y, Z, W);

    internal Vector2<T> GetTransformedVector2()
        => new Vector2<T>(X, Y) / W;
    internal Vector3<T> GetTransformedVector3()
        => new Vector3<T>(X, Y, Z) / W;

    public override string ToString() => $"[{X}, {Y}, {Z}, {W}]";

}
