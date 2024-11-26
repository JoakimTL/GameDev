using Engine.Module.Render.Ogl.OOP.Shaders;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ShaderProgramService : DisposableIdentifiable {

	private readonly IInstanceProvider _programProvider;
	private readonly ShaderSourceService _sourceService;

	public ShaderProgramService( ShaderSourceService sourceService ) {
		_sourceService = sourceService;
		_programProvider = InstanceManagement.CreateProvider();
		_programProvider.OnInstanceAdded += CreateProgram;
	}

	private void CreateProgram( object service ) {
		if (service is OglShaderProgramBase program)
			program.CreateProgram( _sourceService );
	}

	public OglShaderProgramBase Get<T>() where T : OglShaderProgramBase => _programProvider.Get<T>();
	public OglShaderProgramBase? Get( Type t ) => _programProvider.Get( t ) as OglShaderProgramBase;

	protected override bool InternalDispose() {
		_programProvider.Dispose();
		return true;
	}
}
