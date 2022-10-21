using Engine.Rendering.Objects;
using Engine.Structure;

namespace Engine.Rendering.Services;

public sealed class ShaderPipelineService : Identifiable, IContextService, IDisposable {

	private readonly RestrictedServiceProvider<ShaderPipelineBase> _pipelineProvider;
	private readonly ServiceProviderDisposalExtension _pipelineProviderDisposer;

	public ShaderPipelineService( ShaderProgramService programService ) {
		_pipelineProvider = new();
		_pipelineProviderDisposer = new( _pipelineProvider );
		_pipelineProvider.AddConstant( programService );
	}

	public ShaderPipelineBase? Get( string identity ) {
		var type = Global.Get<TypeIdentificationService>().GetFromIdentity( identity );
		if ( type is null ) {
			this.LogWarning( $"Unable to load shader pipeline type from {identity}." );
			return null;
		}
		return _pipelineProvider.Get( type ) as ShaderPipelineBase;
	}

	public ShaderPipelineBase GetOrFail( string identity ) {
		var type = Global.Get<TypeIdentificationService>().GetFromIdentity( identity );
		if ( type is null ) 
			throw new NullReferenceException( $"Unable to load shader pipeline type from {identity}." );
		return _pipelineProvider.Get( type ) as ShaderPipelineBase ?? throw new NullReferenceException( $"Unable to cast to shader pipeline from {identity}." );
	}

	public void Dispose() => _pipelineProviderDisposer.Dispose();
}
