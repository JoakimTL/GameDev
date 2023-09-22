using Engine.Structure.Interfaces;
using GlfwBinding.Enums;

namespace Engine.GlobalServices;

public sealed class InputMappingService : Identifiable, IGlobalService {

	private readonly Dictionary<string, Keys> _keyInputByName;
	private readonly Dictionary<Keys, string> _keyNameByKey;
	private readonly Dictionary<string, MouseButton> _buttonInputByName;
	private readonly Dictionary<MouseButton, string> _buttonNameByButton;

	public InputMappingService() {
		_keyInputByName = new();
		_keyNameByKey = new();
		_buttonInputByName = new();
		_buttonNameByButton = new();
		InitializeDictionaries();
	}

	private void InitializeDictionaries() {
		var keysWithNames = Enum.GetValues<Keys>().Where( p => p != Keys.Unknown ).Select( key => (key, $"Key:{key}") );
		foreach ( var (key, name) in keysWithNames ) {
			if ( _keyInputByName.TryAdd( name, key ) )
				_keyNameByKey.Add( key, name );
		}

		var buttonsWithNames = Enum.GetValues<MouseButton>().Where( p => p != MouseButton.Unknown ).Select( button => (button, $"Button:{button}") );
		foreach ( var (button, name) in buttonsWithNames ) {
			if ( _buttonInputByName.TryAdd( name, button ) )
				_buttonNameByButton.Add( button, name );
		}
	}

	public Keys GetKeyByName( string name ) {
		if ( _keyInputByName.TryGetValue( name, out var key ) )
			return key;
		this.LogWarning( $"Key not found: {name}" );
		return Keys.Unknown;
	}

	public string GetKeyName( Keys key ) {
		if ( _keyNameByKey.TryGetValue( key, out var name ) )
			return name;
		this.LogWarning( $"Key not found: {key}" );
		return string.Empty;
	}

	public MouseButton GetButtonByName( string name ) {
		if ( _buttonInputByName.TryGetValue( name, out var button ) )
			return button;
		this.LogWarning( $"Button not found: {name}" );
		return MouseButton.Unknown;
	}

	public string GetButtonName( MouseButton button ) {
		if ( _buttonNameByButton.TryGetValue( button, out var name ) )
			return name;
		this.LogWarning( $"Button not found: {button}" );
		return string.Empty;
	}
}
