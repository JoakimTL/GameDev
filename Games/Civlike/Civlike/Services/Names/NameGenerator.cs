using Engine.Logging;

namespace Civlike.Services.Names;
public sealed class NameGenerator {

	private readonly Dictionary<string, SyllableMarkovGenerator> _generatorsByFileName;

	public NameGenerator( string basePath ) {
		if (!Directory.Exists( basePath ))
			throw new ArgumentException( $"Directory {basePath} must exist!" );
		string[] fileNames = Directory.GetFiles( basePath, "*.txt" );
		this._generatorsByFileName = [];
		foreach (string file in fileNames) {
			string[] fileData = File.ReadAllLines( file );
			if (fileData.Length == 0) {
				this.LogWarning( $"File {file} missing data!" );
				continue;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( file );
			string namesLine = fileData[ 0 ];
			IEnumerable<string> names = namesLine.Split( ',' ).Select( p => p.Trim() );
			IEnumerable<string> blackListedEndings = Enumerable.Empty<string>();
			if (fileData.Length > 1)
				blackListedEndings = fileData[ 1 ].Split( ',' ).Select(p => p.Trim());
			this._generatorsByFileName[ fileNameWithoutExtension ] = new( names, blackListedEndings );
		}
	}

	public string? GenerateName( string nameType, int maxSyllables = 5, int minLength = 4 ) 
		=> !this._generatorsByFileName.TryGetValue( nameType, out SyllableMarkovGenerator? generator ) ? null : generator.NextName( maxSyllables, minLength );

}
