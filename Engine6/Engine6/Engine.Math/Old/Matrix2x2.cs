namespace Engine.Math.Old;

public readonly struct Matrix2x2<T>(T m00, T m01, T m10, T m11) where T : System.Numerics.INumber<T>
{
    public readonly T M00 = m00;
    public readonly T M01 = m01;
    public readonly T M10 = m10;
    public readonly T M11 = m11;

    public Matrix2x2(Vector2<T> row0, Vector2<T> row1)
        : this(row0.X, row0.Y, row1.X, row1.Y) { }

    public Matrix2x2(Vector2Column<T> col0, Vector2Column<T> col1)
        : this(col0.X, col1.X, col0.Y, col1.Y) { }

    public Vector2<T> Row0 => new(M00, M01);
    public Vector2<T> Row1 => new(M10, M11);

    public Vector2Column<T> Col0 => new(M00, M10);
    public Vector2Column<T> Col1 => new(M01, M11);

    public static readonly Matrix2x2<T> Identity = new(
        T.MultiplicativeIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.MultiplicativeIdentity
    );

    public T Determinant
        => this.GetDeterminant();

    public override string ToString() => $"[{M00}, {M01}]\r\n[{M10}, {M11}]";
}