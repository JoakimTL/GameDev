using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public interface IGlyph {
	FontGlyphHeader Header { get; }
	GlyphMap Mapping { get; }
	(Triangle2<float>, bool filled, bool flipped)[] CreateMeshTriangles( float scale, bool useConstraints );
	(Vector2<float>, uint indexInContour, bool onCurve)[] GetPointsInContours();
}
