namespace Engine.Data;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Vector2b
{

    [System.Runtime.InteropServices.FieldOffset(0)]
    public byte X;
    [System.Runtime.InteropServices.FieldOffset(1)]
    public byte Y;

    public Vector2b(byte x, byte y)
    {
		this.X = x;
		this.Y = y;
    }

    public Vector2b(int x, int y)
    {
		this.X = (byte)x;
		this.Y = (byte)y;
    }

    public Vector2b(byte v)
    {
		this.X = this.Y = v;
    }

    #region Instance Methods
    public override string ToString() => $"v2b[{this.X},{this.Y}]";
    public bool Equals(Vector2b other) => this.X == other.X && this.Y == other.Y;
    public override bool Equals(object? obj) => obj is Vector2b v && Equals(v);
    public override int GetHashCode() => this.Y << 8 | this.X;
    #endregion

    #region Properties
    public Vector2i AsInt => new( this.X, this.Y );
    #endregion

    #region Operations
    public static bool operator ==(Vector2b a, Vector2b b) => a.Equals(b);
    public static bool operator !=(Vector2b a, Vector2b b) => !a.Equals(b);
    public static implicit operator Vector2b(Vector2i a) => new(a.X, a.Y);
    #endregion

}
