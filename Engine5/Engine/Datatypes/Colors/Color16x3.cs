using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Datatypes.Colors;

/// <summary>
/// Color with 2 bytes of precision per color.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct Color16x3
{
    public const uint SIZE = 6;
    public const float USHORT_INV = 1f / ushort.MaxValue;

    [FieldOffset(0)]
    private readonly ushort _red;
    [FieldOffset(2)]
    private readonly ushort _green;
    [FieldOffset(4)]
    private readonly ushort _blue;

    public static readonly Color16x3 Red = new(ushort.MaxValue, 0, 0);
    public static readonly Color16x3 Green = new(0, ushort.MaxValue, 0);
    public static readonly Color16x3 Blue = new(0, 0, ushort.MaxValue);
    public static readonly Color16x3 Black = new(0, 0, 0);
    public static readonly Color16x3 White = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);

    public Color16x3(ushort r, ushort g, ushort b)
    {
        _red = r;
        _green = g;
        _blue = b;
    }

    public Color16x3(in Vector3 vec4)
    {
        Vector3 valueTrue = Vector3.Clamp(vec4, Vector3.Zero, Vector3.One) * ushort.MaxValue;
        _red = (ushort)MathF.Floor(valueTrue.X);
        _green = (ushort)MathF.Floor(valueTrue.Y);
        _blue = (ushort)MathF.Floor(valueTrue.Z);
    }

    public override string ToString() => $"RGB16-{_red:X2}{_green:X2}{_blue:X2}";
    public override bool Equals(object? obj)
    {
        if (obj != null && obj is Color16x3 col)
            return Equals(this, col);
        return false;
    }
    public override int GetHashCode() => HashCode.Combine(_red, _green, _blue);
    public static bool Equals(Color16x3 a, Color16x3 b) => a._red == b._red && a._green == b._green && a._blue == b._blue;
    public static bool operator ==(Color16x3 a, Color16x3 b) => Equals(a, b);
    public static bool operator !=(Color16x3 a, Color16x3 b) => !Equals(a, b);
    public static implicit operator Color16x3(in Vector3 v) => new(v);
    public static implicit operator Vector3(Color16x3 v) => new Vector3(v._red, v._green, v._blue) * USHORT_INV;
}
