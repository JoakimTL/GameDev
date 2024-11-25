using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.OOP;

public abstract class ShaderBundleBase : Identifiable {

	private static uint _uniqueIdCounter;
	private static uint GetNextUniqueId() => Interlocked.Increment( ref _uniqueIdCounter );

	public uint BundleID { get; }
	private readonly Dictionary<string, OglShaderPipelineBase> _pipelines;

	protected ShaderBundleBase() {
		BundleID = GetNextUniqueId();
		_pipelines = new();
	}

	internal void CreateBundle( ShaderPipelineService pipelineService ) => AddPipelines( pipelineService );

	protected abstract void AddPipelines( ShaderPipelineService pipelineService );

	protected void AddPipeline( string index, OglShaderPipelineBase pipeline ) => _pipelines.Add( index, pipeline );

	public OglShaderPipelineBase? Get( string index ) => _pipelines.TryGetValue( index, out OglShaderPipelineBase? r ) ? r : null;

	public IEnumerable<string> AllIndices => _pipelines.Keys;

}