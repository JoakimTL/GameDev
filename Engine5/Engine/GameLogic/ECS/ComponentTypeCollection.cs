using System.Diagnostics.CodeAnalysis;

namespace Engine.GameLogic.ECS;

public sealed class ComponentTypeCollection : IEqualityComparer<ComponentTypeCollection> {

	public static ComponentTypeCollection Empty { get; } = new( Enumerable.Empty<Type>() );

	public IReadOnlyCollection<Type> ComponentTypes => _componentTypes;

	private HashSet<Type> _componentTypes;
	private int _hashCode;

	public ComponentTypeCollection( IEnumerable<Type> componentTypes ) {
		_componentTypes = componentTypes.ToHashSet();
		HashCode hash = new();
		foreach ( var c in _componentTypes.OrderBy( p => p.GUID ) )
			hash.Add( c );
		_hashCode = hash.ToHashCode();
	}

	public override bool Equals( object? obj )
		=> obj is ComponentTypeCollection ctc && Equals( this, ctc );

	public override int GetHashCode()
		=> _hashCode;

	public bool Equals( ComponentTypeCollection? x, ComponentTypeCollection? y )
		=> x is not null && y is not null && x._hashCode == y._hashCode && !x._componentTypes.Except( y._componentTypes ).Any();

	public int GetHashCode( [DisallowNull] ComponentTypeCollection obj )
		=> obj._hashCode;
}
