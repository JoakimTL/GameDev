namespace Sandbox.Logic.Old.OldCiv.Research;

public enum TechnologyKind {
	/// <summary>
	/// Research performed after a discovery of the potential new technology.
	/// </summary>
	Research,
	/// <summary>
	/// A larger research project which needs planning and coordination. This needs to be initialized by the player, and are available as a result of research prior.<br/>
	/// An example would be nuclear explosives. A requirement would be nuclear fission researched. The player can determine how many resources to pool into research projects.
	/// </summary>
	Project,
	/// <summary>
	/// The improvement of a technology to make it more efficient.
	/// </summary>
	Improvement,
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?