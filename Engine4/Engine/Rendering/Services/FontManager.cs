using Engine.Rendering.Standard.UI.Standard.Text;

namespace Engine.Rendering.Services;
public class FontManager : ModuleService {

	private readonly Dictionary<string, Font> _fonts;

	public FontManager() {
		this._fonts = new();
		LoadFonts();
	}

	public Font Get( string name ) => this._fonts[ name ];
	public Font? GetOrDefault( string name ) => this._fonts.TryGetValue( name, out Font? font ) ? font : null;
	protected override bool OnDispose() => true;

	private void LoadFonts() {
		List<string> fontFilePaths = new();
		SearchDirectory( fontFilePaths, "res/textures/fonts", "fnt" );
		foreach ( string fontFile in fontFilePaths )
			this._fonts.Add( Path.GetFileNameWithoutExtension( fontFile ), new Font( Path.GetFileNameWithoutExtension( fontFile ) ) );
	}

	private void SearchDirectory( List<string> files, string path, string extension ) {
		if ( !Directory.Exists( path ) ) {
			this.LogWarning( "Attempted to search for files in a directory that doesn't exist!" );
			return;
		}
		//Find sub directories
		string[] subDirectories = Directory.GetDirectories( path );
		//Loop through subdirectories.
		for ( int i = 0; i < subDirectories.Length; i++ )
			SearchDirectory( files, subDirectories[ i ], extension );
		//Add files from this directory.
		files.AddRange( Directory.GetFiles( path, $"*.{extension}" ) );
	}
}
