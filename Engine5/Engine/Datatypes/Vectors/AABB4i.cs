namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct AABB4i
{
    [System.Runtime.InteropServices.FieldOffset(0)]
    private Vector4i _min;
    [System.Runtime.InteropServices.FieldOffset(16)]
    private Vector4i _max;

    public AABB4i(Vector4i a, Vector4i b)
    {
        _min = Vector4i.Min(a, b);
        _max = Vector4i.Max(a, b);
    }

    public AABB4i(Vector4i[] vecs)
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
    public void Add(Vector4i v)
    {
        _min = Vector4i.Min(_min, v);
        _max = Vector4i.Max(_max, v);
    }

    /// <summary>
    /// Resets the AABB
    /// </summary>
    public void Set(Vector4i a, Vector4i b)
    {
        _min = Vector4i.Min(a, b);
        _max = Vector4i.Max(a, b);
    }

    public Vector4i Min => _min;
    public Vector4i Max => _max;
}

