using Engine.Rendering.Objects;
using Engine.Rendering.OGL;
using GlfwBinding;
using GlfwBinding.Enums;
using System.Numerics;

namespace Engine.Rendering.Input;

public class MouseInputEventManager : Identifiable
{

    public static readonly int MinButtonIndex = Enum.GetValues<MouseButton>().Min(p => (int)p);
    public static readonly int MaxButtonIndex = Enum.GetValues<MouseButton>().Max(p => (int)p);

    private readonly Window _window;
    private readonly MouseButtonCallback _buttonCallback;
    private readonly MouseEnterCallback _enterCallback;
    private readonly MouseCallback _cursorCallback;
    private readonly MouseCallback _scrollCallback;

    private readonly MouseData _data;
    /// <summary>
    /// The current state of the mouse pointer.
    /// </summary>
    public MouseState State { get; }

    /// <summary>
    /// This event fires the mouse wheel has changed position.
    /// </summary>
    public event MouseScrollHandler? WheelScrolled;
    /// <summary>
    /// This event fires whenever a mouse button is pressed down.
    /// </summary>
    public event MouseButtonHandler? ButtonPressed;
    /// <summary>
    /// This event fires whenever a mouse button is released.
    /// </summary>
    public event MouseButtonHandler? ButtonReleased;
    /// <summary>
    /// If a mouse button is held down for a period of time, warranting a repeat event.
    /// </summary>
    public event MouseButtonHandler? ButtonRepeated;
    /// <summary>
    /// If the cursor is visible, this event will trigger when the mouse is moved.<br/>
    /// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseCursorMove(Window, MouseState)"/>!<br/> 
    /// Use <see cref="MovedHidden"/> for hidden mouse movements.
    /// </summary>
    public event MouseMoveHandler? MovedVisible;
    /// <summary>
    /// If the cursor is hidden, this event will trigger when the mouse is moved.<br/>
    /// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseLockedMove(Window, MouseState)(Window, MouseState)"/>!<br/> 
    /// Use <see cref="MovedVisible"/> for visible mouse movements.
    /// </summary>
    public event MouseMoveHandler? MovedHidden;

