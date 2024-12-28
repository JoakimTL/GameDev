using Engine.Logging;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.OOP;

namespace Engine.Module.Render.Input;

public sealed class UserKeyboardInputEventService {
	private readonly OglWindow _window;
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

	public UserKeyboardInputEventService( WindowService windowService ) {
		this._window = windowService.Window;

		this._keyCallback = OnKey;
		this._characterCallback = OnCharacter;

		EventUtilities.SetKeyCallback( this._window.Handle, this._keyCallback );
		EventUtilities.SetCharModsCallback( this._window.Handle, this._characterCallback );
	}

	private void OnKey( nint winPtr, Keys key, int scanCode, InputState state, ModifierKeys mods ) {
		if (key == Keys.Unknown)
			return;

		if (this._window.Handle != winPtr) {
			Log.Warning( $"{nameof( OnKey )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		switch (state) {
			case InputState.Press:
				KeyPressed?.Invoke( key, mods, scanCode );
				break;
			case InputState.Release:
				KeyReleased?.Invoke( key, mods, scanCode );
				break;
			case InputState.Repeat:
				KeyRepeated?.Invoke( key, mods, scanCode );
				break;
		}
	}

	private void OnCharacter( nint winPtr, uint codePoint, ModifierKeys mods ) {
		if (this._window.Handle != winPtr) {
			Log.Warning( $"{nameof( OnCharacter )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		CharacterInput?.Invoke( codePoint, mods );
	}
}