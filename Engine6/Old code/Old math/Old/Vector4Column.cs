namespace Engine.Math.NewFolder.Old;

public readonly struct Vector4Column<T> where T : System.Numerics.INumber<T>
{
    public readonly T X;
    public readonly T Y;
    public readonly T Z;
    public readonly T W;

    internal Vector4Column(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public static Matrix4x4<T> operator *(Vector4Column<T> l, Vector4<T> r) => l.Multiply(r);

    public Vector4<T> AsRow => new(X, Y, Z, W);

    public override string ToString() => $"[{X}]\r\n[{Y}]\r\n[{Z}]\r\n[{W}]";

}
