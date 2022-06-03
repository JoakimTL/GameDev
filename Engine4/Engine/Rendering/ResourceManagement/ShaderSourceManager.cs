using OpenGL;

namespace Engine.Rendering.ResourceManagement;

public class ShaderSourceManager : DisposableIdentifiable {

	public string BaseDirectory { get; }

	private readonly Dictionary<string, ShaderSource> _sourceDictionary;
	private static readonly IReadOnlyDictionary<string, ShaderType> _allowedExtensions = new Dictionary<string, ShaderType>() {
			{ ".vert", ShaderType.VertexShader },
			{ ".frag", ShaderType.FragmentShader },
			{ ".geom", ShaderType.GeometryShader },
			{ ".tesc", ShaderType.TessControlShader },
			{ ".tese", ShaderType.TessEvaluationShader },
			{ ".comp", ShaderType.ComputeShader }
		};

	/// <summary>
	/// Searches the entire folder structure for shader source files. Shader sources are files ending in .vert, .frag, .geom, .tesc, .tese, and .comp.
	/// </summary>
	/// <param name="directory">The base directory to begin the search</param>
	internal ShaderSourceManager( string directory ) {
		this.BaseDirectory = directory;
		this._sourceDictionary = new Dictionary<string, ShaderSource>();
		Initialize();
	}

	internal void Initialize() {
		//Begin search through the directory!
		List<string> files = new();
		SearchDirectory( files, this.BaseDirectory );

		this.LogLine( $"Found {files.Count} files!", Log.Level.LOW, ConsoleColor.Blue );

		for ( int i = 0; i < files.Count; i++ ) {
			string filePath = files[ i ];
			string fileName = filePath[ ( this.BaseDirectory.Length + 1 ).. ].Replace( '\\', '/' );

			string ext = Path.GetExtension( filePath );
			if ( _allowedExtensions.TryGetValue( ext, out ShaderType shaderType ) )
				if ( ShaderSource.ReadSource( filePath, out string source ) ) {
					this._sourceDictionary.Add( fileName, new ShaderSource( fileName, source, shaderType ) );
				}
		}

		this.LogLine( $"Found {this._sourceDictionary.Count} valid shader source files!", Log.Level.NORMAL, ConsoleColor.Green );
	}

	private void SearchDirectory( List<string> files, string path ) {
		if ( !Directory.Exists( path ) ) {
			this.LogWarning( "Attempted to search for shader sources in a directory that doesn't exist!" );
			return;
		}
		//Find sub directories
		string[] subDirectories = Directory.GetDirectories( path );
		//Loop through subdirectories.
		for ( int i = 0; i < subDirectories.Length; i++ )
			SearchDirectory( files, subDirectories[ i ] );
		//Add files from this directory.
		files.AddRange( Directory.GetFiles( path ) );
	}

	protected override bool OnDispose() {
		foreach ( ShaderSource source in this._sourceDictionary.Values )
			source.Dispose();
		return true;
	}

	/// <summary>
	/// Shader names are their path from the base directory.
	/// </summary>
	/// <param name="name">Use forward slashes '/', not backward '\' slashes for directories.</param>
	/// <returns><see cref="ShaderSource"/> instance representing the source code.</returns>
	public ShaderSource this[ string name ] {
		get {
			if ( this._sourceDictionary.TryGetValue( name, out ShaderSource? src ) )
				return src;
			this.LogWarning( $"Attempting to use shader source {name} not found in the directory!" );
			throw new ArgumentException( $"Couldn't find shader file {name}.", nameof( name ) );
		}
	}
}
