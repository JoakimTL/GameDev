using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input;

public abstract class KeyboardEventListener : IKeyboardEventListener
{

    /// <summary>
    /// Whenever a key is pressed.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="key">The key being pressed.</param>
    /// <param name="mods">The keyboard modifiers active when the event was fired.</param>
    /// <param name="scanCode">The scancode of the key.</param>
    public virtual void OnKeyPressed(Keys key, ModifierKeys mods, int scanCode)
    {

    }

    /// <summary>
    /// Whenever a key is released.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="key">The key being released.</param>
    /// <param name="mods">The keyboard modifiers active when the event was fired.</param>
    /// <param name="scanCode">The scancode of the key.</param>
    public virtual void OnKeyReleased(Keys key, ModifierKeys mods, int scanCode)
    {

    }

    /// <summary>
    /// Whenever a key is held long enough to be fire a repeat event.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="key">The key being repeated.</param>
    /// <param name="mods">The keyboard modifiers active when the event was fired.</param>
    /// <param name="scanCode">The scancode of the key.</param>
    public virtual void OnKeyRepeated(Keys key, ModifierKeys mods, int scanCode)
    {

    }

    /// <summary>
    /// Whenever a character is written using the keyboard.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="charCode">The 32-bit character code.</param>
    /// <param name="character">The string representation of the character code as a string.</param>
    /// <param name="modifiers">The keyboard modifiers active when the event was fired.</param>
    public virtual void OnCharacterWritten(uint charCode, string character, ModifierKeys mods)
    {

    }

}
