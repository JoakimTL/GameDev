namespace Sandbox.Logic.Nations.People;

public readonly struct PopulationWithKey( Population population, PopulationKey key ) {
	public readonly Population Population = population;
	public readonly PopulationKey Key = key;
}