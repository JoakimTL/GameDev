using Engine.Rendering.Objects;
using Engine.Structure.ServiceProvider;

namespace Engine.Rendering.Services;

public sealed class ShaderPipelineService : Identifiable, IContextService, IDisposable {

	private readonly RestrictedServiceProvider<ShaderPipelineBase> _pipelineProvider;
	private readonly ServiceProviderDisposalExtension _pipelineProviderDisposer;
	private readonly ShaderProgramService _shaderProgramService;

	public ShaderPipelineService( ShaderProgramService shaderProgramService ) {
		this._shaderProgramService = shaderProgramService;
		_pipelineProvider = new();
		_pipelineProvider.ServiceAdded += CreatePipeline;
		_pipelineProviderDisposer = new( _pipelineProvider );
	}

	private void CreatePipeline( object service ) {
		if ( service is ShaderPipelineBase pipeline )
			pipeline.CreatePipeline( _shaderProgramService );
	}

	public ShaderPipelineBase Get<T>() where T : ShaderPipelineBase => _pipelineProvider.Get<T>();
	public ShaderPipelineBase? Get( Type type ) => _pipelineProvider.Get( type ) as ShaderPipelineBase;

	public void Dispose() => _pipelineProviderDisposer.Dispose();
}
