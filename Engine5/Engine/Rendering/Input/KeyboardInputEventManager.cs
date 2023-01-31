using Engine.Rendering.Objects;
using Engine.Rendering.OGL;
using GlfwBinding;
using GlfwBinding.Enums;

namespace Engine.Rendering.Input;

public class KeyboardInputEventManager : Identifiable
{

    public static readonly int MinKeyIndex = Enum.GetValues<Keys>().Min(p => (int)p);
    public static readonly int MaxKeyIndex = Enum.GetValues<Keys>().Max(p => (int)p);

    private readonly Window _window;
    private readonly bool[] _keys;
    private readonly KeyCallback _keyCallback;
    private readonly CharModsCallback _characterCallback;

    /// <summary>
    /// Occurs when the window receives character input.<br/>
    /// This is usually used for writing text, as opposed to <see cref="KeyPressed"/> and <see cref="KeyReleased"/> being used for gameplay.
    /// </summary>
    public event KeyboardCharHandler? CharacterInput;
    /// <summary>
    /// Occurs when a key is pressed.<br/>
    /// This is usually used for gameplay, as opposed to <see cref="CharacterInput"/>, which is used for writing.
    /// </summary>
    public event KeyboardHandler? KeyPressed;
    /// <summary>
    /// Occurs when a key is released.<br/>
    /// This is usually used for gameplay, as opposed to <see cref="CharacterInput"/>, which is used for writing.
    /// </summary>
    public event KeyboardHandler? KeyReleased;
    /// <summary>
    /// Occurs when a key is held down for a period of time, warranting a repeat event.
    /// </summary>
    public event KeyboardHandler? KeyRepeated;

    internal KeyboardInputEventManager(Window window)
    {
        _window = window;
        _keys = new bool[2048];

        _keyCallback = OnKey;
        _characterCallback = OnCharacter;

        EventUtilities.SetKeyCallback(_window.Pointer, _keyCallback);
        EventUtilities.SetCharModsCallback(_window.Pointer, _characterCallback);

        this.LogLine($"Lowest key input index: {MinKeyIndex}", Log.Level.NORMAL, ConsoleColor.Blue);
        this.LogLine($"Highest key input index: {MaxKeyIndex}", Log.Level.NORMAL, ConsoleColor.Blue);

        //AddListener( Resources.GlobalService<ClientInput>() );
    }

    /// <summary>
    /// Returns whether the key is held down or not.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>The state of the key, true if pressed, false if not.</returns>
    public bool this[Keys key] => _keys[(int)key];

    #region Add/Remove
    public void AddListener(IKeyChangeEventListener listener)
    {
        KeyPressed += listener.OnKeyPressed;
        KeyReleased += listener.OnKeyReleased;
        KeyRepeated += listener.OnKeyRepeated;
    }

    public void AddListener(IWritingEventListener listener) => CharacterInput += listener.OnCharacterWritten;

    public void AddListener(IKeyboardEventListener listener)
    {
        AddListener(listener as IKeyChangeEventListener);
        AddListener(listener as IWritingEventListener);
    }

    public void RemoveListener(IKeyChangeEventListener listener)
    {
        KeyPressed -= listener.OnKeyPressed;
        KeyReleased -= listener.OnKeyReleased;
        KeyRepeated -= listener.OnKeyRepeated;
    }

    public void RemoveListener(IWritingEventListener listener) => CharacterInput -= listener.OnCharacterWritten;

    public void RemoveListener(IKeyboardEventListener listener)
    {
        RemoveListener(listener as IKeyChangeEventListener);
        RemoveListener(listener as IWritingEventListener);
    }
    #endregion

    private void OnKey(nint winPtr, Keys key, int scanCode, InputState state, ModifierKeys mods)
    {
        if (key == Keys.Unknown)
            return;

#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnKey)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        switch (state)
        {
            case InputState.Press:
                _keys[(int)key] = true;
                KeyPressed?.Invoke(key, mods, scanCode);
                break;
            case InputState.Release:
                _keys[(int)key] = false;
                KeyReleased?.Invoke(key, mods, scanCode);
                break;
            case InputState.Repeat:
                KeyRepeated?.Invoke(key, mods, scanCode);
                break;
        }
    }

    private void OnCharacter(nint winPtr, uint codePoint, ModifierKeys mods)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnCharacter)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        CharacterInput?.Invoke(codePoint, char.ConvertFromUtf32(unchecked((int)codePoint)), mods);
    }
}
