using Sandbox.Logic.Setup;
using Sandbox.Logic.Setup.MarketSectors;
using Sandbox.Logic.Setup.Social;

namespace Sandbox.Logic.Nations.People;

public sealed class Population {

	public uint Births { get; private set; }
	public uint Deaths { get; private set; }
	public uint Emigrations { get; private set; }
	public uint Immigrations { get; private set; }

	public ProfessionSubCensus Professions { get; }
	//public PoliticalSubCensus PoliticalParties { get; }

	public uint CurrentPopulationCount => Births + Immigrations - Deaths - Emigrations;

	public Population() {
		Births = 0;
		Deaths = 0;
		Emigrations = 0;
		Immigrations = 0;
		Professions = new();
	}

	public void AddBirths( uint count ) {
		Births += count;
		Professions.AddPeople( Definitions.Professions.Get<DependentProfession>(), 0, count );
	}

	public void AddDeaths( ProfessionKey profession, uint count ) {
		if (CurrentPopulationCount < count)
			throw new ArgumentOutOfRangeException( nameof( count ), "Deaths cannot exceed current population." );
		Deaths += count;
	}

	public void AddImmigrants( ProfessionKey profession, uint count ) {
		Immigrations += count;
	}

	public void AddEmigrants( ProfessionKey profession, uint count ) {
		if (CurrentPopulationCount < count)
			throw new ArgumentOutOfRangeException( nameof( count ), "Emigrations cannot exceed current population." );
		Emigrations += count;
	}

}


public sealed class PoliticalSubCensus {
	private readonly Dictionary<PoliticalParty, uint> _populationByParty;
}