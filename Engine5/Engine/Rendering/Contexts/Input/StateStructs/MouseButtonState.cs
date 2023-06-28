using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input.StateStructs;

public readonly struct MouseButtonState {
	private readonly byte _state;

	public MouseButtonState( bool button1, bool button2, bool button3, bool button4, bool button5, bool button6, bool button7, bool button8 ) {
		unsafe {
			byte localState = 0;
			byte* ptr = &localState;
			*ptr |= (byte) ( *(byte*) &button1 << 0 );
			*ptr |= (byte) ( *(byte*) &button2 << 1 );
			*ptr |= (byte) ( *(byte*) &button3 << 2 );
			*ptr |= (byte) ( *(byte*) &button4 << 3 );
			*ptr |= (byte) ( *(byte*) &button5 << 4 );
			*ptr |= (byte) ( *(byte*) &button6 << 5 );
			*ptr |= (byte) ( *(byte*) &button7 << 6 );
			*ptr |= (byte) ( *(byte*) &button8 << 7 );
			_state = localState;
		}
	}

	public bool LeftButton => ( _state & 0b0000_0001 ) != 0;
	public bool RightButton => ( _state & 0b0000_0010 ) != 0;
	public bool MiddleButton => ( _state & 0b0000_0100 ) != 0;
	public bool Button4 => ( _state & 0b0000_1000 ) != 0;
	public bool Button5 => ( _state & 0b0001_0000 ) != 0;
	public bool Button6 => ( _state & 0b0010_0000 ) != 0;
	public bool Button7 => ( _state & 0b0100_0000 ) != 0;
	public bool Button8 => ( _state & 0b1000_0000 ) != 0;

	public bool this[ MouseButton button ] => button switch {
		MouseButton.Left => LeftButton,
		MouseButton.Right => RightButton,
		MouseButton.Middle => MiddleButton,
		MouseButton.Button4 => Button4,
		MouseButton.Button5 => Button5,
		MouseButton.Button6 => Button6,
		MouseButton.Button7 => Button7,
		MouseButton.Button8 => Button8,
		_ => throw new ArgumentOutOfRangeException( nameof( button ), button, null ),
	};
}
