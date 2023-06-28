using Engine.Rendering.Contexts.Input.Listeners;
using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Rendering.Contexts.Services;
using GlfwBinding.Enums;

namespace Engine.Rendering.Contexts.Input;
public class KeyboardStateContainer : Identifiable {
	private KeyboardInputEventManager _keyboardEventListener;

	private readonly bool[] _keys;

	public event Action? KeyboardEvent;

	public KeyboardStateContainer( KeyboardInputEventManager keyboardEventListener ) {
		this._keyboardEventListener = keyboardEventListener;
		_keys = new bool[ InputEventService.KeyBooleanArrayLength ];

		_keyboardEventListener.KeyPressed += OnKeyPressed;
		_keyboardEventListener.KeyReleased += OnKeyReleased;
	}

	public bool this[ Keys key ] => _keys[ (int) key ];

	public KeyboardKeyState ReadonlyState => KeyboardKeyState.CreateState( _keys );

	private void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) {
		_keys[ (int) key ] = false;
		KeyboardEvent?.Invoke();
	}

	private void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) {
		_keys[ (int) key ] = true;
		KeyboardEvent?.Invoke();
	}
}
