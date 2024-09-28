namespace Engine.Math.NewFolder.Old;

public readonly struct Bivector2<T>(T xy) where T : System.Numerics.INumber<T>
{
    public readonly T XY = xy;
}