using OpenGL;

namespace Engine.Rendering;

public class ShaderPipeline : DisposableIdentifiable {

	private readonly Dictionary<ShaderType, ShaderProgram> _programs;
	public uint PipelineId { get; private set; }
	public IReadOnlyDictionary<ShaderType, ShaderProgram> Programs => this._programs;

	public ShaderPipeline( params Type[] programs ) {
		this._programs = new Dictionary<ShaderType, ShaderProgram>();
		List<ShaderProgram> validPrograms = new();

		for ( int i = 0; i < programs.Length; i++ ) {
			ShaderProgram p = Resources.Render.Shader.Programs.Get( programs[ i ] );
			if ( p is not null )
				validPrograms.Add( p );
		}

		if ( validPrograms.Count == 0 )
			return;

		this.PipelineId = Gl.GenProgramPipeline();

		for ( int i = 0; i < validPrograms.Count; i++ ) {
			ShaderProgram prg = validPrograms[ i ];
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

	protected override bool OnDispose() {
		Gl.DeleteProgramPipelines( this.PipelineId );
		return true;
	}
}
