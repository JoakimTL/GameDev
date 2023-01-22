using Engine.Rendering.Objects;
using Engine.Structure.ServiceProvider;

namespace Engine.Rendering.Services;

public sealed class ShaderProgramService : Identifiable, IContextService, IDisposable {

	private readonly RestrictedServiceProvider<ShaderProgramBase> _programProvider;
	private readonly ServiceProviderDisposalExtension _programProviderDisposer;
	private readonly ShaderSourceService _sourceService;

	public ShaderProgramService( ShaderSourceService sourceService ) {
		this._sourceService = sourceService;
		_programProvider = new();
		_programProvider.ServiceAdded += CreateProgram;
		_programProviderDisposer = new( _programProvider );
	}

	private void CreateProgram( object service ) {
		if ( service is ShaderProgramBase program )
			program.CreateProgram( _sourceService );
	}

	public ShaderProgramBase Get<T>() where T : ShaderProgramBase => _programProvider.Get<T>();
	public ShaderProgramBase? Get( Type t ) => _programProvider.Get( t ) as ShaderProgramBase;

	public void Dispose() => _programProviderDisposer.Dispose();
}
