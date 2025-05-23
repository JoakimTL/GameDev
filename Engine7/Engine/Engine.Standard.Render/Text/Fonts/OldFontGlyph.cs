﻿using Engine.Algorithms.Triangulation;
using Engine.Logging;
using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

//public class GlyphData {
//	public uint UnicodeValue;
//	public uint GlyphIndex;
//	public Point[] Points;
//	public int[] ContourEndIndices;
//	public int AdvanceWidth;
//	public int LeftSideBearing;

//	public int MinX;
//	public int MaxX;
//	public int MinY;
//	public int MaxY;

//	public int Width => MaxX - MinX;
//	public int Height => MaxY - MinY;

//}
public sealed class OldFontGlyph : Identifiable, IOldGlyph {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html
	public FontGlyphHeader Header { get; }
	public GlyphMap Mapping { get; }
	public ushort UnitsPerEm { get; }
	public float Scale { get; }

	private readonly ushort[] _originalEndPointsOfContours;
	private readonly (Vector2<int> coordinate, bool onCurve)[] _originalPoints;

	private readonly List<OldContour> _contours;
	private readonly List<OldContourPoint> _allPoints;

	public IReadOnlyList<ushort> EndPointsOfContours => this._originalEndPointsOfContours;
	public IReadOnlyList<(Vector2<int> coordinate, bool onCurve)> Points => this._originalPoints;

	public OldFontGlyph( FontGlyphHeader header, GlyphMap mapping, (Vector2<int> coordinate, bool onCurve)[] points, ushort[] endPointsOfContours, ushort unitsPerEm ) {
		this.Nickname = $"[{(char) mapping.Unicode},{mapping.Unicode}]";
		this.Header = header;
		this.Mapping = mapping;
		this._originalEndPointsOfContours = endPointsOfContours;
		this.UnitsPerEm = unitsPerEm;
		this.Scale = 1f / unitsPerEm;
		this._originalPoints = points;

		this._contours = CreateContours( endPointsOfContours, points, out this._allPoints );
	}

	private List<OldContour> CreateContours( ushort[] endPointsOfContours, (Vector2<int> coordinate, bool onCurve)[] points, out List<OldContourPoint> allPoints ) {
		List<OldContour> result = [];
		List<OldContourPoint> contourPoints = [];
		allPoints = [];

		for (int i = 0; i < endPointsOfContours.Length; i++) {
			contourPoints.Clear();

			ushort contourStart = (ushort) (i == 0 ? 0u : endPointsOfContours[ i - 1 ] + 1u);
			uint contourEnd = endPointsOfContours[ i ];
			int pointIndex = 0;

			if (contourEnd - contourStart < 3) {
				this.LogLine( $"Contour {i} has less than 3 points.", Log.Level.VERBOSE );
				continue;
			}

			for (uint j = contourStart; j <= contourEnd; j++) {
				if (j > contourStart && !points[ j - 1 ].onCurve && !points[ j ].onCurve) {
					OldContourPoint impliedPoint = new( (points[ j - 1 ].coordinate + points[ j ].coordinate) / 2, true, true, i, pointIndex++ );
					contourPoints.Add( impliedPoint );
					allPoints.Add( impliedPoint );
				}

				{
					OldContourPoint point = new( points[ j ].coordinate, points[ j ].onCurve, false, i, pointIndex++ );
					contourPoints.Add( point );
					allPoints.Add( point );
				}
			}
			result.Add( new OldContour( contourStart, [ .. contourPoints ] ) );
		}

		//if (Mapping.Unicode == 48)
		//	this.Breakpoint();
		for (int i = 0; i < result.Count; i++)
			for (int j = 0; j < result.Count; j++)
				if (i != j)
					result[ i ].CheckContainment( result[ j ] );
		for (int i = 0; i < result.Count; i++) {
			result[ i ].CullContainment();
		}
		return result;
	}

