namespace Sandbox.Logic.Nations.People;
public sealed class Census {

	public PopulationCensusDictionary Population { get; }
	public ProfessionCensus Professions { get; }
	//public PoliticalCensus Politics { get; }

	public Census() {
		Population = new();
		Professions = new();
		//Politics = new();
	}

}

public sealed class ProfessionCensus {

}