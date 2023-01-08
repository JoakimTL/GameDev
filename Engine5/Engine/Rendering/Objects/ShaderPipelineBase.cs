using Engine.Rendering.Services;
using OpenGL;

namespace Engine.Rendering.Objects;

public abstract class ShaderPipelineBase : Identifiable, IDisposable {
	private readonly Dictionary<ShaderType, ShaderProgramBase> _programs;
	public uint PipelineId { get; private set; }
	public IReadOnlyDictionary<ShaderType, ShaderProgramBase> Programs => this._programs;
	public abstract bool UsesTransparency { get; }

	protected ShaderPipelineBase() {
		this._programs = new Dictionary<ShaderType, ShaderProgramBase>();
		this.PipelineId = Gl.GenProgramPipeline();
	}

#if DEBUG
	~ShaderPipelineBase() {
		Debug.Fail( "Shader pipeline was not disposed!" );
	}
#endif

	protected abstract IEnumerable<ShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService );

	internal void CreatePipeline( ShaderProgramService shaderProgramService ) {
		var shaderPrograms = GetShaderPrograms( shaderProgramService );
		List<ShaderProgramBase> validPrograms = new( shaderPrograms );
		if ( validPrograms.Count == 0 )
			return;
		for ( int i = 0; i < validPrograms.Count; i++ ) {
			ShaderProgramBase prg = validPrograms[ i ];
			bool valid = true;
			foreach ( ShaderType type in prg.Sources.Keys ) {
				if ( this._programs.ContainsKey( type ) ) {
					this.LogWarning( "Cannot have multiple programs with the same mask bits active." );
					valid = false;
					break;
				}
			}
			if ( !valid )
				continue;

			foreach ( ShaderType type in prg.Sources.Keys )
				this._programs.Add( type, prg );

			Gl.UseProgramStage( this.PipelineId, prg.Mask, prg.ProgramID );
		}
	}

	public void DirectBind() => Gl.BindProgramPipeline( this.PipelineId );
	public static void DirectUnbind() => Gl.BindProgramPipeline( 0 );

	public void Dispose() {
		Gl.DeleteProgramPipelines( this.PipelineId );
		GC.SuppressFinalize( this );
	}
}