	public GlyphTriangle[] TriangulateGlyph() {
		List<Vector2<int>> pointList = GetAllPoints();
		Span<Vector2<int>> points = stackalloc Vector2<int>[ pointList.Count ];
		for (int i = 0; i < pointList.Count; i++)
			points[ i ] = pointList[ i ];

		List<Edge2<int>> enforcedEdges = CreateEnforcedEdges();

		Delaunator<int, double> triangulation = new( points, false );
		while (!triangulation.Process())
			;

		TriangulationConstrainer<int, double> constrainer = triangulation.CreateConstrainer( enforcedEdges.ToArray() );
		while (!constrainer.Process())
			;

		//https://learn.microsoft.com/en-us/typography/opentype/spec/ttch01#outlines

		Span<bool> triangleDisplay = stackalloc bool[ constrainer.Triangles.Count ];
		for (int i = 0; i < constrainer.Triangles.Count; i++)
			triangleDisplay[ i ] = ShouldDisplayTriangle( constrainer.Triangles[ i ] );

		List<GlyphTriangle> result = [];

		for (int i = 0; i < triangleDisplay.Length; i++)
			if (triangleDisplay[ i ])
				result.Add( new( CreateTriangle( constrainer.Triangles[ i ].A, constrainer.Triangles[ i ].B, constrainer.Triangles[ i ].C, out _ ), true, false ) );

		AddOffCurveTriangles( result );

		return [ .. result ];
	}

	private Triangle2<float> CreateTriangle( Vector2<int> a, Vector2<int> b, Vector2<int> c, out bool flipped ) {
		flipped = new Edge2<int>( a, b ).Orientation( c ) > 0;
		if (flipped)
			return new( c.CastSaturating<int, float>() * this.Scale, b.CastSaturating<int, float>() * this.Scale, a.CastSaturating<int, float>() * this.Scale );
		return new( a.CastSaturating<int, float>() * this.Scale, b.CastSaturating<int, float>() * this.Scale, c.CastSaturating<int, float>() * this.Scale );
	}


	private void AddOffCurveTriangles( List<GlyphTriangle> result ) {
		foreach (OldContour contour in this._contours)
			for (int i = 0; i < contour.OffCurvePoints.Count; i++) {
				//Off curve points must be after an on curve point and followed by an on curve point.
				OldContourPoint offCurvePoint = contour.OffCurvePoints[ i ];
				OldContourPoint onCurvePointBefore = contour.Points[ offCurvePoint.PointIndexInContour == 0 ? contour.Points.Count - 1 : offCurvePoint.PointIndexInContour - 1 ];
				OldContourPoint onCurvePointAfter = contour.Points[ offCurvePoint.PointIndexInContour == contour.Points.Count - 1 ? 0 : offCurvePoint.PointIndexInContour + 1 ];
				Triangle2<float> triangle = CreateTriangle( onCurvePointBefore.Coordinate, offCurvePoint.Coordinate, onCurvePointAfter.Coordinate, out bool flipped );
				result.Add( new( triangle, false, flipped ) );
			}
	}

	public List<Vector2<int>> GetAllPoints() {
		List<Vector2<int>> pointList = [];
		foreach (OldContour contour in this._contours)
			for (int i = 0; i < contour.Points.Count; i++) {
				OldContourPoint p = contour.Points[ i ];
				if (p.OnCurve) {
					pointList.Add( p.Coordinate );
					continue;
				}

				OldContourPoint pBefore = contour.Points[ i == 0 ? contour.Points.Count - 1 : i - 1 ];
				if (p.Coordinate == pBefore.Coordinate)
					continue;
				OldContourPoint pAfter = contour.Points[ (i + 1) % contour.Points.Count ];
				if (pBefore.Coordinate == pAfter.Coordinate)
					continue;

				if (new Edge2<int>( pBefore.Coordinate, pAfter.Coordinate ).Orientation( p.Coordinate ) > 0)
					pointList.Add( p.Coordinate );
			}
		return pointList;
	}

