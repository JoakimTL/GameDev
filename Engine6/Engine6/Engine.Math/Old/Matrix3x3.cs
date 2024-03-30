namespace Engine.Math.Old;

public readonly struct Matrix3x3<T>(T m00, T m01, T m02, T m10, T m11, T m12, T m20, T m21, T m22) where T : System.Numerics.INumber<T>
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

    public Matrix3x3(Vector3<T> row0, Vector3<T> row1, Vector3<T> row2)
        : this(row0.X, row0.Y, row0.Z, row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z) { }

    public Matrix3x3(Vector3Column<T> col0, Vector3Column<T> col1, Vector3Column<T> col2)
        : this(col0.X, col1.X, col2.X, col0.Y, col1.Y, col2.Y, col0.Z, col1.Z, col2.Z) { }

    public Vector3<T> Row0 => new(M00, M01, M02);
    public Vector3<T> Row1 => new(M10, M11, M12);
    public Vector3<T> Row2 => new(M20, M21, M22);

    public Vector3Column<T> Col0 => new(M00, M10, M20);
    public Vector3Column<T> Col1 => new(M01, M11, M21);
    public Vector3Column<T> Col2 => new(M02, M12, M22);

    public static readonly Matrix3x3<T> Identity = new(
        T.MultiplicativeIdentity, T.AdditiveIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.MultiplicativeIdentity, T.AdditiveIdentity,
        T.AdditiveIdentity, T.AdditiveIdentity, T.MultiplicativeIdentity
    );

    public T Determinant
        => this.GetDeterminantByExpansionOfMinors();


    #region Determinant Parts
    /// <summary>
    /// A 2x2 matrix exluding r0 and c0, used for determinants.
    /// </summary>
    public Matrix2x2<T> Excluding00 => new(
        M11, M12,
        M21, M22
    );

    /// <summary>
    /// A 2x2 matrix exluding r1 and c0, used for determinants.
    /// </summary>
    public Matrix2x2<T> Excluding10 => new(
        M01, M02,
        M21, M22
    );

    /// <summary>
    /// A 2x2 matrix exluding r2 and c0, used for determinants.
    /// </summary>
    public Matrix2x2<T> Excluding20 => new(
        M01, M02,
        M11, M12
    );
    #endregion

    public override string ToString() => $"[{M00}, {M01}, {M02}]\r\n[{M10}, {M11}, {M12}]\r\n[{M20}, {M21}, {M22}]";
}
