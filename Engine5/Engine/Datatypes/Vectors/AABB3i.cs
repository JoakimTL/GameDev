﻿namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct AABB3i
{
    [System.Runtime.InteropServices.FieldOffset(0)]
    private Vector3i _min;
    [System.Runtime.InteropServices.FieldOffset(12)]
    private Vector3i _max;

    public uint Volume => (uint)((_max.X - _min.X) * (_max.Y - _min.Y) * (_max.Z - _min.Z));

    public AABB3i(Vector3i a, Vector3i b)
    {
        _min = Vector3i.Min(a, b);
        _max = Vector3i.Max(a, b);
    }

    public AABB3i(Vector3i[] vecs)
    {
        if (vecs.Length == 0)
            throw new ArgumentException($"Length of {nameof(vecs)} must be greater than zero!");
        _min = _max = vecs[0];
        for (int i = 1; i < vecs.Length; i++)
            Add(vecs[i]);
    }

    /// <summary>
    /// Extends the AABB
    /// </summary>
    public void Add(Vector3i v)
    {
        _min = Vector3i.Min(_min, v);
        _max = Vector3i.Max(_max, v);
    }

    /// <summary>
    /// Resets the AABB
    /// </summary>
    public void Set(Vector3i a, Vector3i b)
    {
        _min = Vector3i.Min(a, b);
        _max = Vector3i.Max(a, b);
    }

    public static bool Intersects(ref AABB3i a, ref AABB3i b) =>
        a._min.X <= b._max.X && a._max.X >= b._min.X &&
        a._min.Y <= b._max.Y && a._max.Y >= b._min.Y &&
        a._min.Z <= b._max.Z && a._max.Z >= b._min.Z;

    public static bool Inside(ref AABB3i a, ref Vector3i b) =>
        a._min.X <= b.X && a._max.X >= b.X &&
        a._min.Y <= b.Y && a._max.Y >= b.Y &&
        a._min.Z <= b.Z && a._max.Z >= b.Z;

    public static AABB3i GetLargestVolume(AABB3i a, AABB3i b) => new(Vector3i.Min(a.Min, b.Min), Vector3i.Max(a.Max, b.Max));
    public static AABB3i GetSmallestVolume(AABB3i a, AABB3i b) => new(Vector3i.Max(a.Min, b.Min), Vector3i.Min(a.Max, b.Max));

    public Vector3i Min => _min;
    public Vector3i Max => _max;

    public override string ToString() => $"{Min} -> {Max}";
}
