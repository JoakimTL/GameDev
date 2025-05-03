using Engine.Modularity;

namespace Engine.Standard;

public sealed class GameStateService() : MessageBusServiceBase( "gamestatechanges" ) {

	public delegate void GameStateChangeHandler( string name, object? value );

	public event GameStateChangeHandler? GameStateChanged;

	private readonly Dictionary<string, object> _gameState = [];

	protected override void MessageProcessed( Message message ) {
		if (message.Content is GameStateChangeMessage gameStateChange) {
			Set( gameStateChange.Name, gameStateChange.NewState );
		}
	}

	private void Set( string name, object? value ) {
		if (value is null) {
			if (this._gameState.Remove( name ))
				GameStateChanged?.Invoke( name, value );
			return;
		}
		if (this._gameState.TryGetValue( name, out object? oldValue ) && oldValue.Equals( value ))
			return;
		this._gameState[ name ] = value;
		GameStateChanged?.Invoke( name, value );
	}

	public void SetNewState( string name, object? newState ) => MessageBus.PublishAnonymously( new GameStateChangeMessage( name, newState ), "gamestatechanges", log: false );

	public T? Get<T>( string name ) {
		if (!this._gameState.TryGetValue( name, out object? value ))
			return default;
		if (value is T t)
			return t;
		return default;
	}
}
