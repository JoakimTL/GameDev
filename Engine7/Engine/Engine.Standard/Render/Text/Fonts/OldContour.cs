using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class OldContour( ushort startIndex, OldContourPoint[] points ) : Identifiable {
	public ushort StartIndex { get; } = startIndex;
	public IReadOnlyList<OldContourPoint> Points { get; } = points.AsReadOnly();
	public IReadOnlyList<OldContourPoint> ImpliedPoints { get; } = points.Where( p => p.Implied ).ToList().AsReadOnly();
	public IReadOnlyList<OldContourPoint> RealPoints { get; } = points.Where( p => !p.Implied ).ToList().AsReadOnly();
	public IReadOnlyList<OldContourPoint> OnCurvePoints { get; } = points.Where( p => p.OnCurve ).ToList().AsReadOnly();
	public IReadOnlyList<OldContourPoint> OffCurvePoints { get; } = points.Where( p => !p.OnCurve ).ToList().AsReadOnly();
	public bool ContourWindsClockWise => this.Points.Select( p => p.Coordinate ).ToArray().AsSpan().GetSignedArea() < 0;
	public IReadOnlyList<OldContour> ContainedWithin => this._containedWithin.AsReadOnly();

	private readonly List<OldContour> _containedWithin = [];

	/// <summary>
	/// Checks if a contour contains another. If they intersect there is no containment. If all the points of the other contour are inside this contour, the other contour is contained within this contour.
	/// If there is containment add this contour to the other contour's list of contained within.
	/// </summary>
	public void CheckContainment( OldContour other ) {
		AABB<Vector2<int>> myBounds = AABB.Create( this.Points.Select( p => p.Coordinate ).ToArray().AsSpan() );
		AABB<Vector2<int>> otherBounds = AABB.Create( other.Points.Select( p => p.Coordinate ).ToArray().AsSpan() );
		if (!myBounds.Intersects( otherBounds ))
			return;
		Vector2<int>[] points = Points.Select( p => p.Coordinate ).Distinct().ToArray();
		Vector2<int> p2 = points[ ^1 ];
		for (int i = 0; i < points.Length; i++) {

		}
		for (int i = 0; i < other.Points.Count; i++) {
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (1, 0) )) {
				other._containedWithin.Add( this );
				return;
			}
		}
		for (int i = 0; i < other.Points.Count; i++) {
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (0, 1) )) {
				other._containedWithin.Add( this );
				return;
			}
		}
		for (int i = 0; i < other.Points.Count; i++) {
			if (other.Points[ i ].Coordinate.PointInPolygon( points, (1, 1) )) {
				other._containedWithin.Add( this );
				return;
			}
		}
	}

	public void CullContainment() {
		HashSet<OldContour> containingContours = [];
		foreach (OldContour contour in this._containedWithin)
			contour.AddContainingContours( containingContours );
		_containedWithin.RemoveAll( containingContours.Contains );
	}

	private void AddContainingContours( HashSet<OldContour> contours ) {
		foreach (OldContour contour in _containedWithin) {
			if (contours.Add( contour ))
				contour.AddContainingContours( contours );
		}
	}
}
