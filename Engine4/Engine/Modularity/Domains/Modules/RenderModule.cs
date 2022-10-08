using Engine.Rendering;
using Engine.Rendering.Pipelines;
using Engine.Rendering.Services;

namespace Engine.Modularity.Domains.Modules;

[System]
public sealed class RenderModule : Module {

	//Todo: turn rendering submodules into services.

	public Window Window => ModuleService<WindowService>().Window;

	public ShaderManager Shader => ModuleService<ShaderManager>();
	public TextureManager Textures => ModuleService<TextureManager>();
	public VertexBufferObjectManager VBOs => ModuleService<VertexBufferObjectManager>();
	public RenderDataObjectManager RDOs => ModuleService<RenderDataObjectManager>();
	public CompositeVertexArrayObjectDataLayoutManager VAODataLayouts => ModuleService<CompositeVertexArrayObjectDataLayoutManager>();
	public CompositeVertexLayoutBindingManager VAOLayoutBindings => ModuleService<CompositeVertexLayoutBindingManager>();
	public CompositeVertexArrayObjectManager CompositeVAOs => ModuleService<CompositeVertexArrayObjectManager>();
	public Mesh2Manager Mesh2 => ModuleService<Mesh2Manager>();
	public Mesh3Manager Mesh3 => ModuleService<Mesh3Manager>();
	public BufferedMeshManager BufferedMesh => ModuleService<BufferedMeshManager>();
	//public ContextUpdateManager ContextUpdate => ModuleService<ContextUpdateManager>();
	public ContextObjectDisposer ContextDiposer => ModuleService<ContextObjectDisposer>();
	public ContextTaskManager ContextTask => ModuleService<ContextTaskManager>();
	public FrameDebugDataProvider FrameDebugData => ModuleService<FrameDebugDataProvider>();
	public RenderPipelineManager PipelineManager => ModuleService<RenderPipelineManager>();
	public FontManager FontManager => ModuleService<FontManager>();

	public bool InThread => Thread.CurrentThread == this.Window.ContextThread;

	public RenderModule() : base( true, 0 ) { }

	protected override void ModuleUpdate( float time, float deltaTime ) {
		this.FrameDebugData.Update( time, deltaTime );
		this.PipelineManager.Render();
		this.Window.SwapBuffers();
	}
}
