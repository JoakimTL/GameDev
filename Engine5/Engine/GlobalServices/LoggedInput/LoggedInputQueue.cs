using Engine.Rendering.Contexts.Input.StateStructs;
using System.Collections.Concurrent;

namespace Engine.GlobalServices.LoggedInput;

public sealed class LoggedInputQueue : IDisposable {

    private readonly ConcurrentQueue<TimedKeyEventState> _timedKeyEventStates;
    private readonly ConcurrentQueue<TimedButtonEventState> _timedMouseButtonEventStates;
    private readonly ConcurrentQueue<TimedMousePointerState> _timedMousePointerStates;
    private readonly ConcurrentQueue<TimedMouseLockEventState> _timedMouseLockEventStates;
    private readonly ConcurrentQueue<TimedMouseWheelState> _timedMouseWheelEventStates;
    private readonly ConcurrentQueue<TimedMouseEnterEventState> _timedMouseEnterEventStates;

    private readonly LoggedInputService _loggedInputService;

    public LoggedInputQueue( LoggedInputService loggedInputService ) {
        this._loggedInputService = loggedInputService;
        _timedKeyEventStates = new();
        _timedMouseButtonEventStates = new();
        _timedMousePointerStates = new();
        _timedMouseLockEventStates = new();
        _timedMouseWheelEventStates = new();
        _timedMouseEnterEventStates = new();
        _loggedInputService.KeyEvent += OnKeyEvent;
        _loggedInputService.ButtonEvent += OnButtonEvent;
        _loggedInputService.MouseMoveEvent += OnMouseMoveEvent;
        _loggedInputService.MouseLockEvent += OnMouseLockEvent;
        _loggedInputService.MouseWheelEvent += OnMouseWheelEvent;
        _loggedInputService.MouseEnterEvent += OnMouseEnterEvent;
    }

    private void OnKeyEvent( double time, KeyEventState state ) 
        => _timedKeyEventStates.Enqueue( new( time, state ) );
    private void OnButtonEvent( double time, MouseButtonEventState state ) 
        => _timedMouseButtonEventStates.Enqueue( new( time, state ) );
    private void OnMouseMoveEvent( double time, MousePointerState state, bool lockState ) 
        => _timedMousePointerStates.Enqueue( new( time, state, lockState ) );
    private void OnMouseLockEvent( double time, bool lockState ) 
        => _timedMouseLockEventStates.Enqueue( new( time, lockState ) );
    private void OnMouseWheelEvent( double time, Point2d wheelState ) 
        => _timedMouseWheelEventStates.Enqueue( new( time, wheelState ) );
    private void OnMouseEnterEvent( double time, bool enterState )
        => _timedMouseEnterEventStates.Enqueue( new( time, enterState ) );
    public bool TryDequeueKeyEvent( out TimedKeyEventState timedState ) 
        => _timedKeyEventStates.TryDequeue( out timedState );
    public bool TryDequeueButtonEvent( out TimedButtonEventState timedState ) 
        => _timedMouseButtonEventStates.TryDequeue( out timedState );
    public bool TryDequeueMousePointerEvent( out TimedMousePointerState timedState ) 
        => _timedMousePointerStates.TryDequeue( out timedState );
    public bool TryDequeueMouseLockEvent( out TimedMouseLockEventState timedState ) 
        => _timedMouseLockEventStates.TryDequeue( out timedState );
    public bool TryDequeueMouseWheelEvent( out TimedMouseWheelState timedState ) 
        => _timedMouseWheelEventStates.TryDequeue( out timedState );
    public bool TryDequeueMouseEnterEvent( out TimedMouseEnterEventState timedState ) 
        => _timedMouseEnterEventStates.TryDequeue( out timedState );

    public void Dispose() {
        _loggedInputService.KeyEvent -= OnKeyEvent;
        _loggedInputService.ButtonEvent -= OnButtonEvent;
        _loggedInputService.MouseMoveEvent -= OnMouseMoveEvent;
        _loggedInputService.MouseLockEvent -= OnMouseLockEvent;
        _loggedInputService.MouseWheelEvent -= OnMouseWheelEvent;
        _loggedInputService.MouseEnterEvent -= OnMouseEnterEvent;
    }
}
