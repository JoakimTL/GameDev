using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Entities.Container;

public static class ArchetypeReflectionHelper {
	internal static Func<Entity, ArchetypeBase> CreateArchetypeFactoryDelegate( Type t, IReadOnlyList<PropertyInfo> properties ) {
		TypeManager.AssertHasOnlyParameterlessConstructor( t );

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

	internal static Action<ComponentChangeHandler, ArchetypeBase> CreateArchetypeComponentChangeSubscriber( Type t, IReadOnlyList<PropertyInfo> properties ) {
		ParameterExpression componentChangeHandlerParam = Expression.Parameter( typeof( ComponentChangeHandler ), "componentChangeHandler" );
		ParameterExpression archetypeParam = Expression.Parameter( typeof( ArchetypeBase ), "archetype" );
		ParameterExpression archetypeConvertedInstance = Expression.Variable( t );
		MethodInfo addChangedListenerMethod = typeof( ComponentBase ).GetMethod( nameof( ComponentBase.AddChangedListener ), BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"Component {typeof( ComponentBase ).Name} does not have {nameof( ComponentBase.AddChangedListener )} method." );
		List<Expression> blockExpressions = [];
		blockExpressions.Add( Expression.Assign( archetypeConvertedInstance, Expression.Convert( archetypeParam, t ) ) );
		foreach (PropertyInfo property in properties)
			if (property.CanWrite && property.PropertyType.IsAssignableTo( typeof( ComponentBase ) )) {
				MemberExpression prop = Expression.Property( archetypeConvertedInstance, property );
				MethodCallExpression call = Expression.Call( prop, addChangedListenerMethod, componentChangeHandlerParam );
				blockExpressions.Add( call );
			}
		BlockExpression block = Expression.Block( [ archetypeConvertedInstance ], blockExpressions );
		return Expression.Lambda<Action<ComponentChangeHandler, ArchetypeBase>>( block, componentChangeHandlerParam, archetypeParam ).Compile();
	}

	internal static Action<ComponentChangeHandler, ArchetypeBase> CreateArchetypeComponentChangeUnsubscriber( Type t, IReadOnlyList<PropertyInfo> properties ) {
		ParameterExpression componentChangeHandlerParam = Expression.Parameter( typeof( ComponentChangeHandler ), "componentChangeHandler" );
		ParameterExpression archetypeParam = Expression.Parameter( typeof( ArchetypeBase ), "archetype" );
		ParameterExpression archetypeConvertedInstance = Expression.Variable( t );
		MethodInfo removeChangedListenerMethod = typeof( ComponentBase ).GetMethod( nameof( ComponentBase.RemoveChangedListener ), BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"Component {typeof( ComponentBase ).Name} does not have {nameof( ComponentBase.RemoveChangedListener )} method." );
		List<Expression> blockExpressions = [];
		blockExpressions.Add( Expression.Assign( archetypeConvertedInstance, Expression.Convert( archetypeParam, t ) ) );
		foreach (PropertyInfo property in properties)
			if (property.CanWrite && property.PropertyType.IsAssignableTo( typeof( ComponentBase ) )) {
				MemberExpression prop = Expression.Property( archetypeConvertedInstance, property );
				MethodCallExpression call = Expression.Call( prop, removeChangedListenerMethod, componentChangeHandlerParam );
				blockExpressions.Add( call );
			}
		BlockExpression block = Expression.Block( [ archetypeConvertedInstance ], blockExpressions );
		return Expression.Lambda<Action<ComponentChangeHandler, ArchetypeBase>>( block, componentChangeHandlerParam, archetypeParam ).Compile();
	}
}