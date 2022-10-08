using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class Line {

	private readonly List<LineCharacter> _characters;
	public float Width { get; private set; }
	public float Height { get; private set; }

	public int Count => this._characters.Count;
	public IReadOnlyList<LineCharacter> Characters => this._characters;

	public Line() {
		this._characters = new List<LineCharacter>();
	}

	public void Clear() {
		this._characters.Clear();
		this.Width = 0;
	}

	public void AddCharacter( FontCharacter fontCharacter, float characterWidth, float characterHeight, bool rendered, Color16x4 color, float rotation, Vector2 scale, Vector2 glyphData ) {
		this._characters.Add( new LineCharacter( fontCharacter, characterWidth, rendered, color, rotation, scale, glyphData ) );
		this.Width += characterWidth;
		this.Height = MathF.Max( this.Height, characterHeight );
	}

	public void AddWord( Word word ) {
		this._characters.AddRange( word.Characters );
		this.Width += word.Width;
		this.Height = MathF.Max( this.Height, word.Height );
	}

	public override string ToString() => new( this._characters.Select( p => p.FontCharacter.Character ).ToArray() );
}
