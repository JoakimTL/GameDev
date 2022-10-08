using Engine.Structure;

namespace Engine.Rendering.Services;

[Structure.ProcessBefore( typeof( WindowService ), typeof( IDisposable ) )]
[Structure.ProcessAfter( typeof( WindowService ), typeof( IInitializable ) )]
public class ShaderManager : ModuleService, IInitializable {

	public readonly ShaderSourceManager Sources;
	public readonly ServiceProvider<ShaderProgram> Programs;
	public readonly ServiceProvider<ShaderPipeline> Pipelines;
	public readonly ShaderBundleManager Bundles;

	public ShaderManager() {
		this.Sources = new ShaderSourceManager( "res/shaders" );
		this.Programs = new();
		this.Pipelines = new();
		this.Bundles = new ShaderBundleManager();
	}

	public void Initialize() => this.Bundles.Initialize();

	protected override bool OnDispose() {
		this.Pipelines.Dispose();
		this.Programs.Dispose();
		this.Sources.Dispose();
		return true;
	}
}
