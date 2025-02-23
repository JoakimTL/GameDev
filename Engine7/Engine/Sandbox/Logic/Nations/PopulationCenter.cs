using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations;
public sealed class PopulationCenter {
	/*
	 * Has people, resources, buildings, culture and technology
	 */

	private readonly Dictionary<PartOfPopulationKey, PartOfPopulation> _population;

}

public sealed class PartOfPopulationKey {
	/// <param name="profession">Which profession this pop has.</param>
	/// <param name="gender">Which gender this pop has.</param>
	/// <param name="intelligenceLevel">The intelligence level of this pop. 0 means base level, 100 grants a 100% efficiency boost from their work.</param>
	/// <param name="educationYears">Number of years this pop has been educated in their profession.</param>
	/// <param name="birthYear">Year of birth, used to determine age.</param>
	/// <param name="living">Whether this pop is alive or dead. Dead pops don't do anything and is used mostly for historical statistics.</param>
	public PartOfPopulationKey( ProfessionBase profession, Gender gender, int intelligenceLevel, int educationYears, int birthYear, bool living ) {
		this.Profession = profession;
		this.Gender = gender;
		this.IntelligenceLevel = intelligenceLevel;
		this.EducationYears = educationYears;
		this.BirthYear = birthYear;
		this.Living = living;
	}

	public ProfessionBase Profession { get; }
	public Gender Gender { get; }
	public int IntelligenceLevel { get; }
	public int EducationYears { get; }
	public int BirthYear { get; }
	public bool Living { get; }
}

public sealed class PartOfPopulation {

	public PartOfPopulation( PartOfPopulationKey key, int count ) {
		this.Key = key;
		this.Count = count;
	}

	public PartOfPopulationKey Key { get; }
	public int Count { get; private set; }

	public void AddPeople( int count ) {
		this.Count += count;
	}

	public void TransferPeople( PartOfPopulation target, int count ) {
		if (this.Count < count)
			throw new InvalidOperationException( "Not enough people to transfer." );
		this.Count -= count;
		target.AddPeople( count );
	}

}

public enum Gender { Male, Female }
