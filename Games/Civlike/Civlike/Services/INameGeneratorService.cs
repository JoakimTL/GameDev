using Civlike.Services.Names;

namespace Civlike.Services;

public interface INameGeneratorService {
	public IReadOnlyList<string> GetNameCollections();
	public NameGenerator GetNameGenerator( string collectionName );
}
