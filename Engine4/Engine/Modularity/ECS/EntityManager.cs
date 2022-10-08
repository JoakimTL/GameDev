using System.Collections.Concurrent;
using Engine;
using Engine.Data;

namespace Engine.Modularity.ECS;
public class EntityManager : ModuleService, IUpdateable {

	public bool Active => true;
	public bool SaveOnDispose { get; set; }
	private readonly ConcurrentDictionary<Guid, Entity> _entities;
	private readonly ConcurrentDictionary<string, Entity> _entitiesByName;
	/// <summary>
	/// This is not a thread-safe collection!
	/// </summary>
	public IReadOnlyCollection<KeyValuePair<Guid, Entity>> Entities => this._entities;
	private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, Entity>> _entitiesByPlayer;
	public event Action<Entity, Component>? OnEntityComponentAdded;
	public event Action<Entity, Component>? OnEntityComponentRemoved;
	public event Action<Entity, Component>? OnEntityComponentChanged;
	public event Action<Entity>? OnEntityAdded;
	public event Action<Entity>? OnEntityRemoved;

	public EntityManager() {
		this._entities = new();
		this._entitiesByName = new();
		this._entitiesByPlayer = new();
	}

	public void AddEntity( Entity entity ) {
		if ( entity.Manager is not null ) {
			this.LogWarning( $"{entity} already has a manager!" );
			return;
		}
		if ( this._entities.TryAdd( entity.Guid, entity ) ) {
			this.LogLine( $"{entity} added!", Log.Level.NORMAL );
			entity.SetManager( this );
			this._entitiesByName.TryAdd( entity.Name, entity );
			if ( !this._entitiesByPlayer.TryGetValue( entity.Owner, out ConcurrentDictionary<Guid, Entity>? dict ) )
				this._entitiesByPlayer.TryAdd( entity.Owner, dict = new() );
			dict.TryAdd( entity.Guid, entity );
			OnEntityAdded?.Invoke( entity );
			entity.ComponentAdded += EntityComponentAdded;
			entity.ComponentRemoved += EntityComponentRemoved;
			entity.ComponentChanged += EntityComponentChanged;
		} else {
			this.LogWarning( $"Guid already taken! {entity.Guid}" );
		}
	}

	public void RemoveEntity( Entity entity ) {
		if ( this._entities.TryRemove( entity.Guid, out _ ) ) {
			this._entitiesByName.TryRemove( entity.Name, out _ );
			InternalRemoveEntity( entity );
		}
	}

	public void RemoveEntity( string entityName ) {
		if ( this._entitiesByName.Remove( entityName, out Entity? entity ) ) {
			this._entities.TryRemove( entity.Guid, out _ );
			InternalRemoveEntity( entity );
		}
	}

	public void RemoveEntity( Guid entityId ) {
		if ( this._entities.Remove( entityId, out Entity? entity ) ) {
			this._entitiesByName.TryRemove( entity.Name, out _ );
			InternalRemoveEntity( entity );
		}
	}

	private void InternalRemoveEntity( Entity entity ) {
		if ( this._entitiesByPlayer.TryGetValue( entity.Owner, out ConcurrentDictionary<Guid, Entity>? dict ) ) {
			dict.TryRemove( entity.Guid, out _ );
		}
		OnEntityRemoved?.Invoke( entity );
		entity.SetManager( null );
		entity.ComponentAdded -= EntityComponentAdded;
		entity.ComponentRemoved -= EntityComponentRemoved;
		entity.ComponentChanged -= EntityComponentChanged;
	}

	public void Update( float time, float deltaTime ) {
		foreach ( Entity entity in this._entities.Values ) {
			entity.Update( time, deltaTime );
			if ( !entity.Alive )
				RemoveEntity( entity );
		}
	}

	public IReadOnlyDictionary<Guid, Entity>? GetPlayerEntitiesOrDefault( ulong playerId ) => this._entitiesByPlayer.TryGetValue( playerId, out ConcurrentDictionary<Guid, Entity>? dict ) ? dict : null;

	private void EntityComponentAdded( Entity e, Component c ) => OnEntityComponentAdded?.Invoke( e, c );
	private void EntityComponentRemoved( Entity e, Component c ) => OnEntityComponentRemoved?.Invoke( e, c );
	private void EntityComponentChanged( Entity e, Component c ) => OnEntityComponentChanged?.Invoke( e, c );

	public byte[][] SerializeAll() {
		int i = 0;
		byte[][] data = new byte[ this.Entities.Count ][];
		foreach ( Entity e in this.Entities.Select( p => p.Value ) )
			data[ i++ ] = Entity.Serialize( e ) ?? Array.Empty<byte>();
		return data;
	}
	public void DeserializeAll( byte[][] data ) {
		for ( int i = 0; i < data.Length; i++ ) {
			if ( data[ i ].Length == 0 )
				continue;
			DeserializeEntity( data[ i ] );
		}
	}

	public Entity? DeserializeEntity( byte[] data ) {
		byte[][]? segments = Segmentation.Parse( data );
		if ( segments is null ) {
			Log.Error( "Corrupt data!" );
			return null;
		}

		if ( !Entity.DeserializeIdentification( segments[ 0 ], out string? name, out Guid? guid, out ulong owner ) )
			return null;
		Entity e = new( name, guid.Value, owner );

		if ( this._entities.ContainsKey( e.Guid ) )
			return null;
		if ( this._entitiesByName.ContainsKey( e.Name ) )
			return null;

		for ( int i = 1; i < segments.Length; i++ )
			DeserializeComponent( e, segments[ i ] );

		AddEntity( e );

		return e;
	}

	private void DeserializeComponent( Entity parent, byte[] data ) {
		if ( !ComponentSerializationUtilities.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( parent.Guid != parentGuid )
			return;
		try {
			object? newComponentObject = Activator.CreateInstance( componentType );
			if ( newComponentObject is Component newComponent ) {
				if ( newComponent is ISerializableComponent serializable )
					serializable.SetFromSerializedData( componentData );
				parent.AddComponent( newComponent );
			}
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
	}

	public void AddComponent( byte[] data ) {
		if ( !ComponentSerializationUtilities.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( !this._entities.TryGetValue( parentGuid.Value, out Entity? e ) )
			return;
		try {
			object? newComponentObject = Activator.CreateInstance( componentType );
			if ( newComponentObject is Component newComponent ) {
				if ( newComponent is ISerializableComponent serializable )
					serializable.SetFromSerializedData( componentData );
				e.AddComponent( newComponent );
			}
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
	}

	public void UpdateComponent( byte[] data ) {
		if ( !ComponentSerializationUtilities.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( !this._entities.TryGetValue( parentGuid.Value, out Entity? e ) )
			return;
		Component? c = e.GetComponent( componentType );
		if ( c is ISerializableComponent serializable )
			serializable.SetFromSerializedData( componentData );
	}

	public void RemoveComponent( Guid parentGuid, Type componentType ) {
		if ( !this._entities.TryGetValue( parentGuid, out Entity? e ) )
			return;
		e.RemoveComponent( componentType );
	}

	protected override bool OnDispose() {
		/*if ( SaveOnDispose )
			Save();
		*/
		return true;
	}
}
