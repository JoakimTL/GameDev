namespace Engine.Standard.Render.Text.Fonts;

public sealed class ContourPoint( Vector2<int> coordinate, bool onCurve, bool implied, int contourIndex, int pointIndexInContour ) {
	public Vector2<int> Coordinate { get; internal set; } = coordinate;
	public bool OnCurve { get; } = onCurve;
	public bool Implied { get; } = implied;
	public int ContourIndex { get; } = contourIndex;
	public int PointIndexInContour { get; } = pointIndexInContour;
}
