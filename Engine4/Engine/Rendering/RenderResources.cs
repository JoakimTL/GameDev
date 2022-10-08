using Engine.Rendering.Pipelines;
using Engine.Rendering.Services;
using Engine.Structure;

namespace Engine.Rendering;
public class RenderResources : DisposableIdentifiable, IUpdateable {

	private readonly ServiceProvider<object> _serviceProvider;

	public Window Window => this._serviceProvider.GetOrAdd<Window>();

	public ShaderManager Shader => this._serviceProvider.GetOrAdd<ShaderManager>();
	public TextureManager Textures => this._serviceProvider.GetOrAdd<TextureManager>();
	public VertexBufferObjectManager VBOs => this._serviceProvider.GetOrAdd<VertexBufferObjectManager>();
	public RenderDataObjectManager RDOs => this._serviceProvider.GetOrAdd<RenderDataObjectManager>();
	public CompositeVertexArrayObjectDataLayoutManager VAODataLayouts => this._serviceProvider.GetOrAdd<CompositeVertexArrayObjectDataLayoutManager>();
	public CompositeVertexLayoutBindingManager VAOLayoutBindings => this._serviceProvider.GetOrAdd<CompositeVertexLayoutBindingManager>();
	public CompositeVertexArrayObjectManager CompositeVAOs => this._serviceProvider.GetOrAdd<CompositeVertexArrayObjectManager>();
	public Mesh2Manager Mesh2 => this._serviceProvider.GetOrAdd<Mesh2Manager>();
	public Mesh3Manager Mesh3 => this._serviceProvider.GetOrAdd<Mesh3Manager>();
	public BufferedMeshManager BufferedMesh => this._serviceProvider.GetOrAdd<BufferedMeshManager>();
	//public ContextUpdateManager ContextUpdate => this._serviceProvider.GetOrAdd<ContextUpdateManager>();
	public ContextObjectDisposer ContextDiposer => this._serviceProvider.GetOrAdd<ContextObjectDisposer>();
	public ContextTaskManager ContextTask => this._serviceProvider.GetOrAdd<ContextTaskManager>();
	public FrameDebugDataProvider FrameDebugData => this._serviceProvider.GetOrAdd<FrameDebugDataProvider>();
	public RenderPipelineManager PipelineManager => this._serviceProvider.GetOrAdd<RenderPipelineManager>();
	public FontManager FontManager => this._serviceProvider.GetOrAdd<FontManager>();

	public bool InThread => Thread.CurrentThread == this.Window.ContextThread;
	public bool Active => true;

	public RenderResources() {
		this._serviceProvider = new();
		Log.Line( BitConverter.IsLittleEndian ? "System is little endian" : "System is big endian", Log.Level.NORMAL );
	}

	public void InitializeGl() => this._serviceProvider.GetOrAdd<Window>();

	public void Update( float time, float deltaTime ) {
		this._serviceProvider.Update( time, deltaTime );
		this.Window?.SetTitle( this.FrameDebugData.FrameRateInformation );
	}

	protected override bool OnDispose() {
		this._serviceProvider.Dispose();
		return true;
	}
}