	public List<Edge2<int>> CreateEnforcedEdges() {
		List<Edge2<int>> edges = [];
		foreach (OldContour contour in this._contours) {
			if (contour.Points.Count < 3)
				continue;
			for (int i = 0; i < contour.Points.Count; i++) {
				OldContourPoint p = contour.Points[ i ];
				OldContourPoint pAfter = contour.Points[ (i + 1) % contour.Points.Count ];
				if (p.Coordinate == pAfter.Coordinate)
					continue;

				if (p.OnCurve) {
					if (pAfter.OnCurve)
						AddEdge( edges, new( p.Coordinate, pAfter.Coordinate ) );
					continue;
				}

				OldContourPoint pBefore = contour.Points[ i == 0 ? contour.Points.Count - 1 : i - 1 ];
				if (p.Coordinate == pBefore.Coordinate)
					continue;

				if (new Edge2<int>( pBefore.Coordinate, pAfter.Coordinate ).Orientation( p.Coordinate ) > 0) {
					AddEdge( edges, new( pBefore.Coordinate, p.Coordinate ) );
					AddEdge( edges, new( p.Coordinate, pAfter.Coordinate ) );
				} else if (pBefore.Coordinate != pAfter.Coordinate)
					AddEdge( edges, new( pBefore.Coordinate, pAfter.Coordinate ) );
			}
		}
		return edges;
	}

	private void AddEdge( List<Edge2<int>> edges, Edge2<int> edge ) {
		if (edges.Contains( edge ))
			return;
		edges.Add( edge );
	}

	public (Vector2<float>, uint indexInContour, bool onCurve)[] GetPointsInContours() {
		List<(Vector2<float>, uint, bool)> result = [];
		foreach (OldContour contour in this._contours)
			for (int i = 0; i < contour.Points.Count; i++)
				result.Add( (contour.Points[ i ].Coordinate.CastSaturating<int, float>(), (uint) i, contour.Points[ i ].OnCurve) );
		return [ .. result ];
	}

	private bool ShouldDisplayTriangle( Triangle2<int> triangle ) {
		OldContourPoint[] points = [ GetPoint( triangle.A ), GetPoint( triangle.B ), GetPoint( triangle.C ) ];
		OldContour[] contours = [ .. points.Select( p => p.ContourIndex ).Distinct().Select( p => this._contours[ p ] ) ];
		int windingSum = 0;
		for (int i = 0; i < contours.Length; i++)
			windingSum += contours[ i ].ContourWindsClockWise ? 1 : -1;
		switch (contours.Length) {
			case 1: {
					//The triangle is connected to a single contour. We have 3 cases:
					//1. The contour is winding clockwise and the triangle is winding counter clockwise. The triangle is filled. (We're inside of a filling contour)
					//2. The contour is winding counter clockwise and the triangle is winding clockwise. The triangle is not filled. (We're inside of a hole)
					//3. The contour is winding clockwise and the triangle is winding clockwise. The triangle is not filled. (We're outside of a filling contour)
					if (windingSum == 1) {
						Span<Vector2<int>> trianglePoints = [ triangle.A, triangle.B, triangle.C ];
						SortPointsInContour( points, trianglePoints );
						return trianglePoints.GetSignedArea() < 0; //This might need sorting!
					}
					return false;
				}
			case 2: {
					//The triangle is connected to two distinct contours. We have 4 cases:
					//1.  The contours both wind clockwise. The triangle is not filled. (We're outside of two filling contours)
					//2.  The contours both wind counter clockwise. The triangle is filled. (We're inside of a filling contour, but our vertices are connected to holes)
					//3.1 1 contour is winding clockwise and 1 is winding counter clockwise. The clockwise contour contains the counter clockwise contour. The triangle is filled. (We're inside of a filling contour)
					//3.2 1 contour is winding clockwise and 1 is winding counter clockwise. The counter clockwise contour contains the clockwise contour. The triangle is not filled. (We're inside of a hole)
					if (windingSum == 2)
						return false;
					if (windingSum == -2)
						return true;

					//We'll not deal with 3.2. Let's hope this case doesn't happen when we don't have compound glyphs in one big glyph.
					//What if we find an edge on the hole, and check if the edge on the fill is on the "right" side compared to the edge on the hole?

					OldContour containingContour;
					OldContour containedContour;
					if (contours[ 0 ].ContainedWithin.Contains( contours[ 1 ] )) {
						containedContour = contours[ 0 ];
						containingContour = contours[ 1 ];
					} else if (contours[ 1 ].ContainedWithin.Contains( contours[ 0 ] )) {
						containedContour = contours[ 1 ];
						containingContour = contours[ 0 ];
					} else
						throw new InvalidOperationException( "We have a hole, but it's not contained within another contour." );

					if (containingContour.ContourWindsClockWise)
						return true;
					else
						return false;
				}
			case 3: {
					//The triangle is connected to three distinct contours. We have 2 cases:
					//1. 2 contours are winding counter clockwise and 1 is winding clockwise. The triangle is filled. (We're inside of a filling contour and between two holes)
					return windingSum == -1;
				}
			default:
				throw new InvalidOperationException();
		}
		//Cases:
		// 1 distinct contour: 3 cases
		// 1.1: The contour is winding clockwise and the triangle is winding counter clockwise. The triangle is filled. (We're inside of a filling contour)
		// 1.2: The contour is winding counter clockwise and the triangle is winding clockwise. The triangle is not filled. (We're inside of a hole)
		// 1.3: The contour is winding clockwise and the triangle is winding clockwise. The triangle is not filled. (We're outside of a filling contour)
		// 2 distinct contours: 3 cases
		// 2.1: The contours both wind clockwise. The triangle is not filled. (We're outside of two filling contours)
		// 2.2: The contours both wind counter clockwise. The triangle is filled. (We're inside of a filling contour, but our vertices are connected to holes)
		// 2.3: 1 contour is winding clockwise and 1 is winding counter clockwise. The triangle is filled. (We're inside of a filling contour)
		// 3 distinct contours: 1 case
		// 3.1: 2 contours are winding counter clockwise and 1 is winding clockwise. The triangle is filled. (We're inside of a filling contour and between two holes)
	}

