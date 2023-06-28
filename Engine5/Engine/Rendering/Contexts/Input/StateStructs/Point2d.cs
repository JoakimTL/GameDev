using System.Numerics;

namespace Engine.Rendering.Contexts.Input.StateStructs;

public readonly struct Point2d
{
    public readonly double X;
    public readonly double Y;

    public Point2d(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Vector2 GetDeltaVector(Point2d other) => new((float)(X - other.X), (float)(Y - other.Y));

    public override string ToString() => $"({X:#.####}, {Y:#.####})";
}
