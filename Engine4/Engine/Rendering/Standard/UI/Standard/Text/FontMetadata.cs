using System.Numerics;
using Engine.Data.Datatypes;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class FontMetadata : Identifiable {
	public Vector2 InverseResolution { get; }

	public Vector2i Max { get; }
	public float InverseMax { get; }
	public float RelativeLineHeight { get; }
	public float InverseBaseline { get; }

	public FontMetadata( Font font ) {
		int maxX = 0;
		foreach ( FontCharacter c in font.Characters.Values ) {
			if ( c.Size.X > maxX )
				maxX = c.Size.X;
		}
		this.Max = (maxX, font.PaddingUpDown.X + font.PaddingUpDown.Y + font.Baseline);
		this.InverseMax = 1f / this.Max.Y;
		this.InverseResolution = Vector2.One / font.Resolution;
		this.RelativeLineHeight = (float) font.LineHeight / font.Baseline;
		this.InverseBaseline = 1f / font.Baseline;
	}
}
