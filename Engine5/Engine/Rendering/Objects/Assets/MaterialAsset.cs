namespace Engine.Rendering.Objects.Assets;

public sealed class MaterialAsset : Identifiable {

	public string Path { get; }
	private List<ReferenceContainer<TextureAsset>> _textures;
	private List<TextureAsset> _loadedTextures;
	public ShaderAsset? Shader { get; private set; }
	public IReadOnlyList<TextureAsset> Textures => _loadedTextures;

	public string ShaderIdentity { get; }
	public string[] TexturePaths { get; }

	protected override string UniqueNameTag => Path;

	internal MaterialAsset( string path ) {
		Path = path;
		_textures = new();
		_loadedTextures = new();
		LoadFile(out string shaderIdentity, out string[] texturePaths );
		ShaderIdentity = shaderIdentity;
		TexturePaths = texturePaths;
	}

	private void LoadFile(out string shaderIdentity, out string[] texturePaths) {
		shaderIdentity = string.Empty;
		texturePaths = Array.Empty<string>();
		if ( !File.Exists( Path ) ) {
			this.LogWarning( $"Could not find file {Path}." );
			return;
		}
		List<string> texturePathList = new();
		string[] fileData = File.ReadAllLines( Path );
		for ( int i = 0; i < fileData.Length; i++ ) {
			string line = fileData[ i ];
			string[] split = line.Split( '=' ).Select( p => p.Trim() ).ToArray();
			if ( split.Length != 2 ) {
				this.LogWarning( $"{Path}: Error on line {i + 1}." );
				continue;
			}
			if ( split[0].StartsWith( "shader" ) ) {
				shaderIdentity = split[ 1 ];
			} else if ( split[ 0 ].StartsWith( "texture" ) ) {
				texturePathList.Add(split[ 1 ]);
			}
		}
		texturePaths = texturePathList.ToArray();
	}

	internal void Set( ShaderAsset shaderAsset, List<ReferenceContainer<TextureAsset>> textureAssets ) {
		Shader = shaderAsset;
		_textures.AddRange( textureAssets );
		_loadedTextures.AddRange( textureAssets.Select( p => p.Value ) );
	}

	/*
	protected override void OnContextLoad( Context context ) {
		//File load
		//Parse usecase and shader pipeline name
		//Load shaderbundle
		string data = File.ReadAllText( Path );
		var bundleShaderData = data.Split( '\n' );
		Dictionary<string, Texture> textures = new();
		for ( int i = 0; i < bundleShaderData.Length; i++ ) {
			string line = bundleShaderData[ i ];
			if ( line.StartsWith( "shader" ) )
				ShaderBundle = context.Service<ShaderBundleService>().Get( line[ "shader:".Length.. ].Trim() );
			if ( line.StartsWith( "tex" ) ) {
				var lineSplit = line[ "tex:".Length.. ].Split( ':' );
				if ( lineSplit.Length != 2 )
					continue;
				textures.Add( lineSplit[ 0 ].Trim(), context.Service<TextureService>().Get( lineSplit[ 1 ].Trim() ) );
			}
		}
		Textures = textures;
	}

	public override void Dispose() { }

	public static MaterialAsset? CreateAsset( string relativePath ) {
		string path = $"{BaseDirectory}/{relativePath}";
		if ( !File.Exists( path ) )
			return null;
		return new MaterialAsset( path );
	}
	*/
}