using Engine.Data.Datatypes;

namespace Engine.Rendering.Standard.UI.Standard.Text;

public class Font : Identifiable {

	public readonly string FontTexturePath;
	public readonly string FontInformationPath;

	private readonly Dictionary<char, FontCharacter> _characters;
	public IReadOnlyDictionary<char, FontCharacter> Characters => this._characters;

	private readonly Dictionary<char, Dictionary<char, int>> _kerningValues;

	public FontMetadata Metadata { get; }

	public readonly int LineHeight;
	public readonly int Baseline;
	public readonly Vector2i Resolution;
	public readonly Vector2i PaddingUpDown;
	public readonly Vector2i PaddingLeftRight;

	public Font( string fontPathWithoutExtension ) : base( fontPathWithoutExtension ) {
		this.FontTexturePath = $"{fontPathWithoutExtension}.png";
		this.FontInformationPath = $"{fontPathWithoutExtension}.fnt";

		this._characters = new();
		this._kerningValues = new();
		ReadFontInformation( out string infoLine, out string metadataLine, out IReadOnlyList<string> characterData, out IReadOnlyList<string> kerningData );
		LoadCharacters( characterData );
		LoadKerning( kerningData );
		LoadInfo( infoLine, out this.PaddingUpDown, out this.PaddingLeftRight );
		LoadMetadata( metadataLine, out this.Resolution, out this.LineHeight, out this.Baseline );
		this.Metadata = new( this );
	}

	public int GetKerning( char first, char second ) => this._kerningValues.TryGetValue( first, out var dict ) ? dict.TryGetValue( second, out var value ) ? value : 0 : 0;

	private void ReadFontInformation( out string infoLine, out string metadataLine, out IReadOnlyList<string> characterData, out IReadOnlyList<string> kerningData ) {
		if ( !File.Exists( this.FontInformationPath ) )
			throw new FileNotFoundException( this.FontInformationPath );
		List<string> characters = new();
		List<string> kerning = new();
		string[] lines = File.ReadAllLines( this.FontInformationPath );
		infoLine = "";
		metadataLine = "";

		for ( int i = 0; i < lines.Length; i++ ) {
			if ( lines[ i ].StartsWith( "info " ) )
				infoLine = lines[ i ];
			if ( lines[ i ].StartsWith( "common " ) )
				metadataLine = lines[ i ];
			if ( lines[ i ].StartsWith( "char " ) )
				characters.Add( lines[ i ] );
			if ( lines[ i ].StartsWith( "kerning " ) )
				kerning.Add( lines[ i ] );
		}

		characterData = characters;
		kerningData = kerning;
	}

	private void LoadInfo( string line, out Vector2i paddingUpDown, out Vector2i paddingLeftRight ) {
		IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
		Dictionary<string, string> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => p[ 1 ] );
		int[] paddings = data[ "padding" ].Split( ',' ).Select( int.Parse ).ToArray();
		if ( paddings.Length != 4 )
			throw new InvalidOperationException( $"{this.Name} missing padding data!" );
		paddingUpDown = new Vector2i( paddings[ 0 ], paddings[ 2 ] ); //TODO check if correct indexes, run hiero with different paddings and check
		paddingLeftRight = new Vector2i( paddings[ 1 ], paddings[ 3 ] );
	}

	private void LoadMetadata( string line, out Vector2i resolution, out int lineheight, out int baseline ) {
		IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
		Dictionary<string, int> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => int.Parse( p[ 1 ] ) );
		resolution = new Vector2i( data[ "scaleW" ], data[ "scaleH" ] );
		lineheight = data[ "lineHeight" ];
		baseline = data[ "base" ];
	}

	private void LoadCharacters( IReadOnlyList<string> characterData ) {
		for ( int i = 0; i < characterData.Count; i++ ) {
			string line = characterData[ i ];
			IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
			Dictionary<string, int> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => int.Parse( p[ 1 ] ) );
			FontCharacter c = new( (char) data[ "id" ], data[ "x" ], data[ "y" ], data[ "width" ], data[ "height" ], data[ "xoffset" ], data[ "yoffset" ], data[ "xadvance" ] );
			this._characters.Add( c.Character, c );
		}
	}

	private void LoadKerning( IReadOnlyList<string> kerningData ) {
		for ( int i = 0; i < kerningData.Count; i++ ) {
			string line = kerningData[ i ];
			IEnumerable<string> splitLine = line.Split( ' ' ).Select( p => p.Trim() ).Where( p => p.Length > 0 && p.Contains( '=' ) );
			Dictionary<string, int> data = splitLine.Select( p => p.Split( '=' ) ).ToDictionary( p => p[ 0 ], p => int.Parse( p[ 1 ] ) );
			char first = (char) data[ "first" ];
			if ( !this._kerningValues.TryGetValue( first, out Dictionary<char, int>? dict ) )
				this._kerningValues.Add( first, dict = new() );
			dict[ (char) data[ "second" ] ] = data[ "amount" ];
		}
	}
}
