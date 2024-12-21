using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public sealed class GlyphContour( ushort startIndex, GlyphContourPoint[] points ) : Identifiable {
	public ushort StartIndex { get; } = startIndex;
	public IReadOnlyList<GlyphContourPoint> Points { get; } = points.AsReadOnly();
	public IReadOnlyList<GlyphContourPoint> OffCurvePoints { get; } = points.Where( p => !p.OnCurve ).ToList().AsReadOnly();
	public bool ContourWindsClockWise => this.Points.Select( p => p.Coordinate ).ToArray().AsSpan().GetSignedArea() < 0;
	public IReadOnlyList<GlyphContour> ContainedWithin => this._containedWithin.AsReadOnly();

	private readonly List<GlyphContour> _containedWithin = [];

	/// <summary>
	/// Checks if a contour contains another. If they intersect there is no containment. If all the points of the other contour are inside this contour, the other contour is contained within this contour.
	/// If there is containment add this contour to the other contour's list of contained within.
	/// </summary>
	public void CheckContainment( GlyphContour other ) {
		AABB<Vector2<int>> myBounds = AABB.Create( this.Points.Select( p => p.Coordinate ).ToArray().AsSpan() );
		AABB<Vector2<int>> otherBounds = AABB.Create( other.Points.Select( p => p.Coordinate ).ToArray().AsSpan() );
		if (!myBounds.Intersects( otherBounds ))
			return;
		Vector2<int>[] points = Points.Select( p => p.Coordinate ).Distinct().ToArray();
		for (int i = 0; i < other.Points.Count; i++)
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (1, 0) )) {
				other._containedWithin.Add( this );
				return;
			}
		for (int i = 0; i < other.Points.Count; i++)
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (0, 1) )) {
				other._containedWithin.Add( this );
				return;
			}
		for (int i = 0; i < other.Points.Count; i++)
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (1, 1) )) {
				other._containedWithin.Add( this );
				return;
			}
	}

	public void CullContainment() {
		HashSet<GlyphContour> containingContours = [];
		foreach (GlyphContour contour in this._containedWithin)
			contour.AddContainingContours( containingContours );
		_containedWithin.RemoveAll( containingContours.Contains );
	}

	private void AddContainingContours( HashSet<GlyphContour> contours ) {
		foreach (GlyphContour contour in _containedWithin)
			if (contours.Add( contour ))
				contour.AddContainingContours( contours );
	}
}
