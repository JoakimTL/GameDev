using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;

namespace Engine.Standard.Entities.Render.Services;

public sealed class RenderEntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<EntityContainer, RenderEntityContainer> _containerPairs = [];

	internal void RegisterEntityContainer( EntityContainer entityContainer ) {
		if (this._containerPairs.ContainsKey( entityContainer )) { 
			this.LogLine( $"{entityContainer} already registered.", Log.Level.VERBOSE );
			return;
	}
		this._containerPairs.Add( entityContainer, new RenderEntityContainer( entityContainer ) );
		entityContainer.OnDisposed += OnContainerDisposed;
	}

	private void OnContainerDisposed() {
		List<EntityContainer> containersToRemove = this._containerPairs.Keys.Where( p => p.Disposed ).ToList();

		foreach (EntityContainer container in containersToRemove)
			this._containerPairs.Remove( container );
	}

	protected override bool InternalDispose() {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs)
			pair.Value.Dispose();
		return true;
	}

	public void Update( double time, double deltaTime ) {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs)
			pair.Value.Update( time, deltaTime );
	}
}