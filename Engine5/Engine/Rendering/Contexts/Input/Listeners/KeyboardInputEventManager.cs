using Engine.Rendering.OGL;
using GlfwBinding;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input.Listeners;

public class KeyboardInputEventManager : Identifiable
{

    private readonly Window _window;
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

        _keyCallback = OnKey;
        _characterCallback = OnCharacter;

        EventUtilities.SetKeyCallback(_window.Pointer, _keyCallback);
        EventUtilities.SetCharModsCallback(_window.Pointer, _characterCallback);
    }

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
                KeyPressed?.Invoke(key, mods, scanCode);
                break;
            case InputState.Release:
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
