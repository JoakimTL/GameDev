using Engine.Module.Render.Ogl.OOP.Shaders;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ShaderPipelineService : DisposableIdentifiable {

	private readonly IInstanceProvider _pipelineProvider;
	private readonly ShaderProgramService _shaderProgramService;

	public ShaderPipelineService( ShaderProgramService shaderProgramService ) {
		this._shaderProgramService = shaderProgramService;
		this._pipelineProvider = InstanceManagement.CreateProvider( false );
		this._pipelineProvider.OnInstanceAdded += CreatePipeline;
	}

	private void CreatePipeline( object service ) {
		if (service is OglShaderPipelineBase pipeline)
			pipeline.CreatePipeline( this._shaderProgramService );
	}

	public OglShaderPipelineBase Get<T>() where T : OglShaderPipelineBase => this._pipelineProvider.Get<T>();
	public OglShaderPipelineBase? Get( Type type ) => this._pipelineProvider.Get( type ) as OglShaderPipelineBase;

	protected override bool InternalDispose() {
		this._pipelineProvider.Dispose();
		return true;
	}
}
