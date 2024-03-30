namespace Engine.Math.Old;

public readonly struct Vector2Column<T> where T : System.Numerics.INumber<T>
{
    public readonly T X;
    public readonly T Y;

    internal Vector2Column(T x, T y)
    {
        X = x;
        Y = y;
    }

    public static Matrix2x2<T> operator *(Vector2Column<T> l, Vector2<T> r) => l.Multiply(r);

    public Vector2<T> AsRow => new(X, Y);

    public override string ToString() => $"[{X}]\r\n[{Y}]";
}
