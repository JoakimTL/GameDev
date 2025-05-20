using Engine.Algorithms.Triangulation;
using Engine.Shapes;
using Engine.Standard.Render.Text.Fonts.Tables.Glyf;

namespace Engine.Standard.Render.Text.Fonts;

public sealed class FontElementalGlyph : IGlyph {

	private readonly ElementalGlyphData _glyphData;

	public FontElementalGlyph( ElementalGlyphData glyphData, Matrix2x2<float> transformationMatrix, Vector2<float> offset ) {
		_glyphData = glyphData;
		this.TransformationMatrix = transformationMatrix;
		this.Offset = offset;
	}

	public IGlyphData GlyphData => _glyphData;
	public Matrix2x2<float> TransformationMatrix { get; }
	public Vector2<float> Offset { get; }

	public GlyphTriangle[] TriangulateGlyph() {
		List<Vector2<int>> pointList = GetAllPoints();
		Span<Vector2<int>> pointSpan = stackalloc Vector2<int>[ pointList.Count ];
		for (int i = 0; i < pointList.Count; i++)
			pointSpan[ i ] = pointList[ i ];

		List<Edge2<int>> enforcedEdges = CreateEnforcedEdges();

		Delaunator<int, double> triangulation = new( pointSpan, false );
		while (!triangulation.Process())
			;

		TriangulationConstrainer<int, double> constrainer = triangulation.CreateConstrainer( enforcedEdges.ToArray() );
		while (!constrainer.Process())
			;

		Span<bool> triangleDisplay = stackalloc bool[ constrainer.Triangles.Count ];
		for (int i = 0; i < constrainer.Triangles.Count; i++)
			triangleDisplay[ i ] = ShouldDisplayTriangle( constrainer.Triangles[ i ] );

		List<GlyphTriangle> result = [];

		for (int i = 0; i < triangleDisplay.Length; i++)
			if (triangleDisplay[ i ])
				result.Add( new( CreateTriangle( constrainer.Triangles[ i ].A, constrainer.Triangles[ i ].B, constrainer.Triangles[ i ].C, out _ ), true, false ) );

		AddOffCurveTriangles( result );

		TransformTriangles( result );

		return [ .. result ];
	}

	private List<Vector2<int>> GetAllPoints() {
		List<Vector2<int>> pointList = [];
		foreach (GlyphContour contour in this._glyphData.Contours)
			for (int i = 0; i < contour.Points.Count; i++) {
				GlyphContourPoint p = contour.Points[ i ];
				if (p.OnCurve) {
					pointList.Add( p.Coordinate );
					continue;
				}

				GlyphContourPoint pBefore = contour.Points[ i == 0 ? contour.Points.Count - 1 : i - 1 ];
				if (p.Coordinate == pBefore.Coordinate)
					continue;
				GlyphContourPoint pAfter = contour.Points[ (i + 1) % contour.Points.Count ];
				if (pBefore.Coordinate == pAfter.Coordinate)
					continue;

				if (new Edge2<int>( pBefore.Coordinate, pAfter.Coordinate ).Orientation( p.Coordinate ) > 0)
					pointList.Add( p.Coordinate );
			}
		return pointList;
	}

