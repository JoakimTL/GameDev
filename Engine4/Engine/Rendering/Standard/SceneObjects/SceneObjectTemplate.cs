namespace Engine.Rendering.Standard.SceneObjects;
public class SceneObjectTemplate {

	/*
	 * Name
	 * Mesh filename
	 * Shader guid
	 * Bone filename (later)
	 * Used textures (Id for correct placement)
	 */

	public static SceneObjectTemplate Cube { get; } = new( null, "cube", "cube", new Guid( "e0d49716-c34e-4d1e-97a2-20382ab6060a" ), new() );
	public static SceneObjectTemplate Square { get; } = new( null, "square", "square", new Guid( "6d42491e-0f65-46e5-b340-2ebd0179d9cc" ), new() );

	private SceneObjectTemplate( string? path, string name, string meshFilename, Guid shaderGuid, List<string> textures ) {
		this.Path = path;
		this.Name = name;
		this.MeshFilename = meshFilename;
		this.ShaderGuid = shaderGuid;
		this.TexturePaths = textures;
	}

	public string? Path { get; }
	public string Name { get; }
	public string MeshFilename { get; }
	public Guid ShaderGuid { get; }
	public IReadOnlyList<string> TexturePaths { get; }

	public static SceneObjectTemplate? Read( string path ) {
		if ( !File.Exists( path ) ) {
			Log.Warning( $"Unable to read {path}!" );
			return null;
		}
		string fileData = File.ReadAllText( path );
		if ( !fileData.StartsWith( "#SO_TEMPLATE" ) ) {
			Log.Warning( $"Unable to read {path}!" );
			return null;
		}
		var data = fileData.Split( '\n' ).Select( p => p.Split( ' ', 2 ) ).Where( p => p.Length > 1 ).ToDictionary( p => p[ 0 ].ToLowerInvariant(), p => p[ 1 ].Trim() );
		string? name = null;
		string? meshFilename = null;
		Guid? shaderGuid = null;
		List<string> textures = new();
		foreach ( KeyValuePair<string, string> item in data ) {
			if ( item.Key.StartsWith( "name" ) )
				name = item.Value;
			if ( item.Key.StartsWith( "mesh" ) )
				meshFilename = item.Value;
			if ( item.Key.StartsWith( "shader" ) )
				shaderGuid = new Guid( item.Value );
			if ( item.Key.StartsWith( "tex" ) )
				textures.Add( item.Value );
		}
		if ( name is null || meshFilename is null || !shaderGuid.HasValue )
			throw new InvalidOperationException( $"Model data incomplete for {path}!" );
		return new( path, name, meshFilename, shaderGuid.Value, textures );
	}
}
