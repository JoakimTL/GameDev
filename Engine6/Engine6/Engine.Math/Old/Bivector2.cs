namespace Engine.Math.Old;

public readonly struct Bivector2<T>(T xy) where T : System.Numerics.INumberBase<T>
{
    public readonly T XY = xy;
}