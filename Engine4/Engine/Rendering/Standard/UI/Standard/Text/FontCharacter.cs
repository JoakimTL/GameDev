using Engine.Data.Datatypes;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class FontCharacter {
	public readonly Vector2i PixelStart;
	public readonly Vector2i Size;
	public readonly Vector2i Offset;
	/// <summary>
	/// The number of pixels to advance the cursor after this character. This is affected by kerning.
	/// </summary>
	public readonly int XAdvance;
	public readonly char Character;

	public FontCharacter( char character, int x, int y, int sX, int sY, int oX, int oY, int aX ) {
		this.Character = character;
		this.PixelStart = new( x, y );
		this.Size = new( sX, sY );
		this.Offset = new( oX, oY );
		this.XAdvance = aX;
	}
}
