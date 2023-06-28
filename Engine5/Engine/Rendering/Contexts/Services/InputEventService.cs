using Engine.GlobalServices;
using Engine.Rendering.Contexts.Input;
using Engine.Rendering.Contexts.Input.Listeners;
using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Structure.Interfaces;
using Engine.Time;
using GlfwBinding;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Services;

public sealed class InputEventService : Identifiable, IContextService, IUpdateable {

    public static readonly int MinKeyIndex = Enum.GetValues<Keys>().Min( p => (int) p );
    public static readonly int MaxKeyIndex = Enum.GetValues<Keys>().Max( p => (int) p );
    public static readonly int MinButtonIndex = Enum.GetValues<MouseButton>().Min( p => (int) p );
    public static readonly int MaxButtonIndex = Enum.GetValues<MouseButton>().Max( p => (int) p );
    public static readonly int KeyBooleanArrayLength = MaxKeyIndex + 1;
    public static readonly int ButtonBooleanArrayLength = MaxButtonIndex + 1;
    private readonly LoggedInputService _inputService;

    public MouseInputEventListener MouseEventListener { get; }
    public MouseHandler Mouse { get; }
    public KeyboardInputEventManager KeyboardEventListener { get; }
    public KeyboardStateContainer Keyboard { get; }
    public WindowInputEventListener WindowEventListener { get; }

    public InputEventService( Window window, LoggedInputService inputService ) {
        MouseEventListener = new( window );
        KeyboardEventListener = new( window );
        Mouse = new( window, MouseEventListener );
        Keyboard = new( KeyboardEventListener );
        WindowEventListener = new( window );

        Mouse.ButtonEvent += OnMouseButtonEvent;
        Mouse.LockChanged += OnMouseLockChanged;
        Mouse.MouseMoved += OnMouseMoved;
        Mouse.WheelScrolled += OnMouseWheelScrolled;
        Mouse.MouseEnter += OnMouseEnter;
        KeyboardEventListener.KeyPressed += OnKeyPressed;
        KeyboardEventListener.KeyReleased += OnKeyReleased;

        this.LogLine( $"Lowest key input index: {MinKeyIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
        this.LogLine( $"Highest key input index: {MaxKeyIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
        this.LogLine( $"Lowest button input index: {MinButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
        this.LogLine( $"Highest button input index: {MaxButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue );
        this.LogLine( Glfw.RawMouseMotionSupported() ? "Raw mouse input supported!" : "Raw mouse input not supported!", Log.Level.NORMAL, color: ConsoleColor.Blue );
        this._inputService = inputService;
    }

    private void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) 
        => _inputService.RegisterKeyEvent( Clock64.StartupTime, new( key, false, mods ) );

    private void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) 
        => _inputService.RegisterKeyEvent( Clock64.StartupTime, new( key, true, mods ) );

    private void OnMouseButtonEvent( double time, MouseButtonEventState state )
        => _inputService.RegisterButtonEvent( time, state );

    private void OnMouseLockChanged( double time, bool state )
        => _inputService.RegisterMouseLockEvent( time, state );

    private void OnMouseMoved( double time, MousePointerState state, bool lockState )
        => _inputService.RegisterMouseMoveEvent( time, state, lockState );

    private void OnMouseWheelScrolled( double time, Point2d scroll )
        => _inputService.RegisterMouseWheelEvent( time, scroll );

    private void OnMouseEnter( double time, bool state )
        => _inputService.RegisterMouseEnterEvent( time, state );

    public void Update( float time, float deltaTime ) {
        if (_inputService.LockState != Mouse.State.IsLocked)
            Mouse.SetLock( _inputService.LockState );
    }
}
