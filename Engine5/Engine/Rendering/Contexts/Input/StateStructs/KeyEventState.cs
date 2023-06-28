using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input.StateStructs;

public readonly struct KeyEventState {

    private readonly ushort _value;

    public KeyEventState( Keys key, bool state, ModifierKeys modifierKeys ) {
        unsafe {
            _value = (ushort) ( (ushort) key | ( (ushort) modifierKeys << 9 | *(byte*) &state << 15 ) );
        }
    }

    public Keys Key => (Keys) ( _value & 0b1_1111_1111 );

    /// <summary>
    /// True if the button is pressed
    /// </summary>
    public bool Depressed => ( _value >> 15 & 0b1 ) == 1;

    public ModifierKeys ModifierKeys => (ModifierKeys) ( _value >> 9 & 0b11_1111 );

}