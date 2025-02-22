using Sandbox.Logic.Old.OldCiv.Population;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Logic.Old.OldCiv.Research;

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

//Each nation can have tech hubs. They start with 1, and can split off tiles in their territories into new tech hubs. Tech hubs can also be deleted, but must be done by slowly taking one tile out and into another neighbouring tech hub. The neighbouring tech hub then gets either a tech bump or tech reversal depending on the tech levels in the receiving and giving hub and the population of the tile compared to the hubs. You shouldn't be able to game this system for more tech gains. Tech hubs which neighbour eachother share technology cross border over time.
//This is mostly done to keep computations manageable. I foresee each nation (with there being probably 500 ish nations) being able to field maybe 50 tech hubs each by late game.

public sealed class TechnologyHub {

	private readonly List<Tile> _tilesInHub;
	private readonly TechnologyResearcher _researcher;

	public TechnologyHub( TechnologyHub? derivedFrom, Tile originTile ) {
		_tilesInHub = [ originTile ];
		_researcher = new TechnologyResearcher( derivedFrom?._researcher.ResearchedTechnologyTypes.ToHashSet(), null );
	}

	public void AddTile( Tile tile ) {
		_tilesInHub.Add( tile );
	}

	public void RemoveTile( Tile tile ) {
		_tilesInHub.Remove( tile );
	}

	public void MoveTile( Tile tile, TechnologyHub targetHub ) {
		RemoveTile( tile );
		targetHub.AddTile( tile );
	}

	public void Update( double time, double deltaTime ) {
		_researcher.Update( time, deltaTime );
	}

}

/// <summary>
/// The makeup of people in the hub. This is used to determine the research speed of different technology fields in the hub.
/// </summary>
public sealed class TechnologyHubResearchMakeup {

}

//TODO: Research slowly degrades when no profession is there to maintain the knowledge. This is mitigated by writing/libraries/other written storage
public sealed class TechnologyResearcher : IUpdateable {
	private readonly Dictionary<Type, TechnologyResearch> _research;
	private readonly List<TechnologyResearch> _activelyResearching;
	private readonly List<TechnologyResearch> _remainingTechnologies;
	private readonly List<TechnologyResearch> _researchedTechnologies;
	private readonly Census _census;

	public TechnologyResearcher( HashSet<Type>? alreadyResearchedTechnologies, Census census ) {
		_research = [];
		_activelyResearching = [];
		_remainingTechnologies = [];
		_researchedTechnologies = [];
		foreach (TechnologyBase technology in TechnologyList.GetTechnologies())
			_research[ technology.GetType() ] = new TechnologyResearch( this, technology );
		foreach (TechnologyResearch research in _research.Values) {
			research.Initialize();
			if (alreadyResearchedTechnologies?.Contains( research.GetType() ) ?? false)
				research.Complete();
		}
		foreach (TechnologyResearch research in _research.Values) {
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

		this._census = census;
	}

	public void Update( double time, double deltaTime ) {
		bool researchCompleted = false;

		for (int i = _activelyResearching.Count - 1; i >= 0; i--) {
			TechnologyResearch research = _activelyResearching[ i ];
			if (research.UpdateProgress( deltaTime )) {
				researchCompleted = true;
				_activelyResearching.RemoveAt( i );
				_researchedTechnologies.Add( research );
			}
		}

		if (!researchCompleted)
			return;

		for (int i = _remainingTechnologies.Count - 1; i >= 0; i--) {
			TechnologyResearch research = _remainingTechnologies[ i ];
			research.CheckIfResearching();
			if (research.ActivelyResearching) {
				_activelyResearching.Add( research );
				_remainingTechnologies.RemoveAt( i );
			}
		}
	}

	public TechnologyResearch GetResearchFor<T>() where T : TechnologyBase => _research[ typeof( T ) ];

	public IEnumerable<Type> ResearchedTechnologyTypes => _researchedTechnologies.Select( p => p.Technology.GetType() );
}

public sealed class TechnologyResearch : IInitializable {
	public TechnologyResearch( TechnologyResearcher techHolder, TechnologyBase technology ) {
		this.TechHolder = techHolder;
		this.Technology = technology;
		ActivelyResearching = true;
		ResearchCompleted = technology.RequiredDiscoveryProgress <= 0;
	}

	public TechnologyResearcher TechHolder { get; }
	public TechnologyBase Technology { get; }

	public float DiscoveryProgress { get; private set; }
	public bool ResearchCompleted { get; private set; }
	public bool ActivelyResearching { get; private set; }

	public void Initialize() {
		DiscoveryProgress = 0;
		CheckIfResearching();
	}

	public void CheckIfResearching() => ActivelyResearching = /*Technology.HasPrerequisites( TechHolder )*/ /*&&*/ !ResearchCompleted;

	public bool UpdateProgress( double deltaTime ) {
		DiscoveryProgress += Technology.GetResearchProgressionModifier( TechHolder ) * (float) deltaTime;
		if (DiscoveryProgress < Technology.RequiredDiscoveryProgress)
			return false;
		DiscoveryProgress = Technology.RequiredDiscoveryProgress;
		ResearchCompleted = true;
		return true;
	}

	public void Complete() {
		DiscoveryProgress = Technology.RequiredDiscoveryProgress;
		ResearchCompleted = true;
	}
}