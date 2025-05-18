using Engine.Module.Render.Input;

namespace Engine.Module.Render.Entities;

public sealed class RenderEntityInput : DisposableIdentifiable, ICapturingUserInputListener {
	private readonly CapturableUserInputEventService _capturableUserInputEventService;

	public event TimedMouseButtonHandler? OnMouseButton;
	public event TimedMouseScrollHandler? OnMouseWheelScrolled;
	public event TimedMouseMoveHandler? OnMouseMoved;
	public event TimedMouseEnterHandler? OnMouseEnter;

	public event TimedKeyboardHandler? OnKey;
	public event TimedKeyboardCharHandler? OnCharacter;

	public RenderEntityInput( CapturableUserInputEventService capturableUserInputEventService ) {
		capturableUserInputEventService.AddListener( this );
		this._capturableUserInputEventService = capturableUserInputEventService;
	}

	bool ICapturingUserInputListener.OnMouseButton( MouseButtonEvent e ) {
		OnMouseButton?.Invoke( e );
		return false;
	}

	bool ICapturingUserInputListener.OnMouseWheelScrolled( MouseWheelEvent e ) {
		OnMouseWheelScrolled?.Invoke( e );
		return false;
	}

	bool ICapturingUserInputListener.OnMouseMoved( MouseMoveEvent e ) {
		OnMouseMoved?.Invoke( e );
		return false;
	}

	bool ICapturingUserInputListener.OnMouseEnter( MouseEnterEvent e ) {
		OnMouseEnter?.Invoke( e );
		return false;
	}

	bool ICapturingUserInputListener.OnKey( KeyboardEvent e ) {
		OnKey?.Invoke( e );
		return false;
	}

	bool ICapturingUserInputListener.OnCharacter( KeyboardCharacterEvent e ) {
		OnCharacter?.Invoke( e );
		return false;
	}

	protected override bool InternalDispose() {
		_capturableUserInputEventService.RemoveListener( this );
		return true;
	}
}