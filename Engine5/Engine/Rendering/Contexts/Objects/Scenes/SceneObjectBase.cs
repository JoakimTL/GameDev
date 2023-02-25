namespace Engine.Rendering.Contexts.Objects.Scenes;

public abstract class SceneObjectBase : Identifiable, ISceneObject, IDisposable {

	public VertexArrayObjectBase VertexArrayObject { get; }

	public ShaderBundleBase? ShaderBundle { get; private set; }
	public IMesh? Mesh { get; private set; }
	public ISceneInstanceData? SceneData { get; private set; }

	public ulong SortingIndex { get; private set; }
	public uint Layer { get; private set; } //NOT IMPLEMENTED
	public bool Valid { get; private set; }

	public event Action<ISceneObject>? RenderPropertiesChanged;
	public event Action<ISceneObject>? SceneObjectDisposed;

	public SceneObjectBase( VertexArrayObjectBase vao ) {
		VertexArrayObject = vao;
	}

	private void SetSortingIndex() {
		SortingIndex = 0;
		if ( ShaderBundle is null || VertexArrayObject is null )
			return;
		SortingIndex = (ulong) ShaderBundle.BundleID << 32 | VertexArrayObject.VAOID;
	}

	private void CheckValidity() => Valid = SortingIndex > 0 && Mesh is not null && SceneData is not null;

	private void DataChanged() => RenderPropertiesChanged?.Invoke( this );

	protected void SetMesh( IMesh? newMesh ) {
		if ( Mesh == newMesh )
			return;
		if ( Mesh is not null )
			Mesh.Changed -= DataChanged;
		Mesh = newMesh;
		if ( Mesh is not null )
			Mesh.Changed += DataChanged;
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}


	protected void SetSceneData( ISceneInstanceData? newSceneData ) {
		if ( SceneData == newSceneData )
			return;
		if ( SceneData is not null ) {
			SceneData.Changed -= DataChanged;
		}
		SceneData = newSceneData;
		if ( SceneData is not null ) {
			SceneData.Changed += DataChanged;
		}
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}

	protected void SetShaders( ShaderBundleBase? newShaderBundle ) {
		if ( ShaderBundle == newShaderBundle )
			return;
		ShaderBundle = newShaderBundle;
		SetSortingIndex();
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}

	public bool TryGetIndirectCommand( out IndirectCommand? command ) {
		command = null;
		if ( !Valid || Mesh is null || SceneData is null || SceneData.ActiveInstances == 0 )
			return false;
		command = new(
			Mesh.ElementCount,
			SceneData.ActiveInstances,
			Mesh.ElementOffset,
			Mesh.VertexOffset,
			SceneData.DataOffset
		);
		return true;
	}

	public abstract void Bind();

	public void Dispose() {
		SceneData?.Dispose();
		SceneObjectDisposed?.Invoke( this );
	}
}
