using Engine.Rendering.Services;
using Engine.Structure.Uid;

namespace Engine.Rendering.Objects;

public abstract class ShaderBundleBase : Identifiable {

	private static readonly InstantiableUIDTrackerUInt32 _idSystem = new();

	public uint BundleID { get; }
	private readonly Dictionary<string, ShaderPipelineBase> _pipelines;

	protected ShaderBundleBase() {
		this.BundleID = _idSystem.Next;
		_pipelines = new();
	}

	internal void CreateBundle( ShaderPipelineService pipelineService ) {
		AddPipelines(pipelineService);
	}

	protected abstract void AddPipelines( ShaderPipelineService pipelineService );

	protected void AddPipeline( string index, ShaderPipelineBase pipeline ) => _pipelines.Add( index, pipeline );

	public ShaderPipelineBase? Get( string index ) => this._pipelines.TryGetValue( index, out ShaderPipelineBase? r ) ? r : null;

}

