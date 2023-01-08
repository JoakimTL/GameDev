namespace Engine.ECS;

public class ComponentTypeCollection {

	public IReadOnlyList<Type> ComponentTypes => _componentTypes;

	private Type[] _componentTypes;
	private int _hashCode;

	public ComponentTypeCollection( IEnumerable<Type> componentTypes ) {
		_componentTypes = componentTypes.ToArray();
		_hashCode = HashCode.Combine( _componentTypes );
	}

	public override int GetHashCode() {
		return _hashCode;
	}
}
