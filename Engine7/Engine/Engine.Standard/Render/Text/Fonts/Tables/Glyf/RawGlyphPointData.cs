namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public readonly struct RawGlyphPointData( Vector2<int> coordinate, bool onCurve ) {
	public Vector2<int> Coordinate { get; } = coordinate;
	public bool OnCurve { get; } = onCurve;
}
