using Engine.Module.Render;

namespace Engine.Standard;

public sealed class GameStateProvider : IRenderServiceProvider {

	public delegate void GameStateChangeHandler( string key, object? value );

	public event GameStateChangeHandler? GameStateChanged;

	private readonly Dictionary<string, object> _gameState = [];

	public void Set<T>( string key, T? value ) {
		if (value is null) {
			if (this._gameState.Remove( key ))
				GameStateChanged?.Invoke( key, value );
			return;
		}
		if (this._gameState.TryGetValue( key, out object? oldValue ) && oldValue.Equals( value ))
			return;
		this._gameState[ key ] = value;
		GameStateChanged?.Invoke( key, value );
	}

	public T? Get<T>( string key ) {
		if (!this._gameState.TryGetValue( key, out object? value ))
			return default;
		if (value is T t)
			return t;
		return default;
	}
}
