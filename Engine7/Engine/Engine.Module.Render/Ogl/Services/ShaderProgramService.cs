using Engine.Module.Render.Ogl.OOP.Shaders;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ShaderProgramService : DisposableIdentifiable {

	private readonly IInstanceProvider _programProvider;
	private readonly ShaderSourceService _sourceService;

	public ShaderProgramService( ShaderSourceService sourceService ) {
		this._sourceService = sourceService;
		this._programProvider = InstanceManagement.CreateProvider();
		this._programProvider.OnInstanceAdded += CreateProgram;
	}

	private void CreateProgram( object service ) {
		if (service is OglShaderProgramBase program)
			program.CreateProgram( this._sourceService );
	}

	public OglShaderProgramBase Get<T>() where T : OglShaderProgramBase => this._programProvider.Get<T>();
	public OglShaderProgramBase? Get( Type t ) => this._programProvider.Get( t ) as OglShaderProgramBase;

	protected override bool InternalDispose() {
		this._programProvider.Dispose();
		return true;
	}
}
