using System.Runtime.InteropServices;

namespace Engine.Math.Old;

/// <summary>
/// All vectors start as rows, but can be converted to columns.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector2<T>(T x, T y) where T : System.Numerics.INumber<T>
{
    public readonly T X = x;
    public readonly T Y = y;

    public Vector2(T s) : this(s, s) { }

    public static readonly Vector2<T> Zero = new(T.AdditiveIdentity);
    public static readonly Vector2<T> One = new(T.MultiplicativeIdentity);

    public static implicit operator Vector2<T>((T x, T y) tuple) => new(tuple.x, tuple.y);

    public static Vector2<T> operator +(Vector2<T> l, Vector2<T> r) => l.Add(r);
    public static Vector2<T> operator -(Vector2<T> l, Vector2<T> r) => l.Subtract(r);
    //public static Complex<T> operator *(Vector2<T> l, Vector2<T> r) => l.Multiply(r);
    public static Vector2<T> operator *(Vector2<T> l, T r) => l.ScalarMultiplication(r);
    //public static Vector2<T> operator /( Vector2<T> l, Vector2<T> r ) => l.HadamardDivision( r );
    public static Vector2<T> operator /(Vector2<T> l, T r) => l.ScalarDivision(r);
    public static Vector2<T> operator *(Vector2<T> l, Matrix2x2<T> r) => l.Multiply(r);
    public static T operator *(Vector2<T> l, Vector2Column<T> r) => l.Multiply(r);

    public Vector4<T> WorldTransformVector => new(X, Y, T.AdditiveIdentity, T.MultiplicativeIdentity);
    public Vector4<T> NormalTransformVector => new(X, Y, T.AdditiveIdentity, T.AdditiveIdentity);

    public Vector2Column<T> AsColumn => new(X, Y);

    public override string ToString() => $"[{X}, {Y}]";

}
