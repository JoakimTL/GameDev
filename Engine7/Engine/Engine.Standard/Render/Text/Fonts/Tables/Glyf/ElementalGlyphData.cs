using Engine.Logging;

namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public sealed class ElementalGlyphData : IGlyphData {
	public FontGlyphHeader Header { get; }
	public GlyphMap Mapping { get; }
	public IReadOnlyList<GlyphContourPoint> AllContourPoints { get; }
	public IReadOnlyList<GlyphContour> Contours { get; }

	public ElementalGlyphData( FontGlyphHeader header, GlyphMap glyphMap, RawGlyphPointData[] points, ushort[] endPointsOfContours ) {
		this.Header = header;
		this.Mapping = glyphMap;
		this.Contours = CreateContours( endPointsOfContours, points, out List<GlyphContourPoint> allPoints ).AsReadOnly();
		this.AllContourPoints = allPoints.AsReadOnly();
	}

	private List<GlyphContour> CreateContours( ushort[] endPointsOfContours, RawGlyphPointData[] points, out List<GlyphContourPoint> allPoints ) {
		List<GlyphContour> result = [];
		List<GlyphContourPoint> contourPoints = [];
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
				if (j > contourStart && !points[ j - 1 ].OnCurve && !points[ j ].OnCurve) {
					GlyphContourPoint impliedPoint = new( (points[ j - 1 ].Coordinate + points[ j ].Coordinate) / 2, true, true, i, pointIndex++ );
					contourPoints.Add( impliedPoint );
					allPoints.Add( impliedPoint );
				}

				{
					GlyphContourPoint point = new( points[ j ].Coordinate, points[ j ].OnCurve, false, i, pointIndex++ );
					contourPoints.Add( point );
					allPoints.Add( point );
				}
			}
			result.Add( new GlyphContour( contourStart, contourPoints.ToArray() ) );
		}

		//if (Mapping.Unicode == 48)
		//	this.Breakpoint();
		for (int i = 0; i < result.Count; i++)
			for (int j = 0; j < result.Count; j++)
				if (i != j)
					result[ i ].CheckContainment( result[ j ] );
		for (int i = 0; i < result.Count; i++)
			result[ i ].CullContainment();
		return result;
	}
}
