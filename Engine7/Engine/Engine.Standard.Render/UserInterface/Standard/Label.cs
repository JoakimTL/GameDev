using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class Label : UserInterfaceComponentBase {
	private TextLayout _textLayout;

	public string Text {
		get => this._textLayout.Text;
		set => this._textLayout.Text = value;
	}

	public string FontName {
		get => this._textLayout.FontName;
		set => this._textLayout.FontName = value;
	}

	public float TextScale {
		get => this._textLayout.TextScale;
		set => this._textLayout.TextScale = value;
	}

	public Alignment VerticalAlignment {
		get => this._textLayout.VerticalAlignment;
		set => this._textLayout.VerticalAlignment = value;
	}

	public Alignment HorizontalAlignment {
		get => this._textLayout.HorizontalAlignment;
		set => this._textLayout.HorizontalAlignment = value;
	}

	public Vector4<double> Color {
		get => this._textLayout.Color;
		set => this._textLayout.Color = value;
	}

	public Label( UserInterfaceElementBase element ) : base( element ) {
		this._textLayout = this.Element.UserInterfaceServiceAccess.RequestTextLayout( this.RenderLayer );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (this._textLayout.RenderLayer != this.RenderLayer) {
			this._textLayout.Remove();
			TextLayout oldTextLayout = this._textLayout;
			this._textLayout = this.Element.UserInterfaceServiceAccess.RequestTextLayout( this.RenderLayer );
			this._textLayout.Text = oldTextLayout.Text;
			this._textLayout.FontName = oldTextLayout.FontName;
			this._textLayout.TextScale = oldTextLayout.TextScale;
			this._textLayout.Color = oldTextLayout.Color;
			this._textLayout.HorizontalAlignment = oldTextLayout.HorizontalAlignment;
			this._textLayout.VerticalAlignment = oldTextLayout.VerticalAlignment;
			this._textLayout.TextArea = oldTextLayout.TextArea;
			this._textLayout.TextRotation = oldTextLayout.TextRotation;
		}
		this._textLayout.Update( time, deltaTime );
	}

	protected override void OnPlacementChanged() {
		this._textLayout.TextRotation = (float) this.TransformInterface.GlobalRotation;
		this._textLayout.TextArea = AABB.Create(
			[(this.TransformInterface.GlobalTranslation - this.TransformInterface.GlobalScale).CastSaturating<double, float>(),
			(this.TransformInterface.GlobalTranslation + this.TransformInterface.GlobalScale).CastSaturating<double, float>()] );
	}

	protected override void InternalRemove() {
		this._textLayout.Remove();
	}

	protected internal override void DoHide() {
		this._textLayout.Hide();
	}

	protected internal override void DoShow() {
		this._textLayout.Show();
	}
}