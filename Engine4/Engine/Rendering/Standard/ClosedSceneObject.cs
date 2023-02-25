using System.Runtime.InteropServices;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace Engine.Rendering.Standard;
public abstract class ClosedSceneObject<V, SD> : DisposableIdentifiable, ISceneObject where V : unmanaged where SD : unmanaged {

	public VertexArrayObject VertexArrayObject { get; }

	public ShaderBundle? ShaderBundle { get; private set; }
	public BufferedMesh? Mesh { get; private set; }
	public SceneInstanceData<SD>? SceneData { get; private set; }

	public ulong SortingIndex { get; private set; }
	public uint Layer { get; private set; } //NOT IMPLEMENTED
	public bool Valid { get; private set; }
	public bool HasTransparency { get; private set; }

	public event Action<ISceneObject>? RenderPropertiesChanged;

	public event Action<ISceneObject>? SceneObjectDisposed;

	public ClosedSceneObject() {
		if ( !Resources.Render.CompositeVAOs.TryGet<V, SD>( out CompositeVertexArrayObject? vao ) )
			throw new Exception( "Unable to get VAO." );
		this.VertexArrayObject = vao;
	}

	private void SetSortingIndex() {
		this.SortingIndex = 0;
		if ( this.ShaderBundle is null || this.VertexArrayObject is null )
			return;
		this.SortingIndex = ((ulong) this.ShaderBundle.BundleID << 32) | this.VertexArrayObject.VAOID;
	}

	private void CheckValidity() => this.Valid = this.SortingIndex > 0 && this.Mesh is not null && this.SceneData is not null;

	protected void SetMesh( BufferedMesh? newMesh ) {
		if ( this.Mesh == newMesh )
			return;
		if ( this.Mesh is not null )
			this.Mesh.VertexSegmentData.OffsetChanged -= OffsetChanged;
		this.Mesh = newMesh;
		if ( this.Mesh is not null )
			this.Mesh.VertexSegmentData.OffsetChanged += OffsetChanged;
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}

	private void OffsetChanged( ulong obj ) => RenderPropertiesChanged?.Invoke( this );

	protected void SetSceneData( SceneInstanceData<SD>? newSceneData ) {
		if ( this.SceneData == newSceneData )
			return;
		if ( this.SceneData is not null ) {
			this.SceneData.ActiveInstanceCountChanged -= ActiveInstanceCountChanged;
			this.SceneData.SegmentData.OffsetChanged -= OffsetChanged;
		}
		this.SceneData = newSceneData;
		if ( this.SceneData is not null ) {
			this.SceneData.ActiveInstanceCountChanged += ActiveInstanceCountChanged;
			this.SceneData.SegmentData.OffsetChanged += OffsetChanged;
		}
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}

	private void ActiveInstanceCountChanged() => RenderPropertiesChanged?.Invoke( this );

	protected void SetShaders( ShaderBundle? newShaderBundle ) {
		if ( this.ShaderBundle == newShaderBundle )
			return;
		this.ShaderBundle = newShaderBundle;
		this.HasTransparency = newShaderBundle?.UsesTransparency ?? false;
		SetSortingIndex();
		CheckValidity();
		RenderPropertiesChanged?.Invoke( this );
	}

	public bool TryGetIndirectCommand( out IndirectCommand? command ) {
		command = null;
		if ( !this.Valid || this.Mesh is null || this.SceneData is null || this.SceneData.ActiveInstances == 0 )
			return false;
		command = new(
			this.Mesh.ElementSegmentData.SizeBytes / sizeof( uint ),
			this.SceneData.ActiveInstances,
			(uint) ( this.Mesh.ElementSegmentData.OffsetBytes / sizeof( uint ) ),
			(uint) ( this.Mesh.VertexSegmentData.OffsetBytes / (uint) Marshal.SizeOf<V>() ),
			(uint) ( this.SceneData.SegmentData.OffsetBytes / (uint) Marshal.SizeOf<SD>() )
		);
		return true;
	}

	public abstract void Bind();

	protected override bool OnDispose() {
		this.SceneData?.Dispose();
		SceneObjectDisposed?.Invoke( this );
		return true;
	}
}