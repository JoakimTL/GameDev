using System.Numerics;

namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Vector4i
{

    [System.Runtime.InteropServices.FieldOffset(0)]
    public int X;
    [System.Runtime.InteropServices.FieldOffset(4)]
    public int Y;
    [System.Runtime.InteropServices.FieldOffset(8)]
    public int Z;
    [System.Runtime.InteropServices.FieldOffset(12)]
    public int W;

    public Vector4i(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4i(int v)
    {
        X = Y = Z = W = v;
    }

    #region Instance Methods
    public Vector4i Negate() => new(-X, -Y, -Z, -W);
    public override string ToString() => $"Vector4i[{X},{Y},{Z},{W}]";
    public bool Equals(Vector4i other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    public override bool Equals(object? obj) => obj is Vector4i v && Equals(v);
    public override int GetHashCode() => X ^ Y ^ Z ^ W;
    #endregion

    #region Properties
    public Vector2i XY => new(X, Y);
    public Vector3i XYZ => new(X, Y, Z);
    public Vector4 AsFloat => new(X, Y, Z, W);
    #endregion

    #region Static Methods
    public static Vector4i Min(Vector4i a, Vector4i b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z), Math.Min(a.W, b.W));
    public static Vector4i Max(Vector4i a, Vector4i b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z), Math.Max(a.W, b.W));
    public static Vector4i Round(Vector4 v) => new((int)MathF.Round(v.X), (int)MathF.Round(v.Y), (int)MathF.Round(v.Z), (int)MathF.Round(v.W));
    public static Vector4i Floor(Vector4 v) => new((int)MathF.Floor(v.X), (int)MathF.Floor(v.Y), (int)MathF.Floor(v.Z), (int)MathF.Floor(v.W));
    public static Vector4i Ceiling(Vector4 v) => new((int)MathF.Ceiling(v.X), (int)MathF.Ceiling(v.Y), (int)MathF.Ceiling(v.Z), (int)MathF.Ceiling(v.W));
    public static Vector4i Cast(Vector4 v) => new((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);

    #region Static Operations
    public static Vector4i Add(Vector4i a, Vector4i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static Vector4 Add(Vector4i a, Vector4 b) => a.AsFloat + b;
    public static Vector4 Add(Vector4 a, Vector4i b) => a + b.AsFloat;
    public static Vector4i Subtract(Vector4i a, Vector4i b) => Add(a, b.Negate());
    public static Vector4i Multiply(Vector4i a, Vector4i b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    public static Vector4 Multiply(Vector4i a, Vector4 b) => a.AsFloat * b;
    public static Vector4 Multiply(Vector4 a, Vector4i b) => a * b.AsFloat;
    public static Vector4i Multiply(Vector4i a, int s) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);
    public static Vector4 Multiply(Vector4i a, float s) => a.AsFloat * s;
    public static Vector4 Divide(Vector4i a, Vector4 b) => a.AsFloat / b;
    public static Vector4 Divide(Vector4 a, Vector4i b) => a / b.AsFloat;
    public static Vector4 Divide(Vector4i a, float s) => a.AsFloat / s;
    #endregion

    #region Operations
    public static Vector4i operator +(Vector4i a, Vector4i b) => Add(a, b);
    public static Vector4i operator -(Vector4i a) => a.Negate();
    public static Vector4i operator -(Vector4i a, Vector4i b) => Subtract(a, b);
    public static Vector4i operator *(Vector4i a, Vector4i b) => Multiply(a, b);
    public static Vector4 operator *(Vector4 a, Vector4i b) => Multiply(a, b);
    public static Vector4 operator *(Vector4i a, Vector4 b) => Multiply(a, b);
    public static Vector4i operator *(Vector4i a, int s) => Multiply(a, s);
    public static Vector4 operator *(Vector4i a, float s) => Multiply(a, s);
    public static Vector4 operator /(Vector4 a, Vector4i b) => Divide(a, b);
    public static Vector4 operator /(Vector4i a, Vector4 b) => Divide(a, b);
    public static Vector4 operator /(Vector4i a, float s) => Divide(a, s);
    public static bool operator ==(Vector4i a, Vector4i b) => a.Equals(b);
    public static bool operator !=(Vector4i a, Vector4i b) => !a.Equals(b);
    public static implicit operator Vector4i(Vector4b a) => new(a.X, a.Y, a.Z, a.W);
    public static implicit operator Vector4i((int, int, int, int) a) => new(a.Item1, a.Item2, a.Item3, a.Item4);
    public static implicit operator Vector4i(int a) => new(a);
    #endregion
    #endregion

}
