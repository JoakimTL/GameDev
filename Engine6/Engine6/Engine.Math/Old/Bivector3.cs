namespace Engine.Math.Old;

public readonly struct Bivector3<T>(T yz, T zx, T xy) where T : System.Numerics.INumberBase<T>
{
    public readonly T YZ = yz;
    public readonly T ZX = zx;
    public readonly T XY = xy;

    public Vector3<T> AsVector => new(YZ, ZX, XY);
}
