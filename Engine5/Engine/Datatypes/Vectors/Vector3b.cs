namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Vector3b
{

    [System.Runtime.InteropServices.FieldOffset(0)]
    public byte X;
    [System.Runtime.InteropServices.FieldOffset(1)]
    public byte Y;
    [System.Runtime.InteropServices.FieldOffset(2)]
    public byte Z;

    public Vector3b(byte x, byte y, byte z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3b(int x, int y, int z)
    {
        X = (byte)x;
        Y = (byte)y;
        Z = (byte)z;
    }

    public Vector3b(byte v)
    {
        X = Y = Z = v;
    }

    #region Instance Methods
    public override string ToString() => $"Vector3b[{X},{Y},{Z}]";
    public bool Equals(Vector3b other) => X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is Vector3b v && Equals(v);
    public override int GetHashCode() => (Z << 16) | (Y << 8) | X;
    #endregion

    #region Properties
    public Vector2b XY => new(X, Y);
    public Vector3i AsInt => new(X, Y, Z);
    #endregion

    #region Operations
    public static bool operator ==(Vector3b a, Vector3b b) => a.Equals(b);
    public static bool operator !=(Vector3b a, Vector3b b) => !a.Equals(b);
    public static implicit operator Vector3b(Vector3i a) => new(a.X, a.Y, a.Z);
    #endregion

}
