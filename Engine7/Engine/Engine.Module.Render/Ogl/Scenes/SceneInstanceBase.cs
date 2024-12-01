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
		if (VertexArrayObject == vertexArrayObject)
			return;
		VertexArrayObject = vertexArrayObject;
		ulong? oldBindIndex = BindIndex;
		UpdateValidity();
		BindIndex = ShaderBundle is not null ? VertexArrayObject?.GetBindIndexWith( ShaderBundle ) : null;
		if (oldBindIndex != BindIndex)
			OnBindIndexChanged?.Invoke( this, oldBindIndex );
	}

	protected void SetShaderBundle( ShaderBundleBase? shaderBundle ) {
		if (ShaderBundle == shaderBundle)
			return;
		ShaderBundle = shaderBundle;
		ulong? oldBindIndex = BindIndex;
		UpdateValidity();
		BindIndex = VertexArrayObject is not null ? ShaderBundle?.GetBindIndexWith( VertexArrayObject ) : null;
		if (oldBindIndex != BindIndex)
			OnBindIndexChanged?.Invoke( this, oldBindIndex );
	}

	protected void SetMesh( IMesh? mesh ) {
		if (Mesh == mesh)
			return;
		IMesh? oldMesh = Mesh;
		Mesh = mesh;
		UpdateValidity();
		OnMeshChanged?.Invoke( this, oldMesh );
	}

	protected internal void SetLayer( uint layer ) {
		if (RenderLayer == layer)
			return;
		uint oldLayer = RenderLayer;
		RenderLayer = layer;
		OnLayerChanged?.Invoke( this, oldLayer );
	}

	internal void AssignDataSegment( SubBuffer<BufferSegment>? segment ) {
		if (InstanceDataSegment == segment)
			return;
		SubBuffer<BufferSegment>? oldSegment = InstanceDataSegment;
		InstanceDataSegment = segment;
		UpdateValidity();
		OnInstanceDataSegmentChanged?.Invoke( this, oldSegment );
	}

	protected abstract void Initialize();
	internal void Setup() => Initialize();

	protected bool Write<TInstanceData>( TInstanceData data ) where TInstanceData : unmanaged => InstanceDataSegment?.Write<ulong, TInstanceData>( 0, data ) ?? false;
	protected bool TryRead<TInstanceData>( out TInstanceData data ) where TInstanceData : unmanaged {
		data = default;
		return InstanceDataSegment?.Read<ulong, TInstanceData>( 0, out data ) ?? false;
	}

	public bool MissingDataSegment => VertexArrayObject is not null && ShaderBundle is not null && Mesh is not null && InstanceDataSegment is null;

	private void UpdateValidity() => Valid = VertexArrayObject is not null && ShaderBundle is not null && Mesh is not null && InstanceDataSegment is not null;

	protected override bool InternalDispose() {
		InstanceDataSegment?.Dispose();
		return true;
	}
}

public class SceneInstance<TVertexData, TInstanceData>() : SceneInstanceBase( typeof( TInstanceData ) )
	where TVertexData : unmanaged
	where TInstanceData : unmanaged {
	protected override void Initialize() {

	}
}