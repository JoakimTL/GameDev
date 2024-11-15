using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Entities.Render;

public static class DependentRenderBehaviourTypeManager {
	private static readonly Dictionary<Type, List<Type>> _typesDependentOnComponent;
	private static readonly Dictionary<Type, Func<ArchetypeBase, RenderBehaviourBase>> _dependentRenderBehaviourFactories;

	static DependentRenderBehaviourTypeManager() {
		_typesDependentOnComponent = [];
		_dependentRenderBehaviourFactories = [];
		LoadAllDependents();
	}

	public static IReadOnlyCollection<Type>? GetAllDependentBehaviours( Type archetypeType ) => _typesDependentOnComponent.GetValueOrDefault( archetypeType );

	public static RenderBehaviourBase CreateBehaviour( Type dependentType, ArchetypeBase archetype ) => _dependentRenderBehaviourFactories[ dependentType ]( archetype );

	private static void LoadAllDependents() {
		foreach (Type type in TypeManager.GetAllSubclassesOfGenericType( typeof( DependentRenderBehaviourBase<> ) )) {
			Type archetypeType = type.BaseType!.GetGenericArguments()[ 0 ] ?? throw new InvalidOperationException( "DependentRenderBehaviourBase must have a generic type." );
			if (!_typesDependentOnComponent.TryGetValue( archetypeType, out List<Type>? dependentTypes ))
				_typesDependentOnComponent.Add( archetypeType, dependentTypes = [] );
			dependentTypes.Add( type );
			_dependentRenderBehaviourFactories.Add( type, CreateDependentBehaviourFactory( type, archetypeType ) );
		}
	}

	private static Func<ArchetypeBase, RenderBehaviourBase> CreateDependentBehaviourFactory( Type dependentType, Type archetypeType ) {
		TypeManager.AssertHasOnlyParameterlessConstructor( dependentType );

		Type constructedBaseType = typeof( DependentRenderBehaviourBase<> ).MakeGenericType( archetypeType );

		MethodInfo setArchetypeMethod = dependentType.GetMethod( nameof( DependentRenderBehaviourBase<ArchetypeBase>.SetArchetype ), BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException( $"Method {nameof( DependentRenderBehaviourBase<ArchetypeBase>.SetArchetype )} not found." );

		ParameterExpression archetypeParam = Expression.Parameter( typeof( ArchetypeBase ), "archetype" );
		UnaryExpression castArchetypeParam = Expression.TypeAs( archetypeParam, archetypeType );

		NewExpression dependentNewExpression = Expression.New( dependentType );
		ParameterExpression dependentInstance = Expression.Variable( dependentType );

		LabelTarget returnTarget = Expression.Label( dependentType );
		GotoExpression returnExpression = Expression.Return( returnTarget, dependentInstance, dependentType );
		LabelExpression returnLabel = Expression.Label( returnTarget, Expression.Default( dependentType ) );

		List<Expression> blockExpressions = [];
		blockExpressions.Add( Expression.Assign( dependentInstance, dependentNewExpression ) );
		blockExpressions.Add( Expression.Call( dependentInstance, setArchetypeMethod, castArchetypeParam ) );
		blockExpressions.Add( returnExpression );
		blockExpressions.Add( returnLabel );
		BlockExpression block = Expression.Block( [ dependentInstance ], blockExpressions );

		return Expression.Lambda<Func<ArchetypeBase, RenderBehaviourBase>>( block, archetypeParam ).Compile();
	}

	public static void AddDependenciesOnArchetype( this RenderEntity renderEntity, ArchetypeBase archetype ) {
		IReadOnlyCollection<Type>? allDependentTypes = GetAllDependentBehaviours( archetype.GetType() );
		if (allDependentTypes is null)
			return;
		foreach (Type dependentType in allDependentTypes) {
			RenderBehaviourBase behaviour = CreateBehaviour( dependentType, archetype );
			behaviour.SetRenderEntity( renderEntity );
			renderEntity.AddBehaviour( behaviour );
		}
	}

	public static void RemoveAllDependentsOnArchetype( this RenderEntity renderEntity, ArchetypeBase archetype ) {
		IReadOnlyCollection<Type>? allDependentTypes = GetAllDependentBehaviours( archetype.GetType() );
		if (allDependentTypes is null)
			return;
		foreach (Type dependentType in allDependentTypes) {
			renderEntity.RemoveBehaviour( dependentType );
		}
	}
}
