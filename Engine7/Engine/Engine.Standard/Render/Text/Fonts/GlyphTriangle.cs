using Engine.Shapes;

namespace Engine.Standard.Render.Text.Fonts;

public readonly struct GlyphTriangle( Triangle2<float> triangle, bool filled, bool flipped ) {
	public readonly Triangle2<float> Triangle = triangle;
	public readonly bool Filled = filled;
	public readonly bool Flipped = flipped;
}