	private List<Edge2<int>> CreateEnforcedEdges() {
		List<Edge2<int>> edges = [];
		foreach (GlyphContour contour in this._glyphData.Contours) {
			if (contour.Points.Count < 3)
				continue;
			for (int i = 0; i < contour.Points.Count; i++) {
				GlyphContourPoint p = contour.Points[ i ];
				GlyphContourPoint pAfter = contour.Points[ (i + 1) % contour.Points.Count ];
				if (p.Coordinate == pAfter.Coordinate)
					continue;

				if (p.OnCurve) {
					if (pAfter.OnCurve)
						AddEdge( edges, new( p.Coordinate, pAfter.Coordinate ) );
					continue;
				}

				GlyphContourPoint pBefore = contour.Points[ i == 0 ? contour.Points.Count - 1 : i - 1 ];
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

		static void AddEdge( List<Edge2<int>> edges, Edge2<int> edge ) {
			if (edges.Contains( edge ))
				return;
			edges.Add( edge );
		}
	}

	private bool ShouldDisplayTriangle( Triangle2<int> triangle ) {
		GlyphContourPoint[] points = [ GetPoint( triangle.A ), GetPoint( triangle.B ), GetPoint( triangle.C ) ];
		GlyphContour[] contours = [ .. points.Select( p => p.ContourIndex ).Distinct().Select( p => this._glyphData.Contours[ p ] ) ];
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

					GlyphContour containingContour;
					GlyphContour containedContour;
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

		GlyphContourPoint GetPoint( Vector2<int> point ) { //TODO Tagged triangulation?
			foreach (GlyphContourPoint p in this._glyphData.AllContourPoints)
				if (p.Coordinate == point)
					return p;
			throw new InvalidOperationException( "Point not found" );
		}

		void SortPointsInContour( GlyphContourPoint[] points, Span<Vector2<int>> trianglePoints ) {
			Span<(int, int)> sortingSpan = [ (points[ 0 ].PointIndexInContour, 0), (points[ 1 ].PointIndexInContour, 1), (points[ 2 ].PointIndexInContour, 2) ];
			sortingSpan.Sort( ( a, b ) => a.Item1 - b.Item1 );
			Span<Vector2<int>> preSortedTrianglePoints = stackalloc Vector2<int>[ 3 ];
			trianglePoints.CopyTo( preSortedTrianglePoints );
			for (int i = 0; i < 3; i++)
				trianglePoints[ i ] = preSortedTrianglePoints[ sortingSpan[ i ].Item2 ];
		}
	}

	private static Triangle2<float> CreateTriangle( Vector2<int> a, Vector2<int> b, Vector2<int> c, out bool flipped ) {
		flipped = new Edge2<int>( a, b ).Orientation( c ) > 0;
		Vector2<float> af = a.CastSaturating<int, float>();
		Vector2<float> bf = b.CastSaturating<int, float>();
		Vector2<float> cf = c.CastSaturating<int, float>();
		return flipped
			 ? new( cf, bf, af )
			 : new( af, bf, cf );
	}

	private void AddOffCurveTriangles( List<GlyphTriangle> result ) {
		foreach (GlyphContour contour in this._glyphData.Contours)
			for (int i = 0; i < contour.OffCurvePoints.Count; i++) {
				//Off curve points must be after an on curve point and followed by an on curve point.
				GlyphContourPoint offCurvePoint = contour.OffCurvePoints[ i ];
				GlyphContourPoint onCurvePointBefore = contour.Points[ offCurvePoint.PointIndexInContour == 0 ? contour.Points.Count - 1 : offCurvePoint.PointIndexInContour - 1 ];
				GlyphContourPoint onCurvePointAfter = contour.Points[ offCurvePoint.PointIndexInContour == contour.Points.Count - 1 ? 0 : offCurvePoint.PointIndexInContour + 1 ];
				Triangle2<float> triangle = CreateTriangle( onCurvePointBefore.Coordinate, offCurvePoint.Coordinate, onCurvePointAfter.Coordinate, out bool flipped );
				result.Add( new( triangle, false, flipped ) );
			}
	}

	private void TransformTriangles( List<GlyphTriangle> glyphTriangles ) {
		for (int i = 0; i < glyphTriangles.Count; i++) {
			GlyphTriangle glyphTriangle = glyphTriangles[ i ];

			Triangle2<float> transformedTriangle = new(
				(glyphTriangle.Triangle.A + this.Offset) * this.TransformationMatrix,
				(glyphTriangle.Triangle.B + this.Offset) * this.TransformationMatrix,
				(glyphTriangle.Triangle.C + this.Offset) * this.TransformationMatrix
			);
			GlyphTriangle transformedGlyphTriangle = new( transformedTriangle, glyphTriangle.Filled, glyphTriangle.Flipped );

			glyphTriangles[ i ] = transformedGlyphTriangle;
		}
	}
}
