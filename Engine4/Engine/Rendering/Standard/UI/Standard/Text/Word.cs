using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class Word {

	private readonly List<LineCharacter> _characters;
	public float Width { get; private set; }
	public float Height { get; private set; }
	public IReadOnlyList<LineCharacter> Characters => this._characters;

	public Word() {
		this._characters = new List<LineCharacter>();
	}

	public void Clear() {
		this._characters.Clear();
		this.Width = 0;
	}

	public void AddCharacter( FontCharacter fontCharacter, float characterWidth, float characterHeight, Color16x4 color, float rotation, Vector2 scale, Vector2 glyphData ) {
		this._characters.Add( new LineCharacter( fontCharacter, characterWidth, true, color, rotation, scale, glyphData ) );
		this.Width += characterWidth;
		this.Height = MathF.Max( this.Height, characterHeight );
	}
}
