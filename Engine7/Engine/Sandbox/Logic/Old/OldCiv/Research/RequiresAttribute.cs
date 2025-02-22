namespace Sandbox.Logic.Old.OldCiv.Research;

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
public sealed class RequiresAttribute<T> : Attribute, IRequires where T : TechnologyBase {
	public Type RequiredTechnology { get; } = typeof( T );
}