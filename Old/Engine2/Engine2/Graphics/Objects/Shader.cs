using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LMath;
using Engine.MemLib;
using Engine.Utilities.IO;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.Graphics.Objects {
	public abstract class Shader : Cacheable, ICountable {

		public uint ID { get; private set; }
		private uint vShader, gShader, fShader;
		private readonly Dictionary<string, int> uniforms;

		public bool HasVertexShader { get; private set; }
		public string VertexSourceFile { get; private set; }
		public bool HasGeometryShader { get; private set; }
		public string GeometrySourceFile { get; private set; }
		public bool HasFragmentShader { get; private set; }
		public string FragmentSourceFile { get; private set; }

		private readonly Dictionary<string, FileSystemWatcher> watchedPaths;
		private readonly FileSystemWatcher vertexFileWatcher;
		private readonly FileSystemWatcher fragmentFileWatcher;
		private readonly FileSystemWatcher geometryFileWatcher;

#if DEBUG
		private bool bound;
#endif
		private volatile bool rebuild;

		public Shader( string name, string sourceV = "", string sourceF = "", string sourceG = "" ) : base( name ) {
			watchedPaths = new Dictionary<string, FileSystemWatcher>();

			if( !string.IsNullOrEmpty( sourceV ) ) {
				HasVertexShader = true;
				VertexSourceFile = "res/shaders/" + sourceV + ".vert";
				if( File.Exists( VertexSourceFile ) ) {
					string dir = Path.GetDirectoryName( VertexSourceFile );
					string filename = Path.GetFileName( VertexSourceFile );
					Mem.Logs.Routine.WriteLine( $"[{Name}]: Vertex source [{VertexSourceFile}]" );

					vertexFileWatcher = new FileSystemWatcher {
						Path = dir,
						Filter = filename,
						EnableRaisingEvents = true
					};
					vertexFileWatcher.Changed += FileChangeEvent;
				} else {
					Mem.Logs.Warning.WriteLine( $"[{Name}]: Source for vertex shader was set, but no file was found!" );
				}
			}

			if( !string.IsNullOrEmpty( sourceF ) ) {
				HasFragmentShader = true;
				FragmentSourceFile = "res/shaders/" + sourceF + ".frag";
				if( File.Exists( FragmentSourceFile ) ) {
					string dir = Path.GetDirectoryName( FragmentSourceFile );
					string filename = Path.GetFileName( FragmentSourceFile );
					Mem.Logs.Routine.WriteLine( $"[{Name}]: Fragment source [{FragmentSourceFile}]" );

					fragmentFileWatcher = new FileSystemWatcher {
						Path = dir,
						Filter = filename,
						EnableRaisingEvents = true
					};
					fragmentFileWatcher.Changed += FileChangeEvent;
				} else {
					Mem.Logs.Warning.WriteLine( $"[{Name}]: Source for fragment shader was set, but no file was found!" );
				}
			}

			if( !string.IsNullOrEmpty( sourceG ) ) {
				HasGeometryShader = true;
				GeometrySourceFile = "res/shaders/" + sourceG + ".geom";
				if( File.Exists( GeometrySourceFile ) ) {
					string dir = Path.GetDirectoryName( GeometrySourceFile );
					string filename = Path.GetFileName( GeometrySourceFile );
					Mem.Logs.Routine.WriteLine( $"[{Name}]: Geometry source [{GeometrySourceFile}]" );

					geometryFileWatcher = new FileSystemWatcher {
						Path = dir,
						Filter = filename,
						EnableRaisingEvents = true
					};
					geometryFileWatcher.Changed += FileChangeEvent;
				} else {
					Mem.Logs.Warning.WriteLine( $"[{Name}]: Source for geometry shader was set, but no file was found!" );
				}
			}

			uniforms = new Dictionary<string, int>();

			CreateShader();
		}

		private void FileChangeEvent( object sender, FileSystemEventArgs e ) {
			rebuild = true;
		}

		private void CreateShader() {
			rebuild = false;
			uniforms.Clear();
			ID = Gl.CreateProgram();
			if( ID == 0 )
				throw new Exception( "Shader was not initiated: \"ID set to 0\"" );

			CompileVertexShader();
			if( ID == 0 )
				return;
			CompileGeometryShader();
			if( ID == 0 )
				return;
			CompileFragmentShader();
			if( ID == 0 )
				return;

			InitAttribs();
			PreCompile();
			ValidateShader();
			if( ID == 0 )
				return;
			Bind();
			if( ID == 0 )
				return;
			PostCompile();
		}

		protected abstract void InitAttribs();
		protected abstract void PreCompile();
		protected abstract void PostCompile();

		protected void SetAttribLocation( uint location, string name ) {
			Gl.BindAttribLocation( ID, location, name );
		}

		private bool ReadSource( string path, out string source ) {
			source = "";
			try {
				if( !File.Exists( path ) ) {
					Mem.Logs.Warning.WriteLine( $"[{Name}] Couldn't find file {path}!" );
					return false;
				}

				string dir = Path.GetDirectoryName( path );

				//Watch this file if we're not already doing so.
				if( !watchedPaths.ContainsKey( path ) ) {
					string filename = Path.GetFileName( path );
					FileSystemWatcher fsw = new FileSystemWatcher() {
						Path = dir,
						Filter = filename,
						EnableRaisingEvents = true
					};
					watchedPaths.Add( path, fsw );
					fsw.Changed += FileChangeEvent;
				}
				StringBuilder sb = new StringBuilder();

				StreamReader reader = new StreamReader( File.OpenRead( path ) );

				while( !reader.EndOfStream ) {
					string line = reader.ReadLine();

					if( line.StartsWith( "#include " ) ) {
						string pathInclude = dir + "/" + line.Substring( "#include ".Length );
						ReadSource( pathInclude, out line );
					}

					sb.AppendLine( line );
				}

				source = sb.ToString();
				return true;
			} catch( Exception e ) {
				Mem.Logs.Error.WriteLine( e.ToString(), (LogFlags) 0 );
			}
			source = "";
			return false;
		}

		private void CompileVertexShader() {
			if( vShader != 0 )
				DisposeVertexShader();

			if( !HasVertexShader )
				return;

			if( ReadSource( VertexSourceFile, out string source ) ) {
				vShader = AddProgram( source, ShaderType.VertexShader );
				Mem.Logs.Routine.WriteLine( $"[{Name}]: Added [{ShaderType.VertexShader}]!", ConsoleColor.Cyan );
			}
		}

		private void CompileGeometryShader() {
			if( gShader != 0 )
				DisposeGeometryShader();

			if( !HasGeometryShader )
				return;

			if( ReadSource( GeometrySourceFile, out string source ) ) {
				gShader = AddProgram( source, ShaderType.GeometryShader );
				Mem.Logs.Routine.WriteLine( $"[{Name}]: Added [{ShaderType.GeometryShader}]!", ConsoleColor.Cyan );
			}
		}

		private void CompileFragmentShader() {
			if( fShader != 0 )
				DisposeFragmentShader();

			if( !HasFragmentShader )
				return;

			if( ReadSource( FragmentSourceFile, out string source ) ) {
				fShader = AddProgram( source, ShaderType.FragmentShader );
				Mem.Logs.Routine.WriteLine( $"[{Name}]: Added [{ShaderType.FragmentShader}]!", ConsoleColor.Cyan );
			}
		}

		private void DisposeFragmentShader() {
			Gl.DetachShader( ID, fShader );
			Gl.DeleteShader( fShader );
			fShader = 0;
		}

		private void DisposeGeometryShader() {
			Gl.DetachShader( ID, gShader );
			Gl.DeleteShader( gShader );
			gShader = 0;
		}

		private void DisposeVertexShader() {
			Gl.DetachShader( ID, vShader );
			Gl.DeleteShader( vShader );
			vShader = 0;
		}

		private uint AddProgram( string source, ShaderType type ) {
			uint shaderID = Gl.CreateShader( type );

			if( shaderID == 0 )
				throw new Exception( "Couldn't find valid memory location for shader." );

			Gl.ShaderSource( shaderID, new string[] { source }, new int[] { source.Length } );
			Gl.CompileShader( shaderID );

			Gl.GetShader( shaderID, ShaderParameterName.CompileStatus, out int status );
			if( status == 0 ) {
				StringBuilder ss = new StringBuilder( 1024 );
				Gl.GetShaderInfoLog( shaderID, ss.Capacity, out int logLength, ss );
				Mem.Logs.Warning.WriteLine( $"[{Name}]: [{type}][{logLength}] error {ss}" );
				Dispose();
				return 0;
			}

			Gl.AttachShader( ID, shaderID );
			Mem.Logs.Routine.WriteLine( $"[{Name}]: Attached [{type}]!", ConsoleColor.Cyan );

			return shaderID;
		}

		private void ValidateShader() {
			Gl.LinkProgram( ID );

			Gl.GetProgram( ID, ProgramProperty.LinkStatus, out int status );
			if( status == 0 ) {
				StringBuilder ss = new StringBuilder( 1024 );
				Gl.GetShaderInfoLog( ID, 1024, out _, ss );
				Mem.Logs.Warning.WriteLine( $"[{Name}]: Shader error {ss}" );
				Dispose();
				return;
			}

			Gl.ValidateProgram( ID );

			Gl.GetProgram( ID, ProgramProperty.ValidateStatus, out status );
			if( status == 0 ) {
				StringBuilder ss = new StringBuilder( 1024 );
				Gl.GetShaderInfoLog( ID, 1024, out int logLength, ss );
				Mem.Logs.Warning.WriteLine( $"[{Name}]: [{logLength}] Shader error {ss}" );
				Dispose();
				return;
			}

			Mem.Logs.Routine.WriteLine( $"[{Name}]: Shader validated!", ConsoleColor.Green );
		}

		public void Bind() {
			if( rebuild )
				CreateShader();
#if DEBUG
			bound = true;
#endif
			Gl.UseProgram( ID );
		}

		public void Unbind() {
#if DEBUG
			bound = false;
#endif
			Gl.UseProgram( 0 );
		}

		public override void Dispose() {
			Unbind();
			DisposeVertexShader();
			DisposeGeometryShader();
			DisposeFragmentShader();
			Gl.DeleteProgram( ID );
			Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed!" );
			ID = 0;
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is Shader o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( Shader obj ) {
			return obj.ID == ID;
		}

		public static bool operator ==( Shader a, Shader b ) {
			if( a is null || b is null )
				return false;
			return a.Equals( b );
		}

		public static bool operator !=( Shader a, Shader b ) {
			return !( a == b );
		}

		#region Uniforms
		public int GetUniform( string uniform ) {
			if( uniforms.TryGetValue( uniform, out int location ) )
				return location;
			return GenerateUniform( uniform );
		}

		private int GenerateUniform( string uniform ) {
#if DEBUG
			if( !bound )
				Mem.Logs.Warning.WriteLine( $"[{Name}]: Not bound while trying to generate uniform {uniform}!" );
#endif
			int uniformLocation = Gl.GetUniformLocation( ID, uniform );
			Mem.Logs.Routine.WriteLine( $"[{Name}]: Added uniform [{uniform}] at location [{uniformLocation}]", ConsoleColor.DarkCyan );
			uniforms.Add( uniform, uniformLocation );
			return uniformLocation;
		}

		#region Strings
		public void Set( string uniform, bool value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, int value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, float value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, Vector2 value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, Vector3 value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, Vector4 value ) {
			Set( GetUniform( uniform ), value );
		}

		public void Set( string uniform, Matrix4 value ) {
			Set( GetUniform( uniform ), value );
		}
		#endregion
		#region Indexed
		public void Set( int uniform, bool value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform1( uniform, value ? 1F : 0F );
		}

		public void Set( int uniform, int value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform1( uniform, value );
		}

		public void Set( int uniform, float value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform1( uniform, value );
		}

		public void Set( int uniform, Vector2 value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform2( uniform, value );
		}

		public void Set( int uniform, Vector3 value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform3( uniform, value );
		}

		public void Set( int uniform, Vector4 value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.Uniform4( uniform, value );
		}

		public void Set( int uniform, Matrix4 value ) {
#if DEBUG
			if( !bound )
				Logging.Warning( $"[{Name}]: Not bound while trying to set uniform {uniform} to {value}!" );
#endif
			if( uniform >= 0 )
				Gl.UniformMatrix4( uniform, false, value );
		}
		#endregion
		#endregion

	}
}
