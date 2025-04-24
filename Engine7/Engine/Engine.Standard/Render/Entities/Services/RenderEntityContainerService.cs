using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Services;
using Engine.Processing;
using Engine.Serialization;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Engine.Standard.Render.Entities.Services;

[Do<IDisposable>.After<SceneService>]
public sealed class RenderEntityContainerService( RenderEntityServiceAccess renderEntityServiceAccess, SerializerProvider serializerProvider ) : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<EntityContainer, RenderEntityContainer> _containerPairs = [];
	private readonly RenderEntityServiceAccess _renderEntityServiceAccess = renderEntityServiceAccess;
	private readonly SerializerProvider _serializerProvider = serializerProvider;
	private readonly ConcurrentQueue<EntityContainer> _disposedContainers = [];

	internal void RegisterEntityContainer( SynchronizedEntityContainer synchronizedEntityContainer ) {
		if (this._containerPairs.ContainsKey( synchronizedEntityContainer.OriginalContainer )) {
			this.LogLine( $"{synchronizedEntityContainer.OriginalContainer} already registered.", Log.Level.VERBOSE );
			return;
		}
		this._containerPairs.Add( synchronizedEntityContainer.OriginalContainer, new RenderEntityContainer( synchronizedEntityContainer, _serializerProvider, this._renderEntityServiceAccess ) );
		synchronizedEntityContainer.OriginalContainer.OnDisposed += OnContainerDisposed;
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		if (disposable is not EntityContainer container)
			return;
		_disposedContainers.Enqueue( container );
	}

	protected override bool InternalDispose() {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs) {
			pair.Key.OnDisposed -= OnContainerDisposed;
			pair.Value.Dispose();
		}
		return true;
	}

	public void Update( double time, double deltaTime ) {
		while (this._disposedContainers.TryDequeue( out EntityContainer? container )) {
			container.OnDisposed -= OnContainerDisposed;
			if (this._containerPairs.TryGetValue( container, out RenderEntityContainer? renderEntityContainer )) {
				renderEntityContainer.Dispose();
				this._containerPairs.Remove( container );
			}
		}
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs)
			pair.Value.Update( time, deltaTime );
	}
}