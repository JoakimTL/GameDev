namespace Engine.Module.Entities.Container;

public sealed class EntityContainerArchetypeManager {

	private readonly Dictionary<Type, Dictionary<Guid, ArchetypeBase>> _archetypeDataByArchetypeType;
	private readonly EntityContainer _container;

	public event EntityArchetypeChangeHandler? ArchetypeAdded;
	public event EntityArchetypeChangeHandler? ArchetypeRemoved;

	public EntityContainerArchetypeManager( EntityContainer container ) {
		this._container = container;
		this._archetypeDataByArchetypeType = [];
		this._container.CreateListChangeHandler( OnEntityAdded, OnEntityRemoved );
	}

	private void OnEntityAdded( Entity entity ) {
		entity.ArchetypeAdded += OnArchetypeAdded;
		entity.ArchetypeRemoved += OnArchetypeRemoved;
	}

	private void OnEntityRemoved( Entity entity ) {
		entity.ArchetypeAdded -= OnArchetypeAdded;
		entity.ArchetypeRemoved -= OnArchetypeRemoved;
	}

	private void OnArchetypeAdded( ArchetypeBase archetype ) {
		if (!this._archetypeDataByArchetypeType.TryGetValue( archetype.GetType(), out Dictionary<Guid, ArchetypeBase>? archetypeData ))
			this._archetypeDataByArchetypeType.Add( archetype.GetType(), archetypeData = [] );
		archetypeData.Add( archetype.Entity.EntityId, archetype );
		ArchetypeAdded?.Invoke( archetype );
	}

	private void OnArchetypeRemoved( ArchetypeBase archetype ) {
		if (!this._archetypeDataByArchetypeType.TryGetValue( archetype.GetType(), out Dictionary<Guid, ArchetypeBase>? archetypeData ))
			return;
		archetypeData.Remove( archetype.Entity.EntityId );
		ArchetypeRemoved?.Invoke( archetype );
		if (archetypeData.Count == 0)
			this._archetypeDataByArchetypeType.Remove( archetype.GetType() );
	}

	public IEnumerable<TArchetype> GetArchetypes<TArchetype>() where TArchetype : ArchetypeBase
		=> this._archetypeDataByArchetypeType[ typeof( TArchetype ) ].Values.OfType<TArchetype>();
}
