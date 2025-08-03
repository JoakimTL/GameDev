using Civlike.Services.Names;

namespace Civlike.Services;

public sealed class TagSyllableService {

	private readonly Dictionary<string, TagSyllables> _tagSyllables;

	public TagSyllableService() {
		this._tagSyllables = [];

	}

}

public sealed class TagHybridNameGenerator {

	private readonly TagSyllableService _tagSyllableService;

	public TagHybridNameGenerator(TagSyllableService tagSyllableService) {
		this._tagSyllableService = tagSyllableService;
	}

}