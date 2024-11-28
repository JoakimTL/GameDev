using Engine.Logging;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Shaders;

public abstract class OglShaderPipelineBase : DisposableIdentifiable {
	private readonly Dictionary<ShaderType, OglShaderProgramBase> _programs;
	public uint PipelineId { get; private set; }
	public IReadOnlyDictionary<ShaderType, OglShaderProgramBase> Programs => this._programs;
	public abstract bool UsesTransparency { get; }

	protected OglShaderPipelineBase() {
		this._programs = [];
		this.PipelineId = Gl.GenProgramPipeline();
	}

	protected abstract IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService );

	internal void CreatePipeline( ShaderProgramService shaderProgramService ) {
		IEnumerable<OglShaderProgramBase> shaderPrograms = GetShaderPrograms( shaderProgramService );
		List<OglShaderProgramBase> validPrograms = new( shaderPrograms );
		if (validPrograms.Count == 0)
			return;
		for (int i = 0; i < validPrograms.Count; i++) {
			OglShaderProgramBase prg = validPrograms[ i ];
			bool valid = true;
			foreach (ShaderType type in prg.Sources.Keys)
				if (this._programs.ContainsKey( type )) {
					this.LogWarning( "Cannot have multiple programs with the same mask bits active." );
					valid = false;
					break;
				}
			if (!valid)
				continue;

			foreach (ShaderType type in prg.Sources.Keys)
				this._programs.Add( type, prg );

			Gl.UseProgramStage( this.PipelineId, prg.Mask, prg.ProgramID );
		}
	}

	public void Bind() => Gl.BindProgramPipeline( this.PipelineId );
	public static void Unbind() => Gl.BindProgramPipeline( 0 );

	protected override bool InternalDispose() {
		Gl.DeleteProgramPipelines( this.PipelineId );
		return true;
	}
}
