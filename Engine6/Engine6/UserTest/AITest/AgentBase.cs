using Engine;
using Engine.Data;
using Engine.Modules.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UserTest.AITest.Actions;

namespace UserTest.AITest;
public abstract class AgentBase : ComponentBase {

	//Agents have needs, which are the things they need to survive. This includes food, water, warmth, etc.
	//Agents have desires, which are the things they want to do. This includes socializing, working, procreating, etc.
	//Agents have goals, which are the things they want to achieve. These goals can be short-term or long-term. They are a way for the agents to achieve their needs and desires.
	//Agents have knowledge, which is the information they have. This is used when making decisions to fulfill their goals.
	/*
	 * Agents have a personality, which defines the way they behave.
	 * This includes their inventiveness, curiosity, efficiency, organization, outgoingness, energy, friendliness, compassion, phsycological sensitivity and nervousness.
	 * These personality values has a range from -1 to 1, where -1 is the complete opposite of the trait and 1 is the trait in its full extent.
	 * Opposites are consistency, cautiousness, laziness, carelessness, solitaryness, reservedness, criticallity, judmentalness, phsycological resilient and confidence.
	 * Opposites does not necessarily mean negative, it's just the opposite of the trait.
	 */

	//When agents try to perform a goal, they will do so influenced by their emotional state and personality.
	//This is to make even an otherwise eemotionally stable agent to act irrationally when under stress.
	//The emotional state of an agent is affected by their needs, other agents behavior, their own behavior and their personality.

	public PersonalitySpectrum Personality { get; }
	public AgentWants Wants { get; }
	public AgentRelationships Relationships { get; }
	public AgentMemory Memory { get; }

	public AgentBase() {
		Personality = new();
		Wants = new();
		Relationships = new();
		Memory = new();
	}

}

public sealed class AgentMemory {

	private readonly Dictionary<object, object> _memories;

	public AgentMemory() {
		_memories = [];
	}

	public void Remember( object key, object memory ) => _memories[ key ] = memory;
	public object? GetMemory( object key ) => _memories.GetValueOrDefault( key );

}

public abstract class AgentBase<TVector>( AgentKnowledge<TVector> knowledge ) : AgentBase
	where TVector :
		unmanaged, IVector<TVector, double> {

	public AgentKnowledge<TVector> Knowledge { get; } = knowledge;

	//What should the parameters be?
	//How should the actionpath be created?
	public IEnumerable<IAgentAction> CreateActionPath( AgentGoalBase<AgentBase<TVector>> goal ) {
		return Enumerable.Empty<IAgentAction>();
	}

}

public sealed class AgentMood {

}

public sealed class AgentWants {

	private readonly Dictionary<int, double> _wants;

	public double Get( WantBase want ) => _wants[ want.Id ];

	public double DeterminePriority( AgentBase agent, WantBase want ) => want.DeterminePriority( agent );

}



public sealed class Inventory {
	private List<Entity> _items;

}

public abstract class AgentKnowledge<TVector>
	where TVector :
		unmanaged, IVector<TVector, double> {

	//The entire point of this class is to make sure agents have their most up to date knowledge, but their knowledge may not be up to date.

	private ConditionalWeakTable<Entity, HashSet<Type>> _entityDescriptions;
	private Dictionary<Type, Dictionary<Guid, TVector?>> _objectPersistence;
	private Dictionary<Guid, WeakReference<Entity>> _entityFromGuid;

	public bool Update( Entity e ) {
		TVector? location = GetLocation( e );
		if (!location.HasValue)
			return false;

		if (!_entityDescriptions.TryGetValue( e, out HashSet<Type>? descriptions ))
			_entityDescriptions.Add( e, descriptions = [] );

		foreach (ComponentBase c in e.Components) {
			if (!_objectPersistence.TryGetValue( c.GetType(), out Dictionary<Guid, TVector?>? entityPositions ))
				_objectPersistence.Add( c.GetType(), entityPositions = [] );
			entityPositions[ e.EntityId ] = location.Value;
			descriptions.Add( c.GetType() );
		}

		Type[] toRemove = descriptions.Except( e.Components.Select( c => c.GetType() ) ).ToArray();
		foreach (Type t in toRemove) {
			descriptions.Remove( t );
			_objectPersistence[ t ].Remove( e.EntityId );
		}

		return true;
	}

	public IEnumerable<TVector?> LocateAll<T>() where T : ComponentBase
		=> _objectPersistence.TryGetValue( typeof( T ), out Dictionary<Guid, TVector?>? entityPositions )
			? entityPositions.Select( p => p.Value )
			: [];

	public TVector? LocateClosest<T>( TVector centerOfSearch ) where T : ComponentBase
		=> _objectPersistence.TryGetValue( typeof( T ), out Dictionary<Guid, TVector?>? entityPositions )
			? entityPositions.OrderBy( p => p.Value?.Subtract( centerOfSearch ).MagnitudeSquared() ?? double.MaxValue ).FirstOrDefault().Value
			: null;

	protected abstract TVector? GetLocation( Entity e );
}

public interface ILivingObject {
	bool IsAlive { get; }
}

public interface IFood {
	void Eat( AgentBase eatingAgent );
}

public sealed class Apple : ILivingObject, IFood {
	public bool IsAlive { get; set; }

	public void Eat( AgentBase eatingAgent ) {
		//eatingAgent.Wants.ReduceFood( 10 );
	}
}


public sealed class EatFoodAction : IAgentAction {
	private readonly AgentBase _agent;
	private readonly IFood _food;

	public EatFoodAction( AgentBase agent, IFood food ) {
		this._agent = agent;
		_food = food;
	}

	public ActionState PerformAction() {
		_food.Eat( _agent );
		return ActionState.Completed;
	}
}