	private void SortPointsInContour( OldContourPoint[] points, Span<Vector2<int>> trianglePoints ) {
		Span<(int, int)> sortingSpan = [ (points[ 0 ].PointIndexInContour, 0), (points[ 1 ].PointIndexInContour, 1), (points[ 2 ].PointIndexInContour, 2) ];
		sortingSpan.Sort( ( a, b ) => a.Item1 - b.Item1 );
		Span<Vector2<int>> preSortedTrianglePoints = stackalloc Vector2<int>[ 3 ];
		trianglePoints.CopyTo( preSortedTrianglePoints );
		for (int i = 0; i < 3; i++)
			trianglePoints[ i ] = preSortedTrianglePoints[ sortingSpan[ i ].Item2 ];
	}

	private int IndexOf( Span<Vector2<int>> span, Vector2<int> p ) {
		for (int i = 0; i < span.Length; i++)
			if (span[ i ] == p)
				return i;
		return -1;
	}

	public OldContourPoint GetPoint( Vector2<int> point ) {
		foreach (OldContourPoint p in this._allPoints)
			if (p.Coordinate == point)
				return p;
		throw new InvalidOperationException( "Point not found" );
	}

	internal void Transform( double offsetX, double offsetY, double iHat_x, double iHat_y, double jHat_x, double jHat_y ) {
		for (int i = 0; i < this._allPoints.Count; i++) {
			(double xPrime, double yPrime) = TransformPoint( this._allPoints[ i ].Coordinate.X, this._allPoints[ i ].Coordinate.Y );
			//_originalPoints[ i ].coordinate = new( (int) xPrime, (int) yPrime );
			this._allPoints[ i ].Coordinate = new( (int) xPrime, (int) yPrime );
		}

		(double xPrime, double yPrime) TransformPoint( double x, double y ) {
			double xPrime = (iHat_x * x) + (jHat_x * y) + offsetX;
			double yPrime = (iHat_y * x) + (jHat_y * y) + offsetY;
			return (xPrime, yPrime);
		}
	}
}
