using Engine.Logging;
using Engine.Module.Render.Input;
using Engine.Physics;
using Engine.Standard.Render.Input.Services;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class Button : UserInterfaceComponentBase {

	public event Action? ButtonClicked;
	private bool _hovering;
	private bool _hoveringAtPress;

	private readonly Collider2Shape _collider;
	private readonly Collision2Calculation<double> _collision;

	public TexturedNineSlicedBackground Background { get; }

	public Label Label { get; }

	public Vector4<double> DefaultColor { get; set; }
	public Vector4<double> HoverColor { get; set; }
	public Vector4<double> PressedColor { get; set; }

	public Button( UserInterfaceElementBase element, string text, string fontName, Vector4<double> defaultColor, Vector4<double> hoverColor, Vector4<double> pressedColor ) : base( element ) {
		_collider = new Collider2Shape();
		_collider.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] );
		_collider.SetTransform( TransformInterface );
		_collision = new Collision2Calculation<double>( _collider, element.UserInterfaceServiceAccess.Get<MouseColliderProvider>().ColliderNDCA );
		Background = new TexturedNineSlicedBackground( this, element.UserInterfaceServiceAccess.Textures.Get("test") );
		Label = new Label( this ) {
			Text = text,
			FontName = fontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		};
		this.DefaultColor = defaultColor;
		this.HoverColor = hoverColor;
		this.PressedColor = pressedColor;
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_collision.Evaluate())
			this.LogWarning( "Collision calculation failed." );
		_hovering = _collision.CollisionResult.IsColliding;
		var color = _hoveringAtPress
			? PressedColor
			: _hovering
				? HoverColor
				: DefaultColor;

		Background.Color = color;
	}

	protected override void OnPlacementChanged() { }

	protected override bool DoOnMouseButton( MouseButtonEvent @event ) {
		if (@event.InputType == TactileInputType.Press && _hovering) {
			_hoveringAtPress = _hovering;
			return true;
		}
		if (@event.InputType == TactileInputType.Release && _hoveringAtPress) {
			if (_hovering)
				ButtonClicked?.Invoke();
			_hoveringAtPress = false;
			return true;
		}
		return false;
	}

	protected override bool InternalDispose() => true;
}
