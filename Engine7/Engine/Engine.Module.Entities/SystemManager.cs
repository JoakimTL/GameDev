using Engine.Structures;
using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Entities;

public class EntityContainerSystemManager : IUpdateable {

	private readonly EntityContainer _container;
	private readonly EntityContainerArchetypeManager _entityContainerArchetypeManager;
	private readonly Dictionary<Type, IUpdateable> _systemTypesByArchetype;
	private readonly TypeDigraph<IUpdateable> _systemUpdateTree;
	private readonly List<IUpdateable> _orderedSystems;

	public EntityContainerSystemManager( EntityContainer container, EntityContainerArchetypeManager entityContainerArchetypeManager ) {
		this._container = container;
		this._entityContainerArchetypeManager = entityContainerArchetypeManager;
		_systemTypesByArchetype = [];
		_systemUpdateTree = new();
		_orderedSystems = [];
		_entityContainerArchetypeManager.OnArchetypeAdded += OnArchetypeAdded;
		_entityContainerArchetypeManager.OnArchetypeRemoved += OnArchetypeRemoved;
	}

	private void OnArchetypeAdded( Type archetypeType ) {
		var newSystemTypes = SystemTypeManager.GetSystemTypesUsingArchetype( archetypeType );
		if (newSystemTypes == null)
			return;
		bool changed = false;
		foreach (Type systemType in newSystemTypes) {
			if (_systemTypesByArchetype.TryGetValue( systemType, out IUpdateable? system ))
				continue;
			system = SystemTypeManager.CreateSystem( systemType, this._container );
			_systemTypesByArchetype.Add( systemType, system );
			_systemUpdateTree.Add( systemType );
			changed = true;
		}

		if (!changed)
			return;

		_orderedSystems.Clear();
		_orderedSystems.AddRange( _systemUpdateTree.GetTypes().Select( p => _systemTypesByArchetype[ p ] ) );
	}

	private void OnArchetypeRemoved( Type archetypeType ) {
		var newSystemTypes = SystemTypeManager.GetSystemTypesUsingArchetype( archetypeType );
		if (newSystemTypes == null)
			return;
		foreach (Type systemType in newSystemTypes) {
			if (!_systemTypesByArchetype.TryGetValue( systemType, out IUpdateable? system ))
				continue;
			_systemTypesByArchetype.Remove( systemType );
			_systemUpdateTree.Remove( systemType );
			_orderedSystems.Remove( system );
		}
	}

	public void Update( double time, double deltaTime ) {
		foreach (IUpdateable system in _orderedSystems)
			system.Update( time, deltaTime );
	}
}


public static class SystemTypeManager {

	private static readonly Dictionary<Type, List<Type>> _systemTypesByArchetype = [];
	private static readonly Dictionary<Type, Func<EntityContainer, IUpdateable>> _systemFactories = [];

	static SystemTypeManager() {
		foreach (Type systemType in TypeManager.AllTypes.Where( p => p.IsAssignableTo( typeof( SystemBase<> ) ) && !p.IsAbstract )) {
			var archetypeType = systemType.GetGenericArguments().Single();
			if (!_systemTypesByArchetype.TryGetValue( archetypeType, out List<Type>? systemTypes ))
				_systemTypesByArchetype.Add( archetypeType, systemTypes = [] );
			systemTypes.Add( systemType );
			_systemFactories.Add( systemType, CreateSystemFactory(systemType) );
		}
	}

	private static Func<EntityContainer, IUpdateable> CreateSystemFactory( Type systemType ) {
		ConstructorInfo[] constructors = systemType.GetConstructors();
		if (constructors.Length != 1)
			throw new InvalidOperationException( $"System {systemType.Name} must have exactly one constructor." );
		ConstructorInfo constructor = constructors[ 0 ];
		ParameterInfo[] parameters = constructor.GetParameters();
		if (parameters.Length != 0)
			throw new InvalidOperationException( $"System {systemType.Name} must have a parameterless constructor." );

		ParameterExpression entityContainerParam = Expression.Parameter( typeof( EntityContainer ), "entityContainer" );
		NewExpression newSystemExpression = Expression.New( systemType );
		ParameterExpression systemInstance = Expression.Variable( systemType );
		FieldInfo entityContainerFieldInfo = typeof( SystemBase<> ).GetField( "_container", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"SystemBase<>._container field not found." );
		MemberExpression entityContainerFieldExpression = Expression.Field( systemInstance, entityContainerFieldInfo );

		LabelTarget returnTarget = Expression.Label( systemType );

		List<Expression> blockExpressions = [];
		blockExpressions.Add( Expression.Assign( systemInstance, newSystemExpression ) );
		blockExpressions.Add( Expression.Assign( entityContainerFieldExpression, entityContainerParam ) );
		blockExpressions.Add( Expression.Return( returnTarget, systemInstance, systemType ) );
		blockExpressions.Add( Expression.Label( returnTarget, Expression.Default( systemType ) ) );
		BlockExpression block = Expression.Block( [ systemInstance ], blockExpressions );

		return Expression.Lambda<Func<EntityContainer, IUpdateable>>( block, entityContainerParam ).Compile();
	}

	public static IReadOnlyList<Type>? GetSystemTypesUsingArchetype( Type archetypeType ) {
		if (_systemTypesByArchetype.TryGetValue( archetypeType, out List<Type>? systemTypes ))
			return systemTypes;
		return null;
	}

	public static IUpdateable CreateSystem( Type systemType, EntityContainer entityContainer ) {
		if (!_systemFactories.TryGetValue( systemType, out Func<EntityContainer, IUpdateable>? factory ))
			throw new InvalidOperationException( $"No factory found for system {systemType.Name}" );
		return factory( entityContainer );
	}

}