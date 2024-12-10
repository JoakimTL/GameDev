using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class Contour( ushort startIndex, ContourPoint[] points ) : Identifiable {
	public ushort StartIndex { get; } = startIndex;
	public IReadOnlyList<ContourPoint> Points { get; } = points.AsReadOnly();
	public IReadOnlyList<ContourPoint> ImpliedPoints { get; } = points.Where( p => p.Implied ).ToList().AsReadOnly();
	public IReadOnlyList<ContourPoint> RealPoints { get; } = points.Where( p => !p.Implied ).ToList().AsReadOnly();
	public IReadOnlyList<ContourPoint> OnCurvePoints { get; } = points.Where( p => p.OnCurve ).ToList().AsReadOnly();
	public IReadOnlyList<ContourPoint> OffCurvePoints { get; } = points.Where( p => !p.OnCurve ).ToList().AsReadOnly();
	public bool ContourWindsClockWise => this.Points.Select( p => p.Coordinate ).ToArray().AsSpan().GetSignedArea() < 0;
	public IReadOnlyList<Contour> ContainedWithin => this._containedWithin.AsReadOnly();

	private readonly List<Contour> _containedWithin = [];

	/// <summary>
	/// Checks if a contour contains another. If they intersect there is no containment. If all the points of the other contour are inside this contour, the other contour is contained within this contour.
	/// If there is containment add this contour to the other contour's list of contained within.
	/// </summary>
	public void CheckContainment( Contour other ) {
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
		HashSet<Contour> containingContours = [];
		foreach (Contour contour in this._containedWithin)
			contour.AddContainingContours( containingContours );
		_containedWithin.RemoveAll( containingContours.Contains );
	}

	private void AddContainingContours( HashSet<Contour> contours ) {
		foreach (Contour contour in _containedWithin) {
			if (contours.Add( contour ))
				contour.AddContainingContours( contours );
		}
	}
}
