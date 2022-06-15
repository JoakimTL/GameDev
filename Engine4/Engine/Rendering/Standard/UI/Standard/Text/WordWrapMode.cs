namespace Engine.Rendering.Standard.UI.Standard.Text;

public enum WordWrapMode {
	/// <summary>
	/// Never wraps. Line wraps must be controlled by linebreaks
	/// </summary>
	NoWrap,
	/// <summary>
	/// Never wraps. Line wraps must be controlled by linebreaks. Automatically scales text size fit bounds
	/// </summary>
	NoWrapAutoScale,
	/// <summary>
	/// Wraps if a word exceeds the text width, wrapping the entire word to the next line instead of splitting it. If the word is too long for a line, the word will be split.
	/// </summary>
	WrapWords,
	/// <summary>
	/// Keeps all line the same length. Wraps whenever a letter reaches the text width, often cutting words in two.
	/// </summary>
	WrapLetter
}