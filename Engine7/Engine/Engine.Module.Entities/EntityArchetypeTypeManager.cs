using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Entities;

public static class EntityArchetypeTypeManager {
	private static readonly Dictionary<Type, HashSet<Type>> _requirementsByArchetypes;
	private static readonly Dictionary<Type, HashSet<Type>> _archetypesByRequirements;
	private static readonly Dictionary<Type, Func<Entity, ArchetypeBase>> _archetypeFactories;

	static EntityArchetypeTypeManager() {
		_requirementsByArchetypes = [];
		_archetypesByRequirements = [];
		_archetypeFactories = [];
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
			_archetypeFactories[ t ] = CreateArchetypeFactoryDelegate( t, properties );
		}
	}

	private static Func<Entity, ArchetypeBase> CreateArchetypeFactoryDelegate( Type t, IReadOnlyList<PropertyInfo> properties ) {
		ConstructorInfo[] constructors = t.GetConstructors();
		if (constructors.Length != 1)
			throw new InvalidOperationException( $"Archetype {t.Name} must have exactly one constructor." );
		ConstructorInfo constructor = constructors[ 0 ];
		ParameterInfo[] parameters = constructor.GetParameters();
		if (parameters.Length != 0)
			throw new InvalidOperationException( $"Archetype {t.Name} must have a parameterless constructor." );

		ParameterExpression entityParam = Expression.Parameter( typeof( Entity ), "entity" );
		MethodInfo getComponentMethod = typeof( Entity ).GetMethod( nameof( Entity.GetComponent ), BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( "Entity.GetComponent method not found." );
		NewExpression archetypeNewExpression = Expression.New( t );
		ParameterExpression archetypeInstance = Expression.Variable( t );

		LabelTarget returnTarget = Expression.Label( t );
		GotoExpression returnExpression = Expression.Return( returnTarget, archetypeInstance, t );
		LabelExpression returnLabel = Expression.Label( returnTarget, Expression.Default( t ) );

		List<Expression> blockExpressions = [];
		blockExpressions.Add( Expression.Assign( archetypeInstance, archetypeNewExpression ) );
		foreach (PropertyInfo property in properties)
			if (property.CanWrite && property.PropertyType.IsAssignableTo( typeof( ComponentBase ) ))
				blockExpressions.Add( Expression.Assign( Expression.Property( archetypeInstance, property ), Expression.Convert( Expression.Call( entityParam, getComponentMethod, Expression.Constant( property.PropertyType ) ), property.PropertyType ) ) );
		blockExpressions.Add( returnExpression );
		blockExpressions.Add( returnLabel );
		BlockExpression block = Expression.Block( [ archetypeInstance ], blockExpressions );

		return Expression.Lambda<Func<Entity, ArchetypeBase>>( block, entityParam ).Compile();
	}

	public static ArchetypeBase CreateArchetypeInstance( Type archetypeType, Entity entity ) {
		ArchetypeBase archetypeInstance = _archetypeFactories[ archetypeType ]( entity );
		archetypeInstance.SetEntity( entity );
		return archetypeInstance;
	}

	public static IReadOnlyCollection<Type> GetRequirementsForArchetype( Type archetype ) => _requirementsByArchetypes[ archetype ];
	public static IReadOnlyCollection<Type> GetRequirementsForArchetype<T>() where T : ArchetypeBase => _requirementsByArchetypes[ typeof( T ) ];
	public static IReadOnlyCollection<Type> GetArchetypesRequiringComponent( Type componentType ) => _archetypesByRequirements[ componentType ];
	public static IReadOnlyCollection<Type> GetArchetypesRequiringComponent<T>() where T : ComponentBase => _archetypesByRequirements[ typeof( T ) ];
}
