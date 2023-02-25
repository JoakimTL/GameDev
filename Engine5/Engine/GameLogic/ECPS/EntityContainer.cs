using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.GameLogic.ECPS;

public sealed class EntityContainer : Identifiable, IUpdateable {

	private readonly List<Entity> _entites;
	private readonly Dictionary<Guid, Entity> _entitesById;
	private readonly Dictionary<Type, HashSet<ComponentBase>> _components;

	private readonly ConcurrentQueue<ComponentBase> _addedComponents;
	private readonly ConcurrentQueue<ComponentBase> _removedComponents;

	internal event EntityComponentEvent? ComponentAdded;
	internal event EntityComponentEvent? ComponentRemoved;

	protected override string UniqueNameTag => $"{_entites.Count} E / {_components.Count} CT / {_components.Values.Sum( p => p.Count )} C";

	public static Guid TypeIdentity { get; } = new Guid( "2adb2c13-7923-4f7b-bcb1-09c963f08282" );

	public EntityContainer() {
		_entites = new();
		_entitesById = new();
		_components = new();
		_addedComponents = new();
		_removedComponents = new();
	}

	public Entity Create() {
		Guid id = Guid.NewGuid();
		while ( _entitesById.ContainsKey( id ) ) {
			id = Guid.NewGuid();
			this.LogText( "Just wanted to say that a GUID collision occured. The collision has been averted :)", Log.Level.CRITICAL );
		}
		Entity e = new();
		e.SetId( id );
		Add( e );
		return e;
	}

	public Entity? Load( byte[] serializedData ) {
		if ( serializedData.Deserialize() is not Entity e )
			return null;
		Add( e );
		return e;
	}

	private void Add( Entity e ) {
		if ( !_entitesById.TryAdd( e.EntityId, e ) ) {
			this.LogWarning( $"Entity with id {e.EntityId} already exists." );
			return;
		}
		_entites.Add( e );
		e.ComponentAdded += ComponentAddedHandler;
		e.ComponentRemoved += ComponentRemovedHandler;
		e.ParentIdChanged += EntityParentIdChangeHandler;
		foreach ( var compontent in e.Components )
			_addedComponents.Enqueue( compontent );
	}

	public bool Remove( Entity e ) {
		if ( !_entites.Remove( e ) )
			return false;
		e.ComponentAdded -= ComponentAddedHandler;
		e.ComponentRemoved -= ComponentRemovedHandler;
		foreach ( var compontent in e.Components )
			_removedComponents.Enqueue( compontent );
		return true;
	}

	private void ComponentAddedHandler( ComponentBase component ) => _addedComponents.Enqueue( component );

	private void ComponentRemovedHandler( ComponentBase component ) => _removedComponents.Enqueue( component );

	private void EntityParentIdChangeHandler( Entity child, Guid? parent ) {
		if ( !parent.HasValue ) {
			child.SetParent( null );
			return;
		}
		_entitesById.TryGetValue( parent.Value, out var parentEntity );
		Entity? lcpe = parentEntity;
		while ( lcpe is not null ) {
			if ( !lcpe.ParentId.HasValue )
				break;
			Guid pG = lcpe.ParentId.Value;
			if ( pG == child.EntityId ) {
				this.LogWarning( "Entities can't form looping parentage." );
				child.SetParentId( null );
				return;
			}
			_entitesById.TryGetValue( pG, out lcpe );
		}
		child.SetParent( parentEntity );
	}

	public void Update( float time, float deltaTime ) {
		while ( _addedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				_components.Add( t, set = new() );
			set.Add( component );
			ComponentAdded?.Invoke( component );
		}

		while ( _removedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				continue;
			set.Remove( component );
			if ( set.Count == 0 )
				_components.Remove( t );
			ComponentRemoved?.Invoke( component );
		}
	}

	public IEnumerable<T> GetComponents<T>() where T : ComponentBase
		=> _components.TryGetValue( typeof( T ), out var components ) ? components.OfType<T>() : Enumerable.Empty<T>();

	public byte[][] SerializeEntities() {
		byte[][] serializedEntities = new byte[ _entites.Count ][];
		for ( int i = 0; i < _entites.Count; i++ )
			serializedEntities[ i ] = _entites[ i ].Serialize();
		return serializedEntities;
	}

	//Update manipulator list
	// (Check if all active manipulators are valid still)
	// (Check is any new components added validates other manipulators)
	//Run Update on manipulators

}

