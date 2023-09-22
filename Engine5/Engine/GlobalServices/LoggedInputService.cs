using Engine.GlobalServices.LoggedInput;
using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;
public class LoggedInputService : Identifiable, IGlobalService {

	public event Action<TimedEvent>? TimedEvent;

	public bool LockState { get; private set; }

	public LoggedInputQueue CreateInputQueue() => new( this );

	internal void RegisterKeyEvent( float time, KeyEventState state ) => TimedEvent?.Invoke( new( time, new KeyEvent( state ) ) );

	internal void RegisterButtonEvent( float time, ButtonEventState state ) => TimedEvent?.Invoke( new( time, new ButtonEvent( state ) ) );

	internal void RegisterMouseLockEvent( float time, bool state ) => TimedEvent?.Invoke( new( time, new MouseLockEvent( state ) ) );

	internal void RegisterMouseMoveEvent( float time, MousePointerState state, bool lockState ) => TimedEvent?.Invoke( new( time, new MousePointerEvent( state, lockState ) ) );

	internal void RegisterMouseWheelEvent( float time, Point2d scroll ) => TimedEvent?.Invoke( new( time, new MouseWheelEvent( scroll ) ) );

	internal void RegisterMouseEnterEvent( float time, bool state ) => TimedEvent?.Invoke( new( time, new MouseEnterEvent( state ) ) );

	public void SetLockState( bool lockState ) => LockState = lockState;
}

public class LoggedInputServiceTesterService : Identifiable, IGlobalService {
	private readonly LoggedInputService _loggedInputService;
	private readonly LoggedInputQueue _logger;

	public LoggedInputServiceTesterService( LoggedInputService loggedInputService ) {
		this._loggedInputService = loggedInputService;
		_logger = _loggedInputService.CreateInputQueue();
	}

	public void LogEverything() {
		while ( _logger.TryDequeueTimedEvent( out var timedEvent ) ) {
			switch ( timedEvent.Event ) {
				case KeyEvent timedKeyEvent:
					this.LogLine( $"Key: {timedEvent.Time} {timedKeyEvent.State.Key} {timedKeyEvent.State.Pressed} {timedKeyEvent.State.ModifierKeys}", Log.Level.VERBOSE );
					if ( timedKeyEvent.State.Pressed == true && timedKeyEvent.State.Key == GlfwBinding.Enums.Keys.LeftShift )
						_loggedInputService.SetLockState( !_loggedInputService.LockState );
					break;
				case ButtonEvent timedButtonEvent:
					this.LogLine( $"Button: {timedEvent.Time} {timedButtonEvent.State.Button} {timedButtonEvent.State.Pressed}", Log.Level.VERBOSE );
					break;
				case MousePointerEvent timedMousePointerEvent:
					this.LogLine( $"Mouse Pointer: {timedEvent.Time} {timedMousePointerEvent.State.PixelPosition} {timedMousePointerEvent.State.NdcaPosition} {timedMousePointerEvent.LockState}", Log.Level.VERBOSE );
					break;
				case MouseWheelEvent timedMouseWheelEvent:
					this.LogLine( $"Mouse Wheel: {timedEvent.Time} {timedMouseWheelEvent.ScrollState}", Log.Level.VERBOSE );
					break;
				case MouseLockEvent timedMouseLockEvent:
					this.LogLine( $"Mouse Lock: {timedEvent.Time} {timedMouseLockEvent.LockState}", Log.Level.VERBOSE );
					break;
				case MouseEnterEvent timedMouseEnterEvent:
					this.LogLine( $"Mouse Enter: {timedEvent.Time} {timedMouseEnterEvent.EnterState}", Log.Level.VERBOSE );
					break;
			}
		}
	}

}
