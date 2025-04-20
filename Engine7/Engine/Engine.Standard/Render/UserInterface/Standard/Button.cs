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

	private bool _stateChanged = false;
	public Action<Button> NoHoverAction { get; set; }
	public Action<Button> HoverAction { get; set; }
	public Action<Button> PressedAction { get; set; }

	public Button( UserInterfaceElementBase element, string text, string fontName, Action<Button> noHoverAction, Action<Button> hoverAction, Action<Button> pressedAction ) : base( element ) {
		_collider = new Collider2Shape();
		_collider.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] );
		_collider.SetTransform( TransformInterface );
		_collision = new Collision2Calculation<double>( _collider, element.UserInterfaceServiceAccess.Get<MouseColliderProvider>().ColliderNDCA );
		Background = new TexturedNineSlicedBackground( this, element.UserInterfaceServiceAccess.Textures.Get( "test" ) );
		Label = new Label( this ) {
			Text = text,
			FontName = fontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		};
		NoHoverAction = noHoverAction;
		HoverAction = hoverAction;
		PressedAction = pressedAction;
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_collision.Evaluate())
			this.LogWarning( "Collision calculation failed." );
		bool wasHovering = _hovering;
		_hovering = _collision.CollisionResult.IsColliding;
		_stateChanged |= wasHovering != _hovering;
		if (_stateChanged) {
			StateChanged();
			_stateChanged = false;
		}
	}

	private void StateChanged() {
		if (_hoveringAtPress) {
			PressedAction?.Invoke( this );
			return;
		}
		if (_hovering) {
			HoverAction?.Invoke( this );
			return;
		}
		NoHoverAction?.Invoke( this );
	}

	protected override void OnPlacementChanged() { }

	protected override bool DoOnMouseButton( MouseButtonEvent @event ) {
		if (@event.InputType == TactileInputType.Press && _hovering) {
			_hoveringAtPress = _hovering;
			_stateChanged = true;
			return true;
		}
		if (@event.InputType == TactileInputType.Release && _hoveringAtPress) {
			if (_hovering)
				ButtonClicked?.Invoke();
			_hoveringAtPress = false;
			_stateChanged = true;
			return true;
		}
		return false;
	}

	protected override bool InternalDispose() => true;
	protected internal override void DoHide() { }
	protected internal override void DoShow() { }
}
