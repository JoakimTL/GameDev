using Engine.Module.Entities.Container;

namespace Engine.Standard.Entities.Components.Rendering;
public sealed class RenderedTextComponent : ComponentBase {

	private string _text = string.Empty;
	public string Text { get => this._text; set => SetText( value ); }

	private string _fontName = string.Empty;
	public string FontName { get => this._fontName; set => SetFontName( value ); }

	private bool _billboardedText = true;
	public bool Billboarded { get => this._billboardedText; set => SetBillboarded( value ); }

	private Alignment _horizontalAlignment = Alignment.Negative;
	public Alignment HorizontalAlignment { get => this._horizontalAlignment; set => SetHorizontalAlignment( value ); }

	private Alignment _verticalAlignment = Alignment.Positive;
	public Alignment VerticalAlignment { get => this._verticalAlignment; set => SetVerticalAlignment( value ); }

	private void SetText( string value ) {
		if (this._text == value)
			return;
		this._text = value;
		InvokeComponentChanged();
	}

	private void SetFontName( string value ) {
		if (this._fontName == value)
			return;
		this._fontName = value;
		InvokeComponentChanged();
	}

	private void SetBillboarded( bool value ) {
		if (this._billboardedText == value)
			return;
		this._billboardedText = value;
		InvokeComponentChanged();
	}

	private void SetHorizontalAlignment( Alignment value ) {
		if (this._horizontalAlignment == value)
			return;
		this._horizontalAlignment = value;
		InvokeComponentChanged();
	}

	private void SetVerticalAlignment( Alignment value ) {
		if (this._verticalAlignment == value)
			return;
		this._verticalAlignment = value;
		InvokeComponentChanged();
	}
}