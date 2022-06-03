using Engine.Rendering.Pipelines;
using Engine.Rendering.ResourceManagement;
using Engine.Structure;

namespace Engine.Rendering;
public class RenderResources : DisposableIdentifiable, IUpdateable {

	private readonly UpdateableSingletonProvider<object> _singletonProvider;

	public Window Window => this._singletonProvider.Get<Window>();

	public ShaderManager Shader => this._singletonProvider.Get<ShaderManager>();
	public TextureManager Textures => this._singletonProvider.Get<TextureManager>();
	public VertexBufferObjectManager VBOs => this._singletonProvider.Get<VertexBufferObjectManager>();
	public RenderDataObjectManager RDOs => this._singletonProvider.Get<RenderDataObjectManager>();
	public CompositeVertexArrayObjectDataLayoutManager VAODataLayouts => this._singletonProvider.Get<CompositeVertexArrayObjectDataLayoutManager>();
	public CompositeVertexLayoutBindingManager VAOLayoutBindings => this._singletonProvider.Get<CompositeVertexLayoutBindingManager>();
	public CompositeVertexArrayObjectManager CompositeVAOs => this._singletonProvider.Get<CompositeVertexArrayObjectManager>();
	public Mesh2Manager Mesh2 => this._singletonProvider.Get<Mesh2Manager>();
	public Mesh3Manager Mesh3 => this._singletonProvider.Get<Mesh3Manager>();
	public BufferedMeshManager BufferedMesh => this._singletonProvider.Get<BufferedMeshManager>();
	public ContextUpdateManager ContextUpdate => this._singletonProvider.Get<ContextUpdateManager>();
	public ContextObjectDisposer ContextDiposer => this._singletonProvider.Get<ContextObjectDisposer>();
	public ContextTaskManager ContextTask => this._singletonProvider.Get<ContextTaskManager>();
	public FrameDebugDataProvider FrameDebugData => this._singletonProvider.Get<FrameDebugDataProvider>();
	public RenderPipelineManager PipelineManager => this._singletonProvider.Get<RenderPipelineManager>();

	public Thread? ContextThread { get; private set; }
	public bool InThread => Thread.CurrentThread == this.ContextThread;
	public bool Active => true;

	public RenderResources() {
		this._singletonProvider = new();
		Log.Line( BitConverter.IsLittleEndian ? "System is little endian" : "System is big endian", Log.Level.NORMAL );
	}

	public void InitializeGl() {
		this._singletonProvider.Get<Window>();
		this.ContextThread = Thread.CurrentThread;
		foreach ( IContextInitializable? contextIntializedSingleton in this._singletonProvider.GetMultiple<IContextInitializable>() )
			contextIntializedSingleton.InitializeInContext();
	}

	public void Update( float time, float deltaTime ) {
		this._singletonProvider.Update( time, deltaTime );
		this.Window?.SetTitle( this.FrameDebugData.FrameRateInformation );
	}

	public T Get<T>() where T : Identifiable => this._singletonProvider.Get<T>();

	protected override bool OnDispose() {
		this._singletonProvider.Dispose();
		return true;
	}
}
