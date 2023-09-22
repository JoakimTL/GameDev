using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input.StateStructs;
public readonly struct ButtonEventState {

    private const int ButtonBit = 0;
    private const int StateBit = 3;
    private const int ModifierBit = 4;

    private readonly ushort _value;

    public ButtonEventState( MouseButton button, bool state, ModifierKeys modifierKeys ) {
        unsafe {
            _value =
                (ushort) (
                    ( ( (uint) button & 0b111 ) << ButtonBit ) |
                    ( ( (uint) *(byte*) &state & 0b1 ) << StateBit ) |
                    ( ( (uint) modifierKeys & 0b111111 ) << ModifierBit )
                );
        }
    }

    public MouseButton Button => (MouseButton) ( ( _value >> ButtonBit ) & 0b111 );

    /// <summary>
    /// True if the button is pressed
    /// </summary>
    public bool Pressed => ( ( _value >> StateBit ) & 0b1 ) != 0;

    public ModifierKeys ModifierKeys => (ModifierKeys) ( ( _value >> ModifierBit ) & 0b111111 );

}
