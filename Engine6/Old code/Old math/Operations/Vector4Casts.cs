using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Operations;

/// <summary>
/// Casting methods for <see cref="Vector4{T}"/>. Return types may vary.
/// </summary>
public static class Vector4Casts
{
    /// <returns>The vector values, but throws an exception if the value can't be represented with <typeparamref name="TOut"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<TOut> CastChecked<TIn, TOut>(in this Vector4<TIn> l) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
        => new(TOut.CreateChecked(l.X), TOut.CreateChecked(l.Y), TOut.CreateChecked(l.Z), TOut.CreateChecked(l.W));

    /// <returns>The vector values retaining the bit values within the bit width of the new values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<TOut> CastTruncating<TIn, TOut>(in this Vector4<TIn> l) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
        => new(TOut.CreateTruncating(l.X), TOut.CreateTruncating(l.Y), TOut.CreateTruncating(l.Z), TOut.CreateTruncating(l.W));

    /// <returns>The vector values clamped to min and max value of <typeparamref name="TOut"/> MinValue and MaxValue.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<TOut> CastSaturating<TIn, TOut>(in this Vector4<TIn> l) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
        => new(TOut.CreateSaturating(l.X), TOut.CreateSaturating(l.Y), TOut.CreateSaturating(l.Z), TOut.CreateSaturating(l.W));

    public static Vector4<ushort> FromNormalizedTo16Bits(in this Vector4<double> l, double min = 0, double max = 1)
    {
        double space = max - min;
        if (System.Math.Abs(space) < 0.00001525878)
            throw new ArgumentException($"{nameof(min)} - {nameof(max)} is equal to 0, meaning the space can't be normalized.");
        double spaceInverse = 1d / space;
        return new(
            (byte)System.Math.Round((l.X - min) * spaceInverse * ushort.MaxValue),
            (byte)System.Math.Round((l.Y - min) * spaceInverse * ushort.MaxValue),
            (byte)System.Math.Round((l.Z - min) * spaceInverse * ushort.MaxValue),
            (byte)System.Math.Round((l.W - min) * spaceInverse * ushort.MaxValue));
    }

    public static Vector4<ushort> FromNormalizedTo16Bits(in this Vector4<float> l, float min = 0, float max = 1)
    {
        float space = max - min;
        if (System.Math.Abs(space) < 0.00001525878)
            throw new ArgumentException($"{nameof(min)} - {nameof(max)} is equal to 0, meaning the space can't be normalized.");
        float spaceInverse = 1f / space;
        return new(
            (byte)MathF.Round((l.X - min) * spaceInverse * ushort.MaxValue),
            (byte)MathF.Round((l.Y - min) * spaceInverse * ushort.MaxValue),
            (byte)MathF.Round((l.Z - min) * spaceInverse * ushort.MaxValue),
            (byte)MathF.Round((l.W - min) * spaceInverse * ushort.MaxValue));
    }
    public static Vector4<byte> FromNormalizedTo8Bits(in this Vector4<double> l, double min = 0, double max = 1)
    {
        double space = max - min;
        if (System.Math.Abs(space) < 0.00001525878)
            throw new ArgumentException($"{nameof(min)} - {nameof(max)} is equal to 0, meaning the space can't be normalized.");
        double spaceInverse = 1d / space;
        return new(
            (byte)System.Math.Round((l.X - min) * spaceInverse * byte.MaxValue),
            (byte)System.Math.Round((l.Y - min) * spaceInverse * byte.MaxValue),
            (byte)System.Math.Round((l.Z - min) * spaceInverse * byte.MaxValue),
            (byte)System.Math.Round((l.W - min) * spaceInverse * byte.MaxValue));
    }

    public static Vector4<byte> FromNormalizedTo8Bits(in this Vector4<float> l, float min = 0, float max = 1)
    {
        float space = max - min;
        if (System.Math.Abs(space) < 0.00001525878)
            throw new ArgumentException($"{nameof(min)} - {nameof(max)} is equal to 0, meaning the space can't be normalized.");
        float spaceInverse = 1f / space;
        return new(
            (byte)MathF.Round((l.X - min) * spaceInverse * byte.MaxValue),
            (byte)MathF.Round((l.Y - min) * spaceInverse * byte.MaxValue),
            (byte)MathF.Round((l.Z - min) * spaceInverse * byte.MaxValue),
            (byte)MathF.Round((l.W - min) * spaceInverse * byte.MaxValue));
    }
    public unsafe static Vector4<byte> FromXYZWToByteVector(this int l) => new(*(byte*)&l, *((byte*)&l + 1), *((byte*)&l + 2), *((byte*)&l + 3));
    public unsafe static Vector4<byte> FromXYZWToByteVector(this uint l) => new(*(byte*)&l, *((byte*)&l + 1), *((byte*)&l + 2), *((byte*)&l + 3));
    public unsafe static Vector4<byte> FromWZYXToByteVector(this int l) => new(*((byte*)&l + 3), *((byte*)&l + 2), *((byte*)&l + 1), *(byte*)&l);
    public unsafe static Vector4<byte> FromWZYXToByteVector(this uint l) => new(*((byte*)&l + 3), *((byte*)&l + 2), *((byte*)&l + 1), *(byte*)&l);
    public unsafe static Vector4<ushort> FromXYZWToUshortVector(this int l) => new(*(ushort*)&l, *((ushort*)&l + 2), *((ushort*)&l + 4), *((ushort*)&l + 6));
    public unsafe static Vector4<ushort> FromXYZWToUshortVector(this uint l) => new(*(ushort*)&l, *((ushort*)&l + 2), *((ushort*)&l + 4), *((ushort*)&l + 6));
    public unsafe static Vector4<ushort> FromWZYXToUshortVector(this int l) => new(*((ushort*)&l + 6), *((ushort*)&l + 4), *((ushort*)&l + 2), *(ushort*)&l);
    public unsafe static Vector4<ushort> FromWZYXToUshortVector(this uint l) => new(*((ushort*)&l + 6), *((ushort*)&l + 4), *((ushort*)&l + 2), *(ushort*)&l);

}