    internal MouseInputEventManager(Window window)
    {
        _window = window;
        _data = new MouseData();
        State = new MouseState(_data);
        _data.inside = InputUtilities.GetWindowAttribute(_window.Pointer, WindowAttribute.MouseHover) != 0;

        _enterCallback = OnEnter;
        _buttonCallback = OnButton;
        _cursorCallback = OnCursor;
        _scrollCallback = OnScroll;

        EventUtilities.SetCursorEnterCallback(_window.Pointer, _enterCallback);
        EventUtilities.SetMouseButtonCallback(_window.Pointer, _buttonCallback);
        EventUtilities.SetCursorPositionCallback(_window.Pointer, _cursorCallback);
        EventUtilities.SetScrollCallback(_window.Pointer, _scrollCallback);

        this.LogLine($"Lowest button input index: {MinButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue);
        this.LogLine($"Highest button input index: {MaxButtonIndex}", Log.Level.NORMAL, ConsoleColor.Blue);
        Log.Line(Glfw.RawMouseMotionSupported() ? "Raw mouse input supported!" : "Raw mouse input not supported!", Log.Level.NORMAL, color: ConsoleColor.Blue);

        //AddListener( Resources.GlobalService<ClientInput>() );
    }

    #region Add/Remove
    public void AddListener(IMouseChangeEventListener listener)
    {
        MovedVisible += listener.OnMouseCursorMove;
        MovedHidden += listener.OnMouseLockedMove;
    }

    public void AddListener(IWheelScrollChangeEventListener listener) => WheelScrolled += listener.OnMouseScroll;

    public void AddListener(IButtonChangeEventListener listener)
    {
        ButtonPressed += listener.OnButtonPressed;
        ButtonReleased += listener.OnButtonReleased;
        ButtonRepeated += listener.OnButtonRepeat;
    }

    public void AddListener(IMouseEventListener listener)
    {
        AddListener(listener as IMouseChangeEventListener);
        AddListener(listener as IWheelScrollChangeEventListener);
        AddListener(listener as IButtonChangeEventListener);
    }

    public void RemoveListener(IMouseChangeEventListener listener)
    {
        MovedVisible -= listener.OnMouseCursorMove;
        MovedHidden -= listener.OnMouseLockedMove;
    }

    public void RemoveListener(IWheelScrollChangeEventListener listener) => WheelScrolled -= listener.OnMouseScroll;

    public void RemoveListener(IButtonChangeEventListener listener)
    {
        ButtonPressed -= listener.OnButtonPressed;
        ButtonReleased -= listener.OnButtonReleased;
        ButtonRepeated -= listener.OnButtonRepeat;
    }

    public void RemoveListener(IMouseEventListener listener)
    {
        RemoveListener(listener as IMouseChangeEventListener);
        RemoveListener(listener as IWheelScrollChangeEventListener);
        RemoveListener(listener as IButtonChangeEventListener);
    }
    #endregion

    public void SetLock(bool state)
    {
        if (state)
            if (!_data.locked)
            {
                InputUtilities.SetInputMode(_window.Pointer, InputMode.Cursor, (int)CursorMode.Disabled);

                InputUtilities.GetCursorPosition(_window.Pointer, out double x, out double y);
                _data.lockedCursor.pos = new Vector2((float)x, (float)y);
                GetPositionData(_data.lockedCursor.pos.X, _data.lockedCursor.pos.Y, out Vector2 ndc, out Vector2 ndca);
                _data.lockedCursor.posNDC = ndc;
                _data.lockedCursor.posNDCA = ndca;
                _data.lastLockedCursor.pos = _data.lockedCursor.pos;
                _data.lastLockedCursor.posNDC = ndc;
                _data.lastLockedCursor.posNDCA = ndca;
            }
            else
                InputUtilities.SetInputMode(_window.Pointer, InputMode.Cursor, (int)CursorMode.Normal);
        _data.locked = state;
    }

    private void GetPositionData(float x, float y, out Vector2 ndc, out Vector2 ndca)
    {
        Vector2 preNDC = new Vector2(x, y) / _window.Size.AsFloat * 2;
        //In OGL the y-axis in the NDC space is "flipped". It goes bottom to top rather than top to bottom.
        ndc = new Vector2(preNDC.X - 1, 1 - preNDC.Y);
        ndca = ndc * _window.AspectRatioVector;
    }

    private void OnEnter(nint winPtr, bool enter) => _data.inside = enter;

    private void OnButton(nint winPtr, MouseButton button, InputState state, ModifierKeys modifiers)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnButton)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        switch (state)
        {
            case InputState.Press:
                ButtonPressed?.Invoke(button, modifiers, State);
                break;
            case InputState.Release:
                ButtonReleased?.Invoke(button, modifiers, State);
                break;
            case InputState.Repeat:
                ButtonRepeated?.Invoke(button, modifiers, State);
                break;
        }
    }

    private void OnCursor(nint winPtr, double x, double y)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnCursor)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        if (_data.locked)
        {
            //Set last location to current, as the current location will be the previous one after this movement.
            _data.lastLockedCursor.pos = _data.lockedCursor.pos;
            _data.lastLockedCursor.posNDC = _data.lockedCursor.posNDC;
            _data.lastLockedCursor.posNDCA = _data.lockedCursor.posNDCA;

            _data.lockedCursor.pos = new Vector2((float)x, (float)y);
            GetPositionData(_data.lockedCursor.pos.X, _data.lockedCursor.pos.Y, out Vector2 ndc, out Vector2 ndca);
            _data.lockedCursor.posNDC = ndc;
            _data.lockedCursor.posNDCA = ndca;

            MovedHidden?.Invoke(State);

        }
        else
        {
            //Set last location to current, as the current location will be the previous one after this movement.
            _data.lastCursor.pos = _data.lockedCursor.pos;
            _data.lastCursor.posNDC = _data.lockedCursor.posNDC;
            _data.lastCursor.posNDCA = _data.lockedCursor.posNDCA;

            _data.cursor.pos = new Vector2((float)x, (float)y);
            GetPositionData(_data.cursor.pos.X, _data.cursor.pos.Y, out Vector2 ndc, out Vector2 ndca);
            _data.cursor.posNDC = ndc;
            _data.cursor.posNDCA = ndca;

            MovedVisible?.Invoke(State);
        }
    }

    private void OnScroll(nint winPtr, double x, double y)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnScroll)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        WheelScrolled?.Invoke((float)x, (float)y, State);
    }
}
