using Engine.Rendering.Objects;
using Engine.Structure.ServiceProvider;

namespace Engine.Rendering.Services;

public sealed class ShaderProgramService : Identifiable, IContextService, IDisposable {

	private readonly RestrictedServiceProvider<ShaderProgramBase> _programProvider;
	private readonly ServiceProviderDisposalExtension _programProviderDisposer;

	public ShaderProgramService( ShaderSourceService sourceService ) {
		_programProvider = new();
		_programProviderDisposer = new( _programProvider );
		_programProvider.AddConstant( sourceService );
	}

	public ShaderProgramBase? Get( Type t ) => _programProvider.Get( t ) as ShaderProgramBase;

	public void Dispose() => _programProviderDisposer.Dispose();
}
