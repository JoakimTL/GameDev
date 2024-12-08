using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

public abstract class SceneInstanceBase(Type instanceType) : DisposableIdentifiable {

	public delegate void SceneInstanceBindIndexChangeHandler( SceneInstanceBase changedInstance, ulong? oldBindIndex );
	public delegate void SceneInstancePropertyChangeHandler<T>( SceneInstanceBase changedInstance, T? oldValue );

	public OglVertexArrayObjectBase? VertexArrayObject { get; private set; }
	public ShaderBundleBase? ShaderBundle { get; private set; }
	public ulong? BindIndex { get; private set; }

	protected internal SubBuffer<BufferSegment>? InstanceDataSegment { get; private set; }
	public IMesh? Mesh { get; private set; }
	/// <summary>
	/// The layers control when a sceneobject is rendered. Layers are rendered lowest to highest. Beware of utilizing too many layers for no reason, each layer causes a DrawCall per unique sceneobject.
	/// </summary>
	public uint RenderLayer { get; private set; }
	/// <summary>
	/// If true the instance will be rendered on screen.
	/// </summary>
	public bool Valid { get; private set; }

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
	/// Called when the instance data segment changes.
	/// </summary>
	public event SceneInstancePropertyChangeHandler<SubBuffer<BufferSegment>>? OnInstanceDataSegmentChanged;
	/// <summary>
	/// Called when the instance layer changes.
	/// </summary>
	public event SceneInstancePropertyChangeHandler<uint>? OnLayerChanged;

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
		this.Mesh = mesh;
		UpdateValidity();
		OnMeshChanged?.Invoke( this, oldMesh );
	}

	protected internal void SetLayer( uint layer ) {
		if (this.RenderLayer == layer)
			return;
		uint oldLayer = this.RenderLayer;
		this.RenderLayer = layer;
		OnLayerChanged?.Invoke( this, oldLayer );
	}

	internal void AssignDataSegment( SubBuffer<BufferSegment>? segment ) {
		if (this.InstanceDataSegment == segment)
			return;
		SubBuffer<BufferSegment>? oldSegment = this.InstanceDataSegment;
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

	public bool MissingDataSegment => this.VertexArrayObject is not null && this.ShaderBundle is not null && this.Mesh is not null && this.InstanceDataSegment is null;

	private void UpdateValidity() => this.Valid = this.VertexArrayObject is not null && this.ShaderBundle is not null && this.Mesh is not null && this.InstanceDataSegment is not null;

	protected override bool InternalDispose() {
		this.InstanceDataSegment?.Dispose();
		return true;
	}
}