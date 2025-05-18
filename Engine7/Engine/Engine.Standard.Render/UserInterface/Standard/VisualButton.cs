namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class VisualButton : UserInterfaceComponentBase {
	public TexturedNineSlicedBackground Background { get; }
	public Label Label { get; }

	public VisualButton( UserInterfaceElementBase element, string text, string fontName ) : base( element ) {
		Background = AddChild( new TexturedNineSlicedBackground( element, element.UserInterfaceServiceAccess.Textures.Get( "test" ) ) );
		Label = AddChild( new Label( element ) {
			Text = text,
			FontName = fontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		} );
	}

	protected override void OnUpdate( double time, double deltaTime ) { }
	protected override void OnPlacementChanged() { }
	protected internal override void DoHide() { }
	protected internal override void DoShow() { }
}
