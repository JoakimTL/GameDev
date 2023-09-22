using Engine.Rendering.Contexts.Input.Listeners;
using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Time;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input;

public class MouseHandler : Identifiable {
    private readonly Window _window;
    public readonly MouseStateContainer State;

    public event TimedMouseButtonHandler? ButtonEvent;
    public event TimedMouseScrollHandler? WheelScrolled;
    public event TimedMouseStateChangeHandler? MouseEnter;
    public event TimedMouseMoveHandler? MouseMoved;
    public event TimedMouseStateChangeHandler? LockChanged;

    public MouseHandler( Window window, MouseInputEventListener mouseInputEventListener ) {
        this._window = window;
        this.State = new();

        mouseInputEventListener.ButtonPressed += OnButtonPressed;
        mouseInputEventListener.ButtonReleased += OnButtonReleased;
        mouseInputEventListener.WheelScrolled += OnWheelScrolled;
        mouseInputEventListener.MouseEnter += OnMouseEnter;
        mouseInputEventListener.MouseMoved += OnMouseMoved;
    }

    private void OnButtonPressed( MouseButton button, ModifierKeys modifiers ) {
        var time = Clock32.StartupTime;
        State[ button ] = true;
        ButtonEvent?.Invoke( time, new( button, true, modifiers ) );
    }

    private void OnButtonReleased( MouseButton button, ModifierKeys modifiers ) {
        var time = Clock32.StartupTime;
        State[ button ] = false;
        ButtonEvent?.Invoke( time, new( button, false, modifiers ) );
    }

    private void OnWheelScrolled( double xAxis, double yAxis ) {
        var time = Clock32.StartupTime;
        State.ScrolledWheel( new( xAxis, yAxis ) );
        WheelScrolled?.Invoke( time, new( xAxis, yAxis ) );
    }

    private void OnMouseEnter( bool entered ) {
        var time = Clock32.StartupTime;
        State.IsInside = entered;
        MouseEnter?.Invoke( time, entered );
    }

    private void OnMouseMoved( double xAxis, double yAxis ) {
        var time = Clock32.StartupTime;
        Point2d pixelPosition = new( xAxis, yAxis );
        Point2d ndcaPosition = new( ( 2 * xAxis / _window.Size.X - 1 ) * _window.AspectRatioVector.X, ( 1 - 2 * yAxis / _window.Size.Y ) * _window.AspectRatioVector.Y );
        State.MouseMoved( pixelPosition, ndcaPosition );
        MouseMoved?.Invoke( time, new( pixelPosition, ndcaPosition ), State.IsLocked );
    }

    public void SetLock( bool lockState ) {
        if ( State.SetLock( _window, lockState ) )
            LockChanged?.Invoke( Clock32.StartupTime, lockState );
    }
}
