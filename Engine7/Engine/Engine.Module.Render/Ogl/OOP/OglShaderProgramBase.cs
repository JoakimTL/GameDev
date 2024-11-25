using Engine.Logging;
using Engine.Module.Render.Ogl.Services;
using OpenGL;
using System.Text;

namespace Engine.Module.Render.Ogl.OOP;

public abstract class OglShaderProgramBase : DisposableIdentifiable {

	public uint ProgramID { get; private set; }
	public bool Separable { get; }

	public UseProgramStageMask Mask { get; private set; }

	private readonly Dictionary<ShaderType, OglShaderSource> _sources;

	public IReadOnlyDictionary<ShaderType, OglShaderSource> Sources => _sources;

	protected OglShaderProgramBase( bool separable = true ) {
		Separable = separable;
		_sources = new Dictionary<ShaderType, OglShaderSource>();
		ProgramID = Gl.CreateProgram();
		if (separable)
			Gl.ProgramParameter( ProgramID, ProgramParameterPName.ProgramSeparable, Gl.TRUE );
	}

	internal void CreateProgram( ShaderSourceService shaderSourceService ) {
		AttachShaders( shaderSourceService );
		Initialize();
		ValidateShader();
	}

	protected abstract void AttachShaders( ShaderSourceService shaderSourceService );
	protected virtual void Initialize() { }

	public void AttachShader( OglShaderSource shader ) {
		if (shader is null)
			throw new ArgumentNullException( nameof( shader ) );
		if (_sources.ContainsKey( shader.ShaderType )) {
			this.LogWarning( $"A shader of the type [{shader.ShaderType}] is already attached!" );
			return;
		}
		_sources.Add( shader.ShaderType, shader );
		Gl.AttachShader( ProgramID, shader.ShaderID );
		Mask |= GetMask( shader );
		this.LogLine( $"Attached [{shader.Filepath}]!", Log.Level.VERBOSE, color: ConsoleColor.DarkGreen );
	}

	public void DetachShader( OglShaderSource shader ) {
		if (!_sources.ContainsKey( shader.ShaderType )) {
			this.LogWarning( $"No shader of the type [{shader.ShaderType}] is attached!" );
			return;
		}
		if (_sources[ shader.ShaderType ] != shader) {
			this.LogWarning( $"Attempting to detach a shader of type [{shader.ShaderType}], but that shader is not attached!" );
			return;
		}
		_sources.Remove( shader.ShaderType );
		Gl.DetachShader( ProgramID, shader.ShaderID );
		Mask &= ~GetMask( shader );
		this.LogLine( $"Detached [{shader.ShaderID}]!", Log.Level.VERBOSE, color: ConsoleColor.Magenta );
	}

	public bool GetUniformBlockIndex( string blockName, out uint index ) {
		index = Gl.GetUniformBlockIndex( ProgramID, blockName );
		if (index == Gl.INVALID_INDEX)
			return false;
		return true;
	}

	/// <summary>
	/// Binds a uniform block to a binding in the shader program
	/// </summary>
	/// <param name="blockName">Name of the block.</param>
	/// <param name="binding">The binding this block will use.</param>
	/// <returns>True if the binding was successful, false if otherwise.</returns>
	public bool SetUniformBinding( uint blockIndex, uint binding ) {
		if (blockIndex == Gl.INVALID_INDEX)
			return false;
		Gl.UniformBlockBinding( ProgramID, blockIndex, binding );
		return true;
	}

	public bool GetShaderStorageBlockIndex( string blockName, out uint index ) {
		index = Gl.GetProgramResourceIndex( ProgramID, ProgramInterface.ShaderStorageBlock, blockName );
		if (index == Gl.INVALID_INDEX)
			return false;
		return true;
	}

	/// <summary>
	/// Binds a shader storage block to a binding in the shader program
	/// </summary>
	/// <param name="blockName">Name of the block.</param>
	/// <param name="binding">The binding this block will use.</param>
	/// <returns>True if the binding was successful, false if otherwise.</returns>
	public bool SetShaderStorageBinding( uint blockIndex, uint binding ) {
		if (blockIndex == Gl.INVALID_INDEX)
			return false;
		Gl.ShaderStorageBlockBinding( ProgramID, blockIndex, binding );
		return true;
	}


	/// <summary>
	/// Sets the location for attributes given when binding VAOs.
	/// </summary>
	/// <param name="location">Location, from 0 and up.</param>
	/// <param name="name">Name of the attribute, this is the name of an <b><i>in</i> variable</b> in the vertex shader.</param>
	protected void SetAttribLocation( uint location, string name ) => Gl.BindAttribLocation( ProgramID, location, name );

	/// <summary>
	/// Gets the attribute location of the input attribute.
	/// </summary>
	/// <param name="attribute">The attribute name.</param>
	/// <param name="loc">The attribute location.</param>
	/// <returns>Whether the attribute exists or not.</returns>
	public bool GetAttribLocation( string attribute, out uint loc ) {
		int location = Gl.GetAttribLocation( ProgramID, attribute );
		loc = (uint) location;
		if (location >= 0)
			return true;
		return false;
	}

	private static UseProgramStageMask GetMask( OglShaderSource shader ) => shader.ShaderType switch {
		ShaderType.VertexShader => UseProgramStageMask.VertexShaderBit,
		ShaderType.FragmentShader => UseProgramStageMask.FragmentShaderBit,
		ShaderType.GeometryShader => UseProgramStageMask.GeometryShaderBit,
		ShaderType.TessControlShader => UseProgramStageMask.TessControlShaderBit,
		ShaderType.TessEvaluationShader => UseProgramStageMask.TessEvaluationShaderBit,
		ShaderType.ComputeShader => UseProgramStageMask.ComputeShaderBit,
		_ => 0,
	};

	private void ValidateShader() {
		Gl.LinkProgram( ProgramID );

		Gl.GetProgram( ProgramID, ProgramProperty.LinkStatus, out int status );
		if (status == 0) {
			this.Breakpoint();
			StringBuilder ss = new( 1024 );
			Gl.GetProgramInfoLog( ProgramID, 1024, out int len, ss );
			this.LogWarning( $"Linking error {ss}" );
			//Dispose();
			return;
		}

		Gl.ValidateProgram( ProgramID );

		Gl.GetProgram( ProgramID, ProgramProperty.ValidateStatus, out status );
		if (status == 0) {
			this.Breakpoint();
			StringBuilder ss = new( 1024 );
			Gl.GetProgramInfoLog( ProgramID, 1024, out int len, ss );
			this.LogWarning( $"Validation error {ss}" );
			//Dispose();
			return;
		}

		this.LogLine( $"Shader validated!", Log.Level.HIGH, color: ConsoleColor.Green );
	}

	protected override bool InternalDispose() {
		foreach (KeyValuePair<ShaderType, OglShaderSource> kvp in _sources)
			DetachShader( kvp.Value );
		Gl.DeleteShader( ProgramID );
		return true;
	}
}
