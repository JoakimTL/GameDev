namespace Engine.Standard.Render.Text.Typesetting;

public sealed class Line {

	private readonly List<Word> _lineWords = [];
	public float ScaledWidth { get; private set; }

	public IReadOnlyList<Word> Words => _lineWords;

	public int Read( List<Word> words, float whitespaceAdvance, float width ) {
		_lineWords.Clear();
		ScaledWidth = 0;

		for (int i = 0; i < words.Count; i++) {
			if (ScaledWidth + words[ i ].ScaledWidth > width && _lineWords.Count > 0) {
				ScaledWidth -= whitespaceAdvance; //Removes last whitespace. There will be no words after the last one, so no whitespace is needed. The whitespace can affect centering and right to left alignment.
				break;
			}
			_lineWords.Add( words[ i ] );
			ScaledWidth += words[ i ].ScaledWidth + whitespaceAdvance;
		}

		return _lineWords.Count;
	}

}
