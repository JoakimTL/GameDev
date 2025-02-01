namespace Sandbox.Logic.Research;

public static class TechnologyList {
	private static readonly Dictionary<Type, TechnologyBase> _technologies;

	static TechnologyList() {
		_technologies = [];

		foreach (Type technologyFieldType in TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( TechnologyBase ) ) )) {
			TechnologyBase instance = technologyFieldType.CreateInstance( null ) as TechnologyBase ?? throw new InvalidOperationException( $"Failed to create instance of {technologyFieldType}." );
			_technologies[ technologyFieldType ] = instance;
		}
	}

	public static IReadOnlyCollection<TechnologyBase> GetTechnologies() => _technologies.Values;

	public static T GetTech<T>() where T : TechnologyBase
		=> _technologies[ typeof( T ) ] as T ?? throw new InvalidOperationException( $"Failed to get instance of {typeof( T )}." );
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?

public sealed class NationalTechnology {


}

public sealed class TileTechnologyHolder : IUpdateable {
	private readonly Dictionary<Type, TechResearch> _research;
	private readonly List<TechResearch> _activelyResearching;
	private readonly List<TechResearch> _remainingTechnologies;
	private readonly List<TechResearch> _researchedTechnologies;
	public TileTechnologyHolder() {
		_research = [];
		_activelyResearching = [];
		_remainingTechnologies = [];
		_researchedTechnologies = [];
		foreach (TechnologyBase technology in TechnologyList.GetTechnologies())
			_research[ technology.GetType() ] = new TechResearch( this, technology );
		foreach (TechResearch research in _research.Values) {
			research.Initialize();
			if (research.ActivelyResearching) {
				_activelyResearching.Add( research );
				continue;
			}
			if (research.ResearchCompleted) {
				_researchedTechnologies.Add( research );
				continue;
			}
			_remainingTechnologies.Add( research );
		}
	}

	public void Update( double time, double deltaTime ) {
		bool researchCompleted = false;

		for (int i = _activelyResearching.Count - 1; i >= 0; i--) {
			TechResearch research = _activelyResearching[ i ];
			if (research.UpdateProgress()) {
				researchCompleted = true;
				_activelyResearching.RemoveAt( i );
				_researchedTechnologies.Add( research );
			}
		}

		if (!researchCompleted)
			return;

		for (int i = _remainingTechnologies.Count - 1; i >= 0; i--) {
			TechResearch research = _remainingTechnologies[ i ];
			research.CheckIfResearching();
			if (research.ActivelyResearching) {
				_activelyResearching.Add( research );
				_remainingTechnologies.RemoveAt( i );
			}
		}
	}

	public TechResearch GetResearch<T>() where T : TechnologyBase => _research[ typeof( T ) ];
}

public sealed class TechResearch : IInitializable {
	public TechResearch( TileTechnologyHolder techHolder, TechnologyBase technology ) {
		this.TechHolder = techHolder;
		this.Technology = technology;
		ActivelyResearching = true;
	}

	public TileTechnologyHolder TechHolder { get; }
	public TechnologyBase Technology { get; }

	public float DiscoveryProgress { get; private set; }
	public bool ResearchCompleted { get; private set; }
	public bool ActivelyResearching { get; private set; }

	public void Initialize() {
		DiscoveryProgress = 0;
		CheckIfResearching();
	}

	public void CheckIfResearching() => ActivelyResearching = Technology.HasPrerequisites( TechHolder ) && !ResearchCompleted;

	public bool UpdateProgress( double deltaTime ) {
		if (!ActivelyResearching)
			return true;
		DiscoveryProgress += Technology.GetDiscoveryProgression( TechHolder ) * (float) deltaTime;
		CheckIfResearching();
		if (ActivelyResearching)
			return false;
		DiscoveryProgress = Technology.RequiredDiscoveryProgress;
		ResearchCompleted = true;
		return true;
	}
}