using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class Label : UserInterfaceComponentBase {
	private TextLayout _textLayout;

	public string Text {
		get => _textLayout.Text;
		set => _textLayout.Text = value;
	}

	public string FontName {
		get => _textLayout.FontName;
		set => _textLayout.FontName = value;
	}

	public float TextScale {
		get => _textLayout.TextScale;
		set => _textLayout.TextScale = value;
	}

	public Alignment VerticalAlignment {
		get => _textLayout.VerticalAlignment;
		set => _textLayout.VerticalAlignment = value;
	}

	public Alignment HorizontalAlignment {
		get => _textLayout.HorizontalAlignment;
		set => _textLayout.HorizontalAlignment = value;
	}

	public Vector4<double> Color {
		get => _textLayout.Color;
		set => _textLayout.Color = value;
	}

	public Label( UserInterfaceElementBase element ) : base( element ) {
		_textLayout = Element.UserInterfaceServiceAccess.RequestTextLayout( RenderLayer );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (_textLayout.RenderLayer != RenderLayer) {
			_textLayout.Remove();
			var oldTextLayout = _textLayout;
			_textLayout = Element.UserInterfaceServiceAccess.RequestTextLayout( RenderLayer );
			_textLayout.Text = oldTextLayout.Text;
			_textLayout.FontName = oldTextLayout.FontName;
			_textLayout.TextScale = oldTextLayout.TextScale;
			_textLayout.Color = oldTextLayout.Color;
			_textLayout.HorizontalAlignment = oldTextLayout.HorizontalAlignment;
			_textLayout.VerticalAlignment = oldTextLayout.VerticalAlignment;
			_textLayout.TextArea = oldTextLayout.TextArea;
			_textLayout.TextRotation = oldTextLayout.TextRotation;
		}
		_textLayout.Update( time, deltaTime );
	}

	protected override void OnPlacementChanged() {
		_textLayout.TextRotation = (float) TransformInterface.GlobalRotation;
		_textLayout.TextArea = AABB.Create(
			[(TransformInterface.GlobalTranslation - TransformInterface.GlobalScale).CastSaturating<double, float>(),
			(TransformInterface.GlobalTranslation + TransformInterface.GlobalScale).CastSaturating<double, float>()] );
	}

	protected override bool InternalDispose() => true;

	protected internal override void DoHide() {
		_textLayout.Hide();
	}

	protected internal override void DoShow() {
		_textLayout.Show();
	}
}