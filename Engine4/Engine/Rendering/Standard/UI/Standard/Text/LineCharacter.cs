using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class LineCharacter {
	public FontCharacter FontCharacter { get; }
	public float Width { get; }
	public bool Rendered { get; }
	public Color16x4 Color { get; }
	public float Rotation { get; }
	public Vector2 Scale { get; }
	public Vector2 GlyphData { get; }

	public LineCharacter( FontCharacter character, float width, bool rendered, Color16x4 color, float rotation, Vector2 scale, Vector2 glyphData ) {
		this.FontCharacter = character;
		this.Width = width;
		this.Rendered = rendered;
		this.Color = color;
		this.Rotation = rotation;
		this.Scale = scale;
		this.GlyphData = glyphData;
	}
}
