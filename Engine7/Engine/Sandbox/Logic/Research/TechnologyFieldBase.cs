namespace Sandbox.Logic.Research;

public abstract class TechnologyFieldBase( string displayName ) {
#pragma warning disable IDE0044 // Add readonly modifier
	private IReadOnlySet<TechnologyFieldBase> _parentFields = null!;
#pragma warning restore IDE0044 // Add readonly modifier
	public string DisplayName { get; } = displayName;
	public IReadOnlySet<TechnologyFieldBase> ParentFields => _parentFields;

	internal void SetParentFields( IReadOnlySet<TechnologyFieldBase> parentFields ) {
		if (_parentFields is not null)
			throw new InvalidOperationException( "Parent fields can't be changed." );
		_parentFields = parentFields;
	}
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?