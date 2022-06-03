using Engine.Modularity.Modules;
using Engine.Structure;

namespace Engine.Rendering.ResourceManagement;

[ProcessBefore( typeof( Window ), typeof( IDisposable ) )]
public class ShaderManager : ModuleSingletonBase, IContextInitializable {

	public readonly ShaderSourceManager Sources;
	public readonly SingletonProvider<ShaderProgram> Programs;
	public readonly SingletonProvider<ShaderPipeline> Pipelines;
	public readonly ShaderBundleManager Bundles;

	public ShaderManager() {
		this.Sources = new ShaderSourceManager( "res/shaders" );
		this.Programs = new();
		this.Pipelines = new();
		this.Bundles = new ShaderBundleManager();
	}

	public void InitializeInContext() => this.Bundles.Initialize();

	protected override bool OnDispose() {
		this.Pipelines.Dispose();
		this.Programs.Dispose();
		this.Sources.Dispose();
		return true;
	}
}
