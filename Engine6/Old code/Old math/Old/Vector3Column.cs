namespace Engine.Math.NewFolder.Old;

public readonly struct Vector3Column<T> where T : System.Numerics.INumber<T>
{
    public readonly T X;
    public readonly T Y;
    public readonly T Z;

    internal Vector3Column(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Matrix3x3<T> operator *(Vector3Column<T> l, Vector3<T> r) => l.Multiply(r);

    public Vector3<T> AsRow => new(X, Y, Z);

    public override string ToString() => $"[{X}]\r\n[{Y}]\r\n[{Z}]";
}
