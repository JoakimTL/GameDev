using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Entities.Container;

public static class SystemTypeManager {

	private static readonly Dictionary<Type, List<Type>> _systemTypesByArchetype = [];
	private static readonly Dictionary<Type, Func<EntityContainer, IUpdateable>> _systemFactories = [];

	static SystemTypeManager() {
		foreach (Type systemType in TypeManager.Registry.GetAllSubclassesOfGenericType( typeof( SystemBase<> ) )) {
			Type archetypeType = systemType.BaseType!.GetGenericArguments().Single();
			if (!_systemTypesByArchetype.TryGetValue( archetypeType, out List<Type>? systemTypes ))
				_systemTypesByArchetype.Add( archetypeType, systemTypes = [] );
			systemTypes.Add( systemType );
			_systemFactories.Add( systemType, CreateSystemFactory( systemType ) );
		}
	}

	private static Func<EntityContainer, IUpdateable> CreateSystemFactory( Type systemType ) {
		TypeManager.AssertHasOnlyParameterlessConstructor( systemType );
		Type systemBaseType = systemType.BaseType ?? throw new InvalidOperationException( $"System {systemType.Name} must have a base type." );

		ParameterExpression entityContainerParam = Expression.Parameter( typeof( EntityContainer ), "entityContainer" );
		NewExpression newSystemExpression = Expression.New( systemType );
		ParameterExpression systemInstance = Expression.Variable( systemType );
		FieldInfo entityContainerFieldInfo = systemBaseType.GetField( "_container", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"SystemBase<>._container field not found." );
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