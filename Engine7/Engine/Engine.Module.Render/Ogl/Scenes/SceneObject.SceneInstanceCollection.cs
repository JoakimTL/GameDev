﻿using Engine.Buffers;
using Engine.Logging;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Instances here have a matching bind index, renderlayer and mesh. If any of these changes the instance should be kicked out.
/// </summary>
/// <typeparam name="TInstanceData"></typeparam>
public sealed class SceneObjectSceneInstanceCollection : DisposableIdentifiable {
	private readonly HashSet<SceneInstanceBase> _instances = [];
	private readonly BufferSlicer<BufferSegment> _subBufferManager;
	private readonly BufferSegment _segment;
	private readonly uint _maxInstanceCount;
	private readonly uint _sizePerInstanceBytes;
	public event Action? OnChanged;

	public SceneObjectSceneInstanceCollection( BufferSegment segment, uint sizePerInstanceBytes ) {
		this._subBufferManager = new( segment, sizePerInstanceBytes );
		this._segment = segment;
		this._sizePerInstanceBytes = sizePerInstanceBytes;
		this._maxInstanceCount = (uint) segment.LengthBytes / sizePerInstanceBytes;
	}

	public uint BaseInstance => (uint) (this._segment.OffsetBytes / this._sizePerInstanceBytes);
	public uint InstanceCount => (uint) this._instances.Count;
	public uint InstanceSizeBytes => this._sizePerInstanceBytes;

	public event Action<SceneInstanceBase>? OnInstanceRemoved;

	public bool TryAdd( SceneInstanceBase sceneInstance ) {
		if (!sceneInstance.MissingDataSegment) {
			this.LogWarning( "Instance already has a data segment." );
			return false;
		}
		if (this._instances.Count >= this._maxInstanceCount)
			return false;
		if (!this._instances.Add( sceneInstance ))
			return false;
		if (!this._subBufferManager.TryAllocate( this._sizePerInstanceBytes, out BufferSlice<BufferSegment>? slice )) {
			this._instances.Remove( sceneInstance );
			this.LogWarning( "Failed to allocate a slice." );
			return false;
		}
		sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
		sceneInstance.OnLayerChanged += OnLayerChanged;
		sceneInstance.OnMeshChanged += OnMeshChanged;
		sceneInstance.OnDisposed += OnInstanceDisposed;
		sceneInstance.AssignDataSegment( slice );
		OnChanged?.Invoke();
		return true;
	}

	public bool Remove( SceneInstanceBase sceneInstance ) {
		if (!this._instances.Remove( sceneInstance )) {
			this.LogWarning( "Instance was not found in the collection." );
			return false;
		}
		sceneInstance.InstanceDataSegment?.Free();
		sceneInstance.AssignDataSegment( null );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnMeshChanged -= OnMeshChanged;
		sceneInstance.OnDisposed -= OnInstanceDisposed;
		OnInstanceRemoved?.Invoke( sceneInstance );
		OnChanged?.Invoke();
		return true;
	}

	private void OnInstanceDisposed( IListenableDisposable disposable ) => Remove( (SceneInstanceBase) disposable );
	/// <summary>
	/// If the mesh changes this instance is guaranteed to no longer be compatible with this collection.
	/// </summary>
	private void OnMeshChanged( SceneInstanceBase changedInstance, IMesh? oldMesh ) => Remove( changedInstance );
	/// <summary>
	/// If the bind index changes this instance is guaranteed to no longer be compatible with this collection.
	/// </summary>
	private void OnBindIndexChanged( SceneInstanceBase changedInstance, ulong? oldBindIndex ) => Remove( changedInstance );
	/// <summary>
	/// If the layer changes this instance is guaranteed to no longer be compatible with this collection.
	/// </summary>
	private void OnLayerChanged( SceneInstanceBase changedInstance, uint oldValue ) => Remove( changedInstance );

	protected override bool InternalDispose() {
		this._segment.Dispose();
		return true;
	}
}