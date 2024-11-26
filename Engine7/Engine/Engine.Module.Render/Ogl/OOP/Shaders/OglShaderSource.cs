using Engine.Logging;
using OpenGL;
using System.Text;

namespace Engine.Module.Render.Ogl.OOP.Shaders;

public sealed class OglShaderSource : DisposableIdentifiable {
	public uint ShaderID { get; private set; }
	public ShaderType ShaderType { get; private set; }
	public string Filepath { get; }

	public event Action? FileChanged;

	public OglShaderSource( string path, ShaderType shaderType ) {
		Filepath = path;

		ShaderID = Gl.CreateShader( shaderType );
		ShaderType = shaderType;
		string source = GetData();
		Gl.ShaderSource( ShaderID, [ source ], [ source.Length ] );
		Gl.CompileShader( ShaderID );

		Gl.GetShader( ShaderID, ShaderParameterName.CompileStatus, out int status );
		if (status == 0) {
			this.Breakpoint();
			StringBuilder sb = new( 1024 );
			Gl.GetShaderInfoLog( ShaderID, sb.Capacity, out int logLength, sb );
			this.LogWarning( $"{logLength}-{sb}" );
			//Dispose();
		}

		Nickname = $"SHA{ShaderID} {Filepath}";
	}

	public string GetData() => ReadSource( Filepath, out string source ) ? source : string.Empty;

	private static bool ReadSource( string path, out string source ) {
		source = "";
		try {
			if (!File.Exists( path )) {
				Log.Warning( $"Couldn't find file {path}!" );
				return false;
			}

			string? dir = Path.GetDirectoryName( path );

			StringBuilder sb = new();
			StreamReader reader = new( File.OpenRead( path ) );

			while (!reader.EndOfStream) {
				string? line = reader.ReadLine();

				if (line?.StartsWith( "#include " ) ?? false) {
					string pathInclude = dir + "/" + line[ "#include ".Length.. ];
					ReadSource( pathInclude, out line );
				}

				sb.AppendLine( line );
			}

			source = sb.ToString();
			return true;
		} catch (Exception e) {
			e.Log();
		}
		source = "";
		return false;
	}

	protected override bool InternalDispose() {
		Gl.DeleteShader( ShaderID );
		return true;
	}
}
