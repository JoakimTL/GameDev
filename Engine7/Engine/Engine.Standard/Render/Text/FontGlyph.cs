using Engine.Algorithms.Triangulation;

namespace Engine.Standard.Render.Text;

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
public sealed class FontGlyph( FontGlyphHeader header, GlyphMap mapping, (Vector2<int> coordinate, bool onCurve)[] points, ushort[] endPointsOfContours, byte[] instructions, byte[] flags ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html
	public readonly FontGlyphHeader Header = header;
	private readonly GlyphMap _mapping = mapping;
	public uint Unicode => _mapping.Unicode;
	public uint GlyphIndex => _mapping.GlyphIndex;

	private readonly ushort[] _endPointsOfContours = endPointsOfContours;
	private readonly byte[] _instructions = instructions;
	private readonly byte[] _flags = flags;
	private readonly (Vector2<int> coordinate, bool onCurve)[] _points = points;

	public (Triangle2<float>, bool filled)[] CreateMeshTriangles( float scale ) {
		Span<Vector2<int>> points = stackalloc Vector2<int>[ _points.Length ];
		for (int i = 0; i < _points.Length; i++) {
			points[ i ] = _points[ i ].coordinate;
		}

		List<Edge2<int>> enforcedEdges = [];
		AddEnforcedEdges( enforcedEdges );

		var a = Delaunay.ConstrainedTriangulate<int, double>( points, enforcedEdges ).ToArray();
		//var a = EarClipping.Triangulate( points ).ToArray();

		//https://learn.microsoft.com/en-us/typography/opentype/spec/ttch01#outlines
		//The points that make up a curve must be numbered in consecutive order. It makes a difference whether the order is increasing or decreasing in determining the fill pattern of the shapes that make up the glyph. The direction of the curves has to be such that, if the curve is followed in the direction of increasing point numbers, the black space (the filled area) will always be to the right.

		//So let's retian triangles where the triangle has any points is on a filling contour and remove the triangle if it only has points on a non-filling contour.
		Span<bool> displayTriangleA = stackalloc bool[ a.Length ];
		displayTriangleA.Fill( true );
		for (int i = 0; i < _endPointsOfContours.Length; i++) {
			SetDisplayedTriangles( a, points, displayTriangleA, i );
		}

		List<(Triangle2<float>, bool filled)> result = [];

		for (int i = 0; i < displayTriangleA.Length; i++) {
			if (displayTriangleA[ i ])
				result.Add( (new Triangle2<float>( a[ i ].A.CastSaturating<int, float>() * scale, a[ i ].B.CastSaturating<int, float>() * scale, a[ i ].C.CastSaturating<int, float>() * scale ), true) );
		}

		return result.ToArray();
	}

	public void AddEnforcedEdges( List<Edge2<int>> edges ) {
		uint contourStart = 0;
		for (int i = 0; i < _endPointsOfContours.Length; i++) {
			uint contourEnd = _endPointsOfContours[ i ];
			Vector2<int> p2 = _points[ contourEnd ].coordinate;
			for (uint j = contourStart; j <= contourEnd; j++) {
				Vector2<int> p1 = _points[ j ].coordinate;
				edges.Add( new( p1, p2 ) );
				p2 = p1;
			}
			contourStart = contourEnd + 1;
		}
	}

	public (Vector2<float>, uint indexInContour)[] GetPointsInContours() {
		List<(Vector2<float>, uint)> result = [];
		uint contourStart = 0;
		for (int i = 0; i < _endPointsOfContours.Length; i++) {
			uint contourEnd = _endPointsOfContours[ i ];
			for (uint j = contourStart; j <= contourEnd; j++) {
				result.Add( (_points[ j ].coordinate.CastSaturating<int, float>(), j - contourStart) );
			}
			contourStart = contourEnd + 1;
		}
		return result.ToArray();
	}

	private void SetDisplayedTriangles( Triangle2<int>[] triangles, Span<Vector2<int>> points, Span<bool> displayed, int contourIndex ) {
		uint contourStart = contourIndex > 0 ? _endPointsOfContours[ contourIndex - 1 ] : 0u;
		uint contourEnd = _endPointsOfContours[ contourIndex ];

		Span<Vector2<int>> pointInContour = points.Slice( (int) contourStart, (int) (contourEnd - contourStart + 1) );

		if (PolygonExtensions.GetSignedArea( pointInContour ) < 0) {
			Span<int> indicesInTriangle = stackalloc int[ 3 ];
			Span<Vector2<int>> tempTriangle = stackalloc Vector2<int>[ 3 ];
			for (int i = 0; i < triangles.Length; i++) {
				indicesInTriangle[ 0 ] = IndexOf( pointInContour, triangles[ i ].A );
				indicesInTriangle[ 1 ] = IndexOf( pointInContour, triangles[ i ].B );
				indicesInTriangle[ 2 ] = IndexOf( pointInContour, triangles[ i ].C );
				if (indicesInTriangle[ 0 ] >= 0 && indicesInTriangle[ 1 ] >= 0 && indicesInTriangle[ 2 ] >= 0) {
					indicesInTriangle.Sort();
					tempTriangle[ 0 ] = pointInContour[ indicesInTriangle[ 0 ] ];
					tempTriangle[ 1 ] = pointInContour[ indicesInTriangle[ 1 ] ];
					tempTriangle[ 2 ] = pointInContour[ indicesInTriangle[ 2 ] ];
					if (PolygonExtensions.GetSignedArea( tempTriangle ) > 0) {
						displayed[ i ] = false;
					}
				}
			}
			return;
		}

		for (int i = 0; i < triangles.Length; i++) {
			if (triangles[ i ].AllPointsIn( pointInContour ))
				displayed[ i ] = false;
		}
	}

	private int IndexOf( Span<Vector2<int>> span, Vector2<int> p ) {
		for (int i = 0; i < span.Length; i++) {
			if (span[ i ] == p)
				return i;
		}
		return -1;
	}
}
