namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Vector4b
{

    [System.Runtime.InteropServices.FieldOffset(0)]
    public byte X;
    [System.Runtime.InteropServices.FieldOffset(1)]
    public byte Y;
    [System.Runtime.InteropServices.FieldOffset(2)]
    public byte Z;
    [System.Runtime.InteropServices.FieldOffset(3)]
    public byte W;

    public Vector4b(byte x, byte y, byte z, byte w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4b(int x, int y, int z, int w)
    {
        X = (byte)x;
        Y = (byte)y;
        Z = (byte)z;
        W = (byte)w;
    }

    public Vector4b(byte v)
    {
        X = Y = Z = W = v;
    }

    public static readonly Vector4b Zero = new(0);
    public static readonly Vector4b One = new(1);
    public static readonly Vector4b White = new(255, 255, 255, 255);
    public static readonly Vector4b Black = new(0, 0, 0, 255);
    public static readonly Vector4b Red = new(255, 0, 0, 255);
    public static readonly Vector4b Green = new(0, 255, 0, 255);
    public static readonly Vector4b Blue = new(0, 0, 255, 255);

    #region Instance Methods
    public override string ToString() => $"Vector4b[{X},{Y},{Z},{W}]";
    public bool Equals(Vector4b other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    public override bool Equals(object? obj) => obj is Vector4b v && Equals(v);
    public override int GetHashCode() => W << 24 | Z << 16 | Y << 8 | X;
    #endregion

    #region Properties
    public Vector2b XY => new(X, Y);
    public Vector3b XYZ => new(X, Y, Z);
    public Vector4i AsInt => new(X, Y, Z, W);
    #endregion

    #region Operations
    public static bool operator ==(Vector4b a, Vector4b b) => a.Equals(b);
    public static bool operator !=(Vector4b a, Vector4b b) => !a.Equals(b);
    public static implicit operator Vector4b(Vector4i a) => new(a.X, a.Y, a.Z, a.W);
    public static implicit operator uint(Vector4b a) => (uint)a.X << 24 | (uint)a.Y << 16 | (uint)a.Z << 8 | a.W;
    #endregion

}
