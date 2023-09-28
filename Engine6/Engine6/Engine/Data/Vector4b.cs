namespace Engine.Data;

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
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.W = w;
    }

    public Vector4b(int x, int y, int z, int w)
    {
		this.X = (byte)x;
		this.Y = (byte)y;
		this.Z = (byte)z;
		this.W = (byte)w;
    }

    public Vector4b(byte v)
    {
		this.X = this.Y = this.Z = this.W = v;
    }

    public static readonly Vector4b Zero = new(0);
    public static readonly Vector4b One = new(1);
    public static readonly Vector4b White = new(255, 255, 255, 255);
    public static readonly Vector4b Black = new(0, 0, 0, 255);
    public static readonly Vector4b Red = new(255, 0, 0, 255);
    public static readonly Vector4b Green = new(0, 255, 0, 255);
    public static readonly Vector4b Blue = new(0, 0, 255, 255);

    #region Instance Methods
    public override string ToString() => $"v4b[{this.X},{this.Y},{this.Z},{this.W}]";
    public bool Equals(Vector4b other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
    public override bool Equals(object? obj) => obj is Vector4b v && Equals(v);
    public override int GetHashCode() => this.W << 24 | this.Z << 16 | this.Y << 8 | this.X;
    #endregion

    #region Properties
    public Vector2b XY => new( this.X, this.Y );
    public Vector3b XYZ => new( this.X, this.Y, this.Z );
    public Vector4i AsInt => new( this.X, this.Y, this.Z, this.W );
    #endregion

    #region Operations
    public static bool operator ==(Vector4b a, Vector4b b) => a.Equals(b);
    public static bool operator !=(Vector4b a, Vector4b b) => !a.Equals(b);
    public static implicit operator Vector4b(Vector4i a) => new(a.X, a.Y, a.Z, a.W);
    public static implicit operator uint(Vector4b a) => (uint)a.X << 24 | (uint)a.Y << 16 | (uint)a.Z << 8 | a.W;
    #endregion

}
