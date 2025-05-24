using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Services;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Services;
using Engine.Processing;
using Engine.Serialization;
using System.Collections.Concurrent;

namespace Engine.Standard.Render.Entities.Services;

[Do<IDisposable>.After<SceneService>]
public sealed class RenderEntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<Guid, RenderEntityContainer> _renderEntityContainersByContainerId = [];
	private readonly SynchronizedEntityContainerService _synchronizedEntityContainerService;
	private readonly RenderEntityServiceAccess _renderEntityServiceAccess;
	private readonly SerializerProvider _serializerProvider;
	private readonly ConcurrentQueue<RenderEntityContainer> _disposedContainers = [];

	public RenderEntityContainerService( SynchronizedEntityContainerService synchronizedEntityContainerService, RenderEntityServiceAccess renderEntityServiceAccess, SerializerProvider serializerProvider ) {
		this._synchronizedEntityContainerService = synchronizedEntityContainerService;
		this._renderEntityServiceAccess = renderEntityServiceAccess;
		this._serializerProvider = serializerProvider;
		this._synchronizedEntityContainerService.SynchronizedEntityContainerAdded += OnContainerAdded;
		this._synchronizedEntityContainerService.SynchronizedEntityContainerRemoved += OnContainerRemoved;
	}

	private void OnContainerAdded( SynchronizedEntityContainer container ) {
		if (this._renderEntityContainersByContainerId.ContainsKey( container.ContainerId )) {
			this.LogLine( $"{container.ContainerId} already registered.", Log.Level.VERBOSE );
			return;
		}
		this._renderEntityContainersByContainerId.Add( container.ContainerId, new RenderEntityContainer( container, this._renderEntityServiceAccess ) );
	}

	private void OnContainerRemoved( SynchronizedEntityContainer container ) {
		if (!this._renderEntityContainersByContainerId.Remove( container.ContainerId, out RenderEntityContainer? renderEntityContainer ))
			return;
		renderEntityContainer.Dispose();
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		if (disposable is not RenderEntityContainer container)
			return;
		this._disposedContainers.Enqueue( container );
	}

	public IReadOnlyCollection<RenderEntityContainer> RenderEntityContainers => this._renderEntityContainersByContainerId.Values;


	protected override bool InternalDispose() {
		this._synchronizedEntityContainerService.SynchronizedEntityContainerAdded -= OnContainerAdded;
		this._synchronizedEntityContainerService.SynchronizedEntityContainerRemoved -= OnContainerRemoved;
		foreach (RenderEntityContainer renderEntityContainer in this._renderEntityContainersByContainerId.Values) {
			renderEntityContainer.OnDisposed -= OnContainerDisposed;
			renderEntityContainer.Dispose();
		}
		return true;
	}

	public void Update( double time, double deltaTime ) {
		while (this._disposedContainers.TryDequeue( out RenderEntityContainer? container )) {
			container.OnDisposed -= OnContainerDisposed;
			this._renderEntityContainersByContainerId.Remove( container.SynchronizedEntityContainer.ContainerId );
		}
		foreach (RenderEntityContainer renderEntityContainer in this._renderEntityContainersByContainerId.Values)
			renderEntityContainer.Update( time, deltaTime );
	}
}
