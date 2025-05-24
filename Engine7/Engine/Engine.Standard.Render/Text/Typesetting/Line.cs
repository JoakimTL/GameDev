namespace Engine.Standard.Render.Text.Typesetting;

public sealed class Line {

	private readonly List<Word> _lineWords = [];
	public float ScaledWidth { get; private set; }

	public IReadOnlyList<Word> Words => this._lineWords;

	public int Read( List<Word> words, float whitespaceAdvance, float width ) {
		this._lineWords.Clear();
		this.ScaledWidth = 0;

		for (int i = 0; i < words.Count; i++) {
			if (this.ScaledWidth + words[ i ].ScaledWidth > width && this._lineWords.Count > 0)
				break;
			this._lineWords.Add( words[ i ] );
			this.ScaledWidth += words[ i ].ScaledWidth + whitespaceAdvance;
			if (words[ i ].EndCharacter == '\n' || words[ i ].EndCharacter == '\r')
				break;
		}

		this.ScaledWidth -= whitespaceAdvance; //Removes last whitespace. There will be no words after the last one, so no whitespace is needed. The whitespace can affect centering and right to left alignment.

		return this._lineWords.Count;
	}

}
