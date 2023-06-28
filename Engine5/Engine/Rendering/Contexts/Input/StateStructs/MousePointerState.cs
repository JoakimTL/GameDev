namespace Engine.Rendering.Contexts.Input.StateStructs;

public readonly struct MousePointerState
{
    public readonly Point2d PixelPosition;
    public readonly Point2d NdcaPosition;

    public MousePointerState(Point2d pixelPosition, Point2d ndcaPosition)
    {
        PixelPosition = pixelPosition;
        NdcaPosition = ndcaPosition;
    }

    public override string ToString() => $"Pixel: {PixelPosition} Ndca: {NdcaPosition}";
}
