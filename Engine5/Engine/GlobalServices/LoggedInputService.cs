using Engine.GlobalServices.LoggedInput;
using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;
public class LoggedInputService : Identifiable, IGlobalService {

    public event Action<double, KeyEventState>? KeyEvent;
    public event Action<double, MouseButtonEventState>? ButtonEvent;
    public event Action<double, bool>? MouseLockEvent;
    public event Action<double, MousePointerState, bool>? MouseMoveEvent;
    public event Action<double, Point2d>? MouseWheelEvent;
    public event Action<double, bool>? MouseEnterEvent;

    public bool LockState { get; private set; }

    public LoggedInputQueue CreateInputQueue() => new( this );

    internal void RegisterKeyEvent( double time, KeyEventState state ) => KeyEvent?.Invoke( time, state );

    internal void RegisterButtonEvent( double time, MouseButtonEventState state ) => ButtonEvent?.Invoke( time, state );

    internal void RegisterMouseLockEvent( double time, bool state ) => MouseLockEvent?.Invoke( time, state );

    internal void RegisterMouseMoveEvent( double time, MousePointerState state, bool lockState ) => MouseMoveEvent?.Invoke( time, state, lockState );

    internal void RegisterMouseWheelEvent( double time, Point2d scroll ) => MouseWheelEvent?.Invoke( time, scroll );

    internal void RegisterMouseEnterEvent( double time, bool state ) => MouseEnterEvent?.Invoke( time, state );

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
        while ( _logger.TryDequeueMouseEnterEvent( out var timedState ) )
            this.LogLine( $"Mouse Enter: {timedState.Time} {timedState.EnterState}", Log.Level.VERBOSE );
        while ( _logger.TryDequeueMouseLockEvent( out var timedState ) )
            this.LogLine( $"Mouse Lock: {timedState.Time} {timedState.LockState}", Log.Level.VERBOSE );
        while ( _logger.TryDequeueMousePointerEvent( out var timedState ) )
            this.LogLine( $"Mouse Move: {timedState.Time} {timedState.State.PixelPosition} {timedState.State.NdcaPosition} {timedState.LockState}", Log.Level.VERBOSE );
        while ( _logger.TryDequeueMouseWheelEvent( out var timedState ) )
            this.LogLine( $"Mouse Wheel: {timedState.Time} {timedState.ScrollState}", Log.Level.VERBOSE );
        while ( _logger.TryDequeueButtonEvent( out var timedState ) )
            this.LogLine( $"Button: {timedState.Time} {timedState.State.Button} {timedState.State.Depressed} {timedState.State.ModifierKeys}", Log.Level.VERBOSE );
        while ( _logger.TryDequeueKeyEvent( out var timedState ) ) {
            this.LogLine( $"Key: {timedState.Time} {timedState.State.Key} {timedState.State.Depressed} {timedState.State.ModifierKeys}", Log.Level.VERBOSE );
            if ( timedState.State.Depressed == true && timedState.State.Key == GlfwBinding.Enums.Keys.LeftShift )
                _loggedInputService.SetLockState( !_loggedInputService.LockState );
        }
    }

}
