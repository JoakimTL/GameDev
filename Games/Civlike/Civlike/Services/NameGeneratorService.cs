using Civlike.Services.Names;

namespace Civlike.Services;

public sealed class NameGeneratorService : INameGeneratorService {
	public const string BaseDirectory = "res/names";
	private readonly Dictionary<string, NameGenerator> _existingGenerators;

	public NameGeneratorService() {
		this._existingGenerators = [];
	}

	public IReadOnlyList<string> GetNameCollections() 
		=> [ .. Directory.GetDirectories( BaseDirectory ).Where( p => Directory.GetFiles( p, "*.txt" ).Length > 0 ) ];

	public NameGenerator GetNameGenerator( string collectionName ) {
		if (this._existingGenerators.TryGetValue( collectionName, out NameGenerator? generator ) )
			return generator;
		string directoryPath = Path.Combine( BaseDirectory, collectionName );
		if (!Directory.Exists( directoryPath ))
			throw new InvalidOperationException();
		generator = new NameGenerator( directoryPath );
		this._existingGenerators[ collectionName ] = generator;
		return generator;
	}
}