using System.Collections.Concurrent;

namespace Engine.Module.Entities.Container;

//Listens to changes in the entity and reflects them onto itself. It is essentially a copy of the entity, but with no logic. This is to allow the render entity and its behaviours to determine the behaviour, but with the newest data from the entity at this frame.
public sealed class SynchronizedEntity : IUpdateable {

	private readonly Entity _entity;
	private readonly Dictionary<Type, ComponentBase> _components;
	private readonly Dictionary<Type, ArchetypeBase> _archetypes;

	private readonly ConcurrentQueue<Type> _addedComponents;
	private readonly ConcurrentQueue<Type> _removedComponents;
	private readonly ConcurrentQueue<ComponentSerializationResult> _componentSerializationResults;
	private EntitySerializationResult? _entitySerializationResult;

	public SynchronizedEntity(Entity entity) {
		_entity = entity;
		_components = [];
		_archetypes = [];
		_addedComponents = [];
		_removedComponents = [];
		_componentSerializationResults = [];
		_entity.ComponentAdded += OnComponentAdded;
		_entity.ComponentRemoved += OnComponentRemoved;
	}

	public void Update( double time, double deltaTime ) {
		while (_addedComponents.TryDequeue( out Type? addedComponentType )) {
			if (!addedComponentType.IsAssignableTo(typeof(ISerializableComponent)) || _components.ContainsKey( addedComponentType ))
				continue;
			ComponentBase component = addedComponentType.Resolve().CreateInstance( null ) as ComponentBase ?? throw new InvalidOperationException( $"Failed to create instance of {addedComponentType.Name}." );
			_components.Add( addedComponentType, component );
		}
		while (_removedComponents.TryDequeue( out Type? removedComponentType )) {
			if (!_components.Remove( removedComponentType, out ComponentBase? component ))
				continue;
			component.Dispose();
		}
		while (_componentSerializationResults.TryDequeue( out SerializationResult? serializationResult )) {
			if (serializationResult is null)
				continue;
			serializationResult.Dispose();
		}
	}

	//Called from Entity
	internal void Synchronize() {
		_entitySerializationResult = _entity.Serialize();
	}

	private void OnComponentAdded( ComponentBase component ) {
		_addedComponents.Enqueue( component.GetType() );
	}

	private void OnComponentRemoved( ComponentBase component ) {
		_removedComponents.Enqueue( component.GetType() );
	}
}
