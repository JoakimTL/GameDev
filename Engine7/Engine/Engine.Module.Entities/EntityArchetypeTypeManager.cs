using System.Reflection;

namespace Engine.Module.Entities;

public static class EntityArchetypeTypeManager {
	private static readonly Dictionary<Type, HashSet<Type>> _requirementsByArchetypes;
	private static readonly Dictionary<Type, HashSet<Type>> _archetypesByRequirements;
	private static readonly Dictionary<Type, Func<Entity, ArchetypeBase>> _archetypeFactories;
	private static readonly Dictionary<Type, Action<ComponentChangeHandler, ArchetypeBase>> _archetypeComponentChangeSubscribers;
	private static readonly Dictionary<Type, Action<ComponentChangeHandler, ArchetypeBase>> _archetypeComponentChangeUnsubscribers;

	static EntityArchetypeTypeManager() {
		_requirementsByArchetypes = [];
		_archetypesByRequirements = [];
		_archetypeFactories = [];
		_archetypeComponentChangeSubscribers = [];
		_archetypeComponentChangeUnsubscribers = [];
		LoadAllArchetypes();
	}

	private static void LoadAllArchetypes() {
		IEnumerable<Type> allArchetypes = TypeManager.AllTypes.Where( p => p.IsClass && !p.IsAbstract && p.IsAssignableTo( typeof( ArchetypeBase ) ) );
		foreach (Type t in allArchetypes) {
			ResolvedType archetype = TypeManager.ResolveType( t );
			IReadOnlyList<PropertyInfo> properties = archetype.GetProperties( BindingFlags.Public | BindingFlags.Instance );
			foreach (PropertyInfo property in properties) {
				if (!property.PropertyType.IsAssignableTo( typeof( ComponentBase ) ))
					continue;
				if (!_requirementsByArchetypes.TryGetValue( t, out HashSet<Type>? requirements ))
					_requirementsByArchetypes.Add( t, requirements = [] );
				requirements.Add( property.PropertyType );
				if (!_archetypesByRequirements.TryGetValue( property.PropertyType, out HashSet<Type>? archetypes ))
					_archetypesByRequirements.Add( property.PropertyType, archetypes = [] );
				archetypes.Add( t );
			}
			_archetypeFactories[ t ] = ArchetypeReflectionHelper.CreateArchetypeFactoryDelegate( t, properties );
			_archetypeComponentChangeSubscribers[ t ] = ArchetypeReflectionHelper.CreateArchetypeComponentChangeSubscriber( t, properties );
			_archetypeComponentChangeUnsubscribers[ t ] = ArchetypeReflectionHelper.CreateArchetypeComponentChangeUnsubscriber( t, properties );
		}
	}

	public static ArchetypeBase CreateArchetypeInstance( this Entity entity, Type archetypeType ) {
		ArchetypeBase archetypeInstance = _archetypeFactories[ archetypeType ]( entity );
		archetypeInstance.SetEntity( entity );
		return archetypeInstance;
	}

	public static void SubscribeToComponentChanges( this ArchetypeBase archetype, ComponentChangeHandler componentChangeHandler ) => _archetypeComponentChangeSubscribers[ archetype.GetType() ]( componentChangeHandler, archetype );

	public static void UnsubscribeFromComponentChanges( this ArchetypeBase archetype, ComponentChangeHandler componentChangeHandler ) => _archetypeComponentChangeUnsubscribers[ archetype.GetType() ]( componentChangeHandler, archetype );

	public static IReadOnlyCollection<Type> GetRequirementsForArchetype( Type archetype ) => _requirementsByArchetypes.TryGetValue( archetype, out HashSet<Type>? requirements ) ? requirements : Type.EmptyTypes;
	public static IReadOnlyCollection<Type> GetRequirementsForArchetype<T>() where T : ArchetypeBase => _requirementsByArchetypes.TryGetValue( typeof( T ), out HashSet<Type>? requirements ) ? requirements : Type.EmptyTypes;
	public static IReadOnlyCollection<Type> GetArchetypesRequiringComponent( Type componentType ) => _archetypesByRequirements.TryGetValue( componentType, out HashSet<Type>? requirements ) ? requirements : Type.EmptyTypes;
	public static IReadOnlyCollection<Type> GetArchetypesRequiringComponent<T>() where T : ComponentBase => _archetypesByRequirements.TryGetValue( typeof( T ), out HashSet<Type>? requirements ) ? requirements : Type.EmptyTypes;
}
