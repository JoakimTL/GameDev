namespace Civlike.Logic.Setup;

public abstract class ResourceTypeBase( string name, IEnumerable<string> tags ) : SelfIdentifyingBase {
	public string Name { get; } = name;
	public IReadOnlySet<string> Tags { get; } = tags.ToHashSet();

	/// <summary>
	/// Over a year of game time how much of the original stockpile has perished?
	/// Need a way to modify it such that technologies and other things can reduce the perishing rate. 
	/// </summary>
	public double PerishingRate { get; }
}

