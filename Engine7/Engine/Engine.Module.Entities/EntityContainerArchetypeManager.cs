namespace Engine.Module.Entities;

public sealed class EntityContainerArchetypeManager : IDisposable {

	private readonly HashSet<Entity> _hookedEntities;
	private readonly Dictionary<Type, Dictionary<Guid, ArchetypeBase>> _archetypeDataByArchetypeType;
	private readonly EntityContainer _container;

	public EntityContainerArchetypeManager( EntityContainer container ) {
		this._container = container;
		this._hookedEntities = [];
		this._archetypeDataByArchetypeType = [];
		this._container.OnEntityAdded += OnEntityAdded;
		this._container.OnEntityRemoved += OnEntityRemoved;
	}

	public IEnumerable<TArchetype> GetArchetypes<TArchetype>() where TArchetype : ArchetypeBase
		=> this._archetypeDataByArchetypeType[ typeof( TArchetype ) ].Values.OfType<TArchetype>();

	private void OnComponentAdded( ComponentBase component ) {
		Type componentType = component.GetType();
		IReadOnlyCollection<Type> potentialArchetypes = EntityArchetypeTypeManager.GetArchetypesRequiringComponent( componentType );
		foreach (Type archetypeType in potentialArchetypes) {
			bool isArchetype = component.Entity.IsArchetype( archetypeType );
			if (!isArchetype)
				continue;
			if (!this._archetypeDataByArchetypeType.TryGetValue( archetypeType, out Dictionary<Guid, ArchetypeBase>? archetypeData ))
				this._archetypeDataByArchetypeType.Add( archetypeType, archetypeData = [] );
			archetypeData.Add( component.Entity.EntityId, EntityArchetypeTypeManager.CreateArchetypeInstance( archetypeType, component.Entity ) );
		}
	}

	private void OnComponentRemoved( ComponentBase component ) {
		Type componentType = component.GetType();
		IReadOnlyCollection<Type> affectedArchetypes = EntityArchetypeTypeManager.GetArchetypesRequiringComponent( componentType );
		foreach (Type archetypeType in affectedArchetypes) {
			if (!this._archetypeDataByArchetypeType.TryGetValue( archetypeType, out Dictionary<Guid, ArchetypeBase>? archetypeData ))
				continue;
			archetypeData.Remove( component.Entity.EntityId );
		}
	}

	private void OnEntityAdded( Entity entity ) {
		if (!this._hookedEntities.Add( entity ))
			return;
		entity.ComponentAdded += OnComponentAdded;
		entity.ComponentRemoved += OnComponentRemoved;
	}

	private void OnEntityRemoved( Entity entity ) {
		if (!this._hookedEntities.Remove( entity ))
			return;
		entity.ComponentAdded -= OnComponentAdded;
		entity.ComponentRemoved -= OnComponentRemoved;
	}

	public void Dispose() {
		this._container.OnEntityAdded -= OnEntityAdded;
		this._container.OnEntityRemoved -= OnEntityRemoved;
		foreach (Entity entity in this._hookedEntities) {
			entity.ComponentAdded -= OnComponentAdded;
			entity.ComponentRemoved -= OnComponentRemoved;
		}
		this._hookedEntities.Clear();
	}
}
