namespace Engine.Standard;

public sealed class GameStateProvider {
	private readonly Dictionary<string, object> _gameState = [];

	public void Set<T>( string key, T? value ) {
		if (value is null) {
			this._gameState.Remove( key );
			return;
		}
		this._gameState[ key ] = value;
	}

	public T? Get<T>( string key ) => this._gameState.TryGetValue( key, out object? value ) && value is T t ? t : default;

}
