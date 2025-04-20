using System.Numerics;

namespace Engine.Datatypes.Vectors;

//TODO Turn into a generic Vec3
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Vector3i
{

    [System.Runtime.InteropServices.FieldOffset(0)]
    public int X;
    [System.Runtime.InteropServices.FieldOffset(4)]
    public int Y;
    [System.Runtime.InteropServices.FieldOffset(8)]
    public int Z;

    public Vector3i(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3i(int v)
    {
        X = Y = Z = v;
    }

    #region Instance Methods
    public Vector3i Negate() => new(-X, -Y, -Z);
    public bool Inside(AABB3i b) => AABB3i.Inside(ref b, ref this);
    public override string ToString() => $"Vector3i[{X},{Y},{Z}]";
    public bool Equals(Vector3i other) => X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is Vector3i v && Equals(v);
    public override int GetHashCode() => X ^ Y ^ Z;
    #endregion

    #region Properties
    public Vector2i XY => new(X, Y);
    public Vector2i XZ => new(X, Z);
    public Vector2i YZ => new(Y, Z);
    public Vector3 AsFloat => new(X, Y, Z);
    #endregion

    #region Static Methods
    public static Vector3i Min(Vector3i a, Vector3i b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    public static Vector3i Max(Vector3i a, Vector3i b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    public static Vector3i Round(Vector3 v) => new((int)MathF.Round(v.X), (int)MathF.Round(v.Y), (int)MathF.Round(v.Z));
    public static Vector3i Floor(Vector3 v) => new((int)MathF.Floor(v.X), (int)MathF.Floor(v.Y), (int)MathF.Floor(v.Z));
    public static Vector3i Ceiling(Vector3 v) => new((int)MathF.Ceiling(v.X), (int)MathF.Ceiling(v.Y), (int)MathF.Ceiling(v.Z));
    public static Vector3i Cast(Vector3 v) => new((int)v.X, (int)v.Y, (int)v.Z);
    public static Vector3i Abs(Vector3i v) => new(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));

    #region Static Operations
    public static Vector3i Add(Vector3i a, Vector3i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 Add(Vector3i a, Vector3 b) => a.AsFloat + b;
    public static Vector3 Add(Vector3 a, Vector3i b) => a + b.AsFloat;
    public static Vector3i Subtract(Vector3i a, Vector3i b) => Add(a, b.Negate());
    public static Vector3i Multiply(Vector3i a, Vector3i b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vector3 Multiply(Vector3i a, Vector3 b) => a.AsFloat * b;
    public static Vector3 Multiply(Vector3 a, Vector3i b) => a * b.AsFloat;
    public static Vector3i Multiply(Vector3i a, int s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 Multiply(Vector3i a, float s) => a.AsFloat * s;
    public static Vector3 Divide(Vector3i a, Vector3 b) => a.AsFloat / b;
    public static Vector3 Divide(Vector3 a, Vector3i b) => a / b.AsFloat;
    public static Vector3 Divide(Vector3i a, float s) => a.AsFloat / s;
    #endregion

    #region Operations
    public static Vector3i operator +(Vector3i a, Vector3i b) => Add(a, b);
    public static Vector3i operator -(Vector3i a) => a.Negate();
    public static Vector3i operator -(Vector3i a, Vector3i b) => Subtract(a, b);
    public static Vector3i operator *(Vector3i a, Vector3i b) => Multiply(a, b);
    public static Vector3 operator *(Vector3 a, Vector3i b) => Multiply(a, b);
    public static Vector3 operator *(Vector3i a, Vector3 b) => Multiply(a, b);
    public static Vector3i operator *(Vector3i a, int s) => Multiply(a, s);
    public static Vector3 operator *(Vector3i a, float s) => Multiply(a, s);
    public static Vector3 operator /(Vector3 a, Vector3i b) => Divide(a, b);
    public static Vector3 operator /(Vector3i a, Vector3 b) => Divide(a, b);
    public static Vector3 operator /(Vector3i a, float s) => Divide(a, s);
    public static bool operator ==(Vector3i a, Vector3i b) => a.Equals(b);
    public static bool operator !=(Vector3i a, Vector3i b) => !a.Equals(b);
    public static implicit operator Vector3i((int, int, int) a) => new(a.Item1, a.Item2, a.Item3);
    public static implicit operator Vector3i(int a) => new(a);
    #endregion
    #endregion

}


//[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
//public struct Vector3i128 {

//	[System.Runtime.InteropServices.FieldOffset( 0 )]
//	public Int128 X;
//	[System.Runtime.InteropServices.FieldOffset( 16 )]
//	public Int128 Y;
//	[System.Runtime.InteropServices.FieldOffset( 32 )]
//	public Int128 Z;

//    public Vector3i128(Int128 x, Int128 y, Int128 z) {
//		X = x;
//		Y = y;
//		Z = z;
//        Int128.
//	}

//}

//public static class Vector3i128Extensions {
//	public const long YearInSeconds = 31_556_926;
//	public const long SpeedOfLight = 299_792_458;
//	public const long LightYear = 9_460_730_472_580_800;

    
//	public static Vector3i128 LightYearsToMeters( this Vector3 lightYears ) => new Vector3i128(new Int128();

//}