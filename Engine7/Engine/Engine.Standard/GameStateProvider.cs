using Engine.Modularity;
using Engine.Module.Render;

namespace Engine.Standard;

public sealed class GameStateProvider( GameStateService gameStateService) : IRenderServiceProvider {
	private readonly GameStateService _gameStateService = gameStateService;

	public void SetNewState( string name, object? newState ) => _gameStateService.SetNewState( name, newState );

	public T? Get<T>( string name ) => _gameStateService.Get<T>( name );
}
