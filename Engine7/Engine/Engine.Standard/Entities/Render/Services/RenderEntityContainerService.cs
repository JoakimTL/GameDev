using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;

namespace Engine.Standard.Entities.Render.Services;

public sealed class RenderEntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<EntityContainer, RenderEntityContainer> _containerPairs = [];

	internal void RegisterEntityContainer( EntityContainer entityContainer ) {
		if (_containerPairs.ContainsKey( entityContainer )) { 
			this.LogLine( $"{entityContainer} already registered.", Log.Level.VERBOSE );
			return;
	}
		_containerPairs.Add( entityContainer, new RenderEntityContainer( entityContainer ) );
		entityContainer.OnDisposed += OnContainerDisposed;
	}

	private void OnContainerDisposed() {
		List<EntityContainer> containersToRemove = _containerPairs.Keys.Where( p => p.Disposed ).ToList();

		foreach (EntityContainer container in containersToRemove)
			_containerPairs.Remove( container );
	}

	protected override bool InternalDispose() {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in _containerPairs)
			pair.Value.Dispose();
		return true;
	}

	public void Update( double time, double deltaTime ) {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in _containerPairs)
			pair.Value.Update( time, deltaTime );
	}
}