using Engine.GlobalServices.LoggedInput;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class KeyRequirement : IInputRequirement {

	private readonly Keys[] _keys;
	private readonly bool[] _pressed;
	public bool RequirementMet { get; private set; }

	public KeyRequirement( params Keys[] keys ) {
		RequirementMet = false;
		_keys = keys;
		_pressed = new bool[ keys.Length ];
	}

	public void RegisterInput<T>( T input ) {
		if ( input is not TimedKeyEvent keyEventState )
			return;
		for ( int i = 0; i < _keys.Length; i++ )
			if ( keyEventState.State.Key == _keys[ i ] ) {
				_pressed[ i ] = keyEventState.State.Pressed;
				break;
			}
		RequirementMet = _pressed.Any( p => p );
	}
}
