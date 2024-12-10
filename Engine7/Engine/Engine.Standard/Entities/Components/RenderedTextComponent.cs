using Engine.Module.Entities.Container;

namespace Engine.Standard.Entities.Components;
public sealed class RenderedTextComponent : ComponentBase {

	private string _text = string.Empty;
	public string Text { get => this._text; set => SetText(value); }

	private string _fontName = string.Empty;
	public string FontName { get => this._fontName; set => SetFontName( value ); }

	private void SetText( string value ) {
		if (this._text == value)
			return;
		this._text = value;
		this.InvokeComponentChanged();
	}

	private void SetFontName( string value ) {
		if (this._fontName == value)
			return;
		this._fontName = value;
		this.InvokeComponentChanged();
	}
}
