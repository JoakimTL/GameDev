using Engine.Logging;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Physics;
using Engine.Standard.Render.Input.Services;

namespace Engine.Standard.Render.UserInterface.Standard;

public abstract class InteractableUserInterfaceComponentBase<TSelf> : UserInterfaceComponentBase where TSelf : InteractableUserInterfaceComponentBase<TSelf> {

	public delegate void MouseMovementInteractionDelegate( TSelf component, MouseMoveEvent mouseMoveEvent );
	public delegate void MouseButtonInteractionDelegate( TSelf component, MouseButtonEvent mouseButtonEvent );
	public delegate void MouseWheelInteractionDelegate( TSelf component, MouseWheelEvent mouseWheelEvent );

	public event MouseMovementInteractionDelegate? OnMouseEntered;
	public event MouseMovementInteractionDelegate? OnMouseExited;
	public event MouseMovementInteractionDelegate? OnMouseMovedInside;
	public event MouseButtonInteractionDelegate? OnClicked;
	public event MouseButtonInteractionDelegate? OnPressed;
	public event MouseButtonInteractionDelegate? OnReleased;
	public event MouseWheelInteractionDelegate? OnScrolled;

	protected readonly Collider2Shape Collider;
	protected readonly Collision2Calculation<double> Collision;

	public bool ClickEnabled { get; protected set; }
	public bool ScrollEnabled { get; protected set; }
	public MouseButton? ClickButton { get; protected set; }
	public bool Hovering { get; protected set; }
	public bool HoveredPress { get; protected set; }

	public InteractableUserInterfaceComponentBase( UserInterfaceElementBase element ) : base( element ) {
		this.Collider = new Collider2Shape();
		this.Collider.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] );
		this.Collider.SetTransform( this.TransformInterface );
		this.Collision = new Collision2Calculation<double>( this.Collider, element.UserInterfaceServiceAccess.Get<MouseColliderProvider>().ColliderNDCA );
	}

	protected override bool DoOnMouseMoved( MouseMoveEvent @event ) {
		if (!this.Collision.Evaluate())
			this.LogWarning( "Collision calculation failed." );
		bool wasHovering = this.Hovering;
		this.Hovering = this.Collision.CollisionResult.IsColliding;
		if (wasHovering != this.Hovering) {
			if (this.Hovering) {
				OnMouseEntered?.Invoke( (TSelf) this, @event );
			} else {
				OnMouseExited?.Invoke( (TSelf) this, @event );
			}
		}
		if (this.Hovering)
			OnMouseMovedInside?.Invoke( (TSelf) this, @event );
		return false;
	}

	protected override bool DoOnMouseButton( MouseButtonEvent @event ) {
		if (!this.ClickEnabled)
			return false;
		if (this.ClickButton.HasValue && this.ClickButton.Value != @event.Button)
			return this.Hovering;
		if (@event.InputType == TactileInputType.Press && this.Hovering && (!this.ClickButton.HasValue || this.ClickButton.Value == @event.Button)) {
			OnPressed?.Invoke( (TSelf) this, @event );
			this.HoveredPress = true;
			return this.Hovering;
		}
		if (@event.InputType == TactileInputType.Release && this.HoveredPress && (!this.ClickButton.HasValue || this.ClickButton.Value == @event.Button)) {
			OnReleased?.Invoke( (TSelf) this, @event );
			if (this.Hovering)
				OnClicked?.Invoke( (TSelf) this, @event );
			this.HoveredPress = false;
			return this.Hovering;
		}
		return this.Hovering;
	}

	protected override bool DoOnMouseWheelScrolled( MouseWheelEvent @event ) {
		if (!this.ScrollEnabled)
			return false;
		if (this.Hovering) {
			OnScrolled?.Invoke( (TSelf) this, @event );
			return true;
		}
		return false;
	}
}

public sealed class InteractableButton : InteractableUserInterfaceComponentBase<InteractableButton> {
	public TexturedNineSlicedBackground Background { get; }
	public Label Label { get; }

	public InteractableButton( UserInterfaceElementBase element, string text ) : base( element ) {
		this.Background = AddChild( new TexturedNineSlicedBackground( element, element.UserInterfaceServiceAccess.Textures.Get( "test" ) ) );
		this.Label = AddChild( new Label( element ) {
			Text = text,
			FontName = DefaultFontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		} );

		this.ClickEnabled = true;
		OnMouseEntered += DefaultOnEnter;
		OnMouseExited += DefaultOnExit;
		OnPressed += DefaultOnPressed;
		OnReleased += DefaultOnReleased;
	}

	protected override void OnUpdate( double time, double deltaTime ) { }
	protected override void OnPlacementChanged() { }
	protected internal override void DoHide() { }
	protected internal override void DoShow() { }
	public void RemoveDefaultDecorations() {
		OnMouseEntered -= DefaultOnEnter;
		OnMouseExited -= DefaultOnExit;
		OnPressed -= DefaultOnPressed;
		OnReleased -= DefaultOnReleased;
	}

	public static string DefaultFontName => "calibrib";
	private static void DefaultOnExit( InteractableButton button, MouseMoveEvent @event ) => button.Background.Color = 1;
	private static void DefaultOnEnter( InteractableButton button, MouseMoveEvent @event ) => button.Background.Color = (.9, .9, .9, 1);
	private static void DefaultOnPressed( InteractableButton button, MouseButtonEvent @event ) => button.Background.Color = (.75, .75, .75, 1);
	private static void DefaultOnReleased( InteractableButton button, MouseButtonEvent @event ) => button.Background.Color = 1;
}
