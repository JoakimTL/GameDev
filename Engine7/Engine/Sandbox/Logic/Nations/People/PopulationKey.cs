namespace Sandbox.Logic.Nations.People;

public readonly struct PopulationKey( PhysicalTrait physicalTrait, ushort birthPopulationCenterId, ushort cultureId, ushort birthYear ) {
	// - Sex
	// - Phenotype
	// - Birthcountry
	// - Culture
	// - Year of birth
	public readonly PhysicalTrait PhysicalTrait = physicalTrait;
	public readonly ushort BirthPopulationCenterId = birthPopulationCenterId;
	public readonly ushort CultureId = cultureId;
	public readonly ushort BirthYear = birthYear;
}