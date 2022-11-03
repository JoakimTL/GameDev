using Engine.Rendering.Objects;
using Engine.Structure.ServiceProvider;

namespace Engine.Rendering.Services;

public sealed class ShaderPipelineService : Identifiable, IContextService, IDisposable {

	private readonly RestrictedServiceProvider<ShaderPipelineBase> _pipelineProvider;
	private readonly ServiceProviderDisposalExtension _pipelineProviderDisposer;

	public ShaderPipelineService( ShaderProgramService programService ) {
		_pipelineProvider = new();
		_pipelineProviderDisposer = new( _pipelineProvider );
		_pipelineProvider.AddConstant( programService );
	}

	public ShaderPipelineBase? Get( Type type ) => _pipelineProvider.Get( type ) as ShaderPipelineBase;

	public void Dispose() => _pipelineProviderDisposer.Dispose();
}
