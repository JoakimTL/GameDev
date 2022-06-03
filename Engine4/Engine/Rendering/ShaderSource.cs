using System.Text;
using Engine.Data;
using OpenGL;

namespace Engine.Rendering;
public class ShaderSource : DisposableIdentifiable, ISerializableComponent {

	public uint ShaderID { get; private set; }
	public ShaderType ShaderType { get; private set; }

	protected override string UniqueNameTag => this.ShaderType.ToString();

	public ShaderSource( string name, string source, ShaderType shaderType ) : base( name ) {
		this.ShaderID = Gl.CreateShader( shaderType );
		this.ShaderType = shaderType;
		Gl.ShaderSource( this.ShaderID, new string[] { source }, new int[] { source.Length } );
		Gl.CompileShader( this.ShaderID );

		Gl.GetShader( this.ShaderID, ShaderParameterName.CompileStatus, out int status );
		if ( status == 0 ) {
			StringBuilder ss = new( 1024 );
			Gl.GetShaderInfoLog( this.ShaderID, ss.Capacity, out int logLength, ss );
			this.LogWarning( $"{logLength}-{ss}" );
			Dispose();
		}
	}

	protected override bool OnDispose() {
		Gl.DeleteShader( this.ShaderID );
		return true;
	}

	public static bool ReadSource( string path, out string source ) {
		source = "";
		try {
			if ( !File.Exists( path ) ) {
				Log.Warning( $"Couldn't find file {path}!" );
				return false;
			}

			string? dir = Path.GetDirectoryName( path );

			StringBuilder sb = new();
			StreamReader reader = new( File.OpenRead( path ) );

			while ( !reader.EndOfStream ) {
				string? line = reader.ReadLine();

				if ( line?.StartsWith( "#include " ) ?? false ) {
					string pathInclude = dir + "/" + line[ "#include ".Length.. ];
					ReadSource( pathInclude, out line );
				}

				sb.AppendLine( line );
			}

			source = sb.ToString();
			return true;
		} catch ( Exception e ) {
			Log.Error( e );
		}
		source = "";
		return false;
	}

	public byte[]? Serialize() => DataUtils.ToBytes( this.Name );

	public void SetFromSerializedData( byte[] data ) => this.LogLine( $"{nameof(ShaderSource)} does not utilize {nameof(SetFromSerializedData)}!", Log.Level.LOW );
}
