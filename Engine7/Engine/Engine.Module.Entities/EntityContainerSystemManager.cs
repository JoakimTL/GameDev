using Engine.Structures;

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
		this._systemTypesByArchetype = [];
		this._systemUpdateTree = new();
		this._orderedSystems = [];
		this._entityContainerArchetypeManager.OnArchetypeAdded += OnArchetypeAdded;
		this._entityContainerArchetypeManager.OnArchetypeRemoved += OnArchetypeRemoved;
	}

	private void OnArchetypeAdded( Type archetypeType ) {
		IReadOnlyList<Type>? newSystemTypes = SystemTypeManager.GetSystemTypesUsingArchetype( archetypeType );
		if (newSystemTypes == null)
			return;
		bool changed = false;
		foreach (Type systemType in newSystemTypes) {
			if (this._systemTypesByArchetype.TryGetValue( systemType, out IUpdateable? system ))
				continue;
			system = SystemTypeManager.CreateSystem( systemType, this._container );
			this._systemTypesByArchetype.Add( systemType, system );
			this._systemUpdateTree.Add( systemType );
			changed = true;
		}

		if (!changed)
			return;

		this._orderedSystems.Clear();
		this._orderedSystems.AddRange( this._systemUpdateTree.GetTypes().Select( p => this._systemTypesByArchetype[ p ] ) );
	}

	private void OnArchetypeRemoved( Type archetypeType ) {
		var newSystemTypes = SystemTypeManager.GetSystemTypesUsingArchetype( archetypeType );
		if (newSystemTypes == null)
			return;
		foreach (Type systemType in newSystemTypes) {
			if (!this._systemTypesByArchetype.TryGetValue( systemType, out IUpdateable? system ))
				continue;
			this._systemTypesByArchetype.Remove( systemType );
			this._systemUpdateTree.Remove( systemType );
			this._orderedSystems.Remove( system );
		}
	}

	public void Update( double time, double deltaTime ) {
		foreach (IUpdateable system in this._orderedSystems)
			system.Update( time, deltaTime );
	}
}
