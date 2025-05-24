using Engine.Logging;
using Engine.Module.Entities.Container;
using System.Linq.Expressions;
using System.Reflection;

namespace Engine.Module.Render.Entities;

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
		foreach (Type type in TypeManager.Registry.GetAllNonAbstractSubclassesOf( typeof( DependentRenderBehaviourBase<> ) )) {
			Type archetypeType = type.BaseType!.GetGenericArguments()[ 0 ] ?? throw new InvalidOperationException( "DependentRenderBehaviourBase must have a generic type." );
			if (!_typesDependentOnComponent.TryGetValue( archetypeType, out List<Type>? dependentTypes ))
				_typesDependentOnComponent.Add( archetypeType, dependentTypes = [] );
			dependentTypes.Add( type );
			_dependentRenderBehaviourFactories.Add( type, CreateDependentBehaviourFactory( type, archetypeType ) );
#if DEBUG
			PerformDependencySerializationCheck( type, archetypeType );
#endif
		}
	}

	private static void PerformDependencySerializationCheck( Type dependentType, Type archetypeType ) {
		List<Type> missingSerializers = [];
		foreach (Type componentType in archetypeType.GetProperties().Where( p => p.CanWrite ).Select( p => p.PropertyType )) {
			if (TypeManager.Serializers.GetSerializerType( componentType ) is not null)
				continue;
			missingSerializers.Add( componentType );
		}
		if (missingSerializers.Count == 0)
			return;
		Log.Warning( $"Rendering of {dependentType.Name} will not work. Reason is missing serializers for {string.Join( ", ", missingSerializers.Select( p => p.Name ) )}." );
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
			if (renderEntity.HasBehaviour( dependentType ))
				continue;
			RenderBehaviourBase behaviour = CreateBehaviour( dependentType, archetype );
			behaviour.SetRenderEntity( renderEntity );
			renderEntity.AddBehaviour( behaviour );
		}
	}

	public static void RemoveAllDependentsOnArchetype( this RenderEntity renderEntity, ArchetypeBase archetype ) {
		IReadOnlyCollection<Type>? allDependentTypes = GetAllDependentBehaviours( archetype.GetType() );
		if (allDependentTypes is null)
			return;
		foreach (Type dependentType in allDependentTypes)
			renderEntity.RemoveBehaviour( dependentType );
	}
}
