using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.OOP.Shaders;

public abstract class ShaderBundleBase : Identifiable {

	private static uint _uniqueIdCounter;
	private static uint GetNextUniqueId() => Interlocked.Increment( ref _uniqueIdCounter );

	public uint BundleID { get; }
	private readonly Dictionary<string, OglShaderPipelineBase> _pipelines;

	protected ShaderBundleBase() {
		this.BundleID = GetNextUniqueId();
		this._pipelines = new();
	}

	internal void CreateBundle( ShaderPipelineService pipelineService ) => AddPipelines( pipelineService );

	protected abstract void AddPipelines( ShaderPipelineService pipelineService );

	protected void AddPipeline( string index, OglShaderPipelineBase pipeline ) => this._pipelines.Add( index, pipeline );

	public OglShaderPipelineBase? Get( string index ) => this._pipelines.TryGetValue( index, out OglShaderPipelineBase? r ) ? r : null;

	public IEnumerable<string> AllIndices => this._pipelines.Keys;

}