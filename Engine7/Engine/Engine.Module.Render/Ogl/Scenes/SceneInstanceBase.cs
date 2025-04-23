using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

public abstract class SceneInstanceBase(Type instanceType) : Identifiable, IRemovable {

	public delegate void SceneInstanceBindIndexChangeHandler( SceneInstanceBase changedInstance, ulong? oldBindIndex );
	public delegate void SceneInstancePropertyChangeHandler<T>( SceneInstanceBase changedInstance, T? oldValue );
	public delegate void SceneInstanceChangeHandler( SceneInstanceBase changedInstance );

	public OglVertexArrayObjectBase? VertexArrayObject { get; private set; }
	public ShaderBundleBase? ShaderBundle { get; private set; }
	public ulong? BindIndex { get; private set; }

	protected internal BufferSlice<BufferSegment>? InstanceDataSegment { get; private set; }
	public IMesh? Mesh { get; private set; }
	/// <summary>
	/// The layers control when a sceneobject is rendered. Layers are rendered lowest to highest. Beware of utilizing too many layers for no reason, each layer causes a DrawCall per unique sceneobject.
	/// </summary>
	public uint RenderLayer { get; private set; }
	/// <summary>
	/// If true the instance will be rendered on screen.
	/// </summary>
	public bool Valid { get; private set; }

	/// <summary>
	/// If true the instance will be rendered on screen. When an instance is inactive it loses it's instance data segment and is not rendered, but doesn't lose it's spot in the scene. This means it's much faster to reactivate than reinsert.
	/// </summary>
	public bool Allocated { get; private set; } = true;
	public bool Removed { get; private set; } = false;

	public Type InstanceDataType { get; } = instanceType;

	/// <summary>
	/// Called when the bind index changes. The old bind index is passed as the second parameter.
	/// </summary>
	public event SceneInstanceBindIndexChangeHandler? OnBindIndexChanged;
	/// <summary>
	/// Called when the mesh changes.
	/// </summary>
	public event SceneInstancePropertyChangeHandler<IMesh>? OnMeshChanged;
	/// <summary>
	/// Called when the mesh changes.
	/// </summary>
	public event SceneInstanceChangeHandler? OnMeshOffsetChanged;
	/// <summary>
	/// Called when the instance data segment changes.
	/// </summary>
	public event SceneInstancePropertyChangeHandler<BufferSlice<BufferSegment>>? OnInstanceDataSegmentChanged;
	/// <summary>
	/// Called when the instance layer changes.
	/// </summary>
	public event SceneInstancePropertyChangeHandler<uint>? OnLayerChanged;
	/// <summary>
	/// Called when the instance is allocated (<c>true</c>) or deallocated (<c>false</c>).
	/// </summary>
	public event SceneInstancePropertyChangeHandler<bool>? OnAllocatedChanged;
	/// <summary>
	/// Called when the instance should be removed from all scenes.
	/// </summary>
	public event RemovalHandler? OnRemoved;

	protected void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) {
		if (this.VertexArrayObject == vertexArrayObject)
			return;
		this.VertexArrayObject = vertexArrayObject;
		ulong? oldBindIndex = this.BindIndex;
		UpdateValidity();
		this.BindIndex = this.ShaderBundle is not null ? this.VertexArrayObject?.GetBindIndexWith( this.ShaderBundle ) : null;
		if (oldBindIndex != this.BindIndex)
			OnBindIndexChanged?.Invoke( this, oldBindIndex );
	}

	protected void SetShaderBundle( ShaderBundleBase? shaderBundle ) {
		if (this.ShaderBundle == shaderBundle)
			return;
		this.ShaderBundle = shaderBundle;
		ulong? oldBindIndex = this.BindIndex;
		UpdateValidity();
		this.BindIndex = this.VertexArrayObject is not null ? this.ShaderBundle?.GetBindIndexWith( this.VertexArrayObject ) : null;
		if (oldBindIndex != this.BindIndex)
			OnBindIndexChanged?.Invoke( this, oldBindIndex );
	}

	protected void SetMesh( IMesh? mesh ) {
		if (this.Mesh == mesh)
			return;
		IMesh? oldMesh = this.Mesh;
		if (this.Mesh is not null)
			this.Mesh.OnOffsetChanged -= InvokeOnMeshOffsetChanged;
		this.Mesh = mesh;
		if (this.Mesh is not null)
			this.Mesh.OnOffsetChanged += InvokeOnMeshOffsetChanged;
		UpdateValidity();
		OnMeshChanged?.Invoke( this, oldMesh );
	}

	private void InvokeOnMeshOffsetChanged() => OnMeshOffsetChanged?.Invoke( this );

	protected internal void SetLayer( uint layer ) {
		if (this.RenderLayer == layer)
			return;
		uint oldLayer = this.RenderLayer;
		this.RenderLayer = layer;
		OnLayerChanged?.Invoke( this, oldLayer );
	}

	protected void SetAllocated(bool allocated) {
		if (this.Allocated == allocated)
			return;
		this.Allocated = allocated;
		UpdateValidity();
		OnAllocatedChanged?.Invoke( this, !allocated );
	}

	public void Remove() {
		if (this.Removed)
			return;
		this.Removed = true;
		UpdateValidity();
		OnRemoved?.Invoke( this );
	}

	internal void AssignDataSegment( BufferSlice<BufferSegment>? segment ) {
		if (this.InstanceDataSegment == segment)
			return;
		BufferSlice<BufferSegment>? oldSegment = this.InstanceDataSegment;
		this.InstanceDataSegment = segment;
		UpdateValidity();
		OnInstanceDataSegmentChanged?.Invoke( this, oldSegment );
	}

	protected abstract void Initialize();
	internal void Setup() => Initialize();

	protected bool Write<TInstanceData>( TInstanceData data ) where TInstanceData : unmanaged => this.InstanceDataSegment?.Write<ulong, TInstanceData>( 0, data ) ?? false;
	protected bool TryRead<TInstanceData>( out TInstanceData data ) where TInstanceData : unmanaged {
		data = default;
		return this.InstanceDataSegment?.Read<ulong, TInstanceData>( 0, out data ) ?? false;
	}

	public bool MissingDataSegment => !Removed && Allocated && this.VertexArrayObject is not null && this.ShaderBundle is not null && this.Mesh is not null && this.InstanceDataSegment is null;

	private void UpdateValidity() => this.Valid = !Removed && Allocated && this.VertexArrayObject is not null && this.ShaderBundle is not null && this.Mesh is not null && this.InstanceDataSegment is not null;
}