namespace Engine.Standard;

public sealed class GameStateProvider( GameStateService gameStateService) : IServiceProvider {
	private readonly GameStateService _gameStateService = gameStateService;

	public void SetNewState( string name, object? newState ) => this._gameStateService.SetNewState( name, newState );

	public T? Get<T>( string name ) => this._gameStateService.Get<T>( name );
}
