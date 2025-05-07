using Engine.Logging;

namespace Civs.Logic.Setup;

public abstract class ResourceTypeBase : SelfIdentifyingBase {
	public string Name { get; }
	/// <summary>
	/// Over a year of game time how much of the original stockpile has perished?
	/// Need a way to modify it such that technologies and other things can reduce the perishing rate. 
	/// </summary>
	public double PerishingRate { get; }

	protected ResourceTypeBase( string name ) {
		this.Name = name;
	}
}

