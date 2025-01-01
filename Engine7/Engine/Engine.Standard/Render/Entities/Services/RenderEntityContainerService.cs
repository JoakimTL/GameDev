using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Services;
using Engine.Processing;

namespace Engine.Standard.Render.Entities.Services;

[Do<IDisposable>.After<SceneService>]
public sealed class RenderEntityContainerService( RenderEntityServiceAccess renderEntityServiceAccess ) : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<EntityContainer, RenderEntityContainer> _containerPairs = [];
	private readonly RenderEntityServiceAccess _renderEntityServiceAccess = renderEntityServiceAccess;

	internal void RegisterEntityContainer( EntityContainer entityContainer ) {
		if (this._containerPairs.ContainsKey( entityContainer )) {
			this.LogLine( $"{entityContainer} already registered.", Log.Level.VERBOSE );
			return;
		}
		this._containerPairs.Add( entityContainer, new RenderEntityContainer( entityContainer, this._renderEntityServiceAccess ) );
		entityContainer.OnDisposed += OnContainerDisposed;
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
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