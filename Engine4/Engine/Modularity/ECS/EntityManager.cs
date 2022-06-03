using Engine.Data;
using Engine.Modularity.Modules;

namespace Engine.Modularity.ECS;
public class EntityManager : ModuleSingletonBase, IUpdateable {

	public bool Active => true;
	public bool SaveOnDispose { get; set; }
	private readonly Dictionary<Guid, Entity> _entities;
	private readonly Dictionary<string, Entity> _entitiesByName;
	public IReadOnlyCollection<Entity> Entities => this._entities.Values;
	public event Action<Entity, Component>? OnEntityComponentAdded;
	public event Action<Entity, Component>? OnEntityComponentRemoved;
	public event Action<Entity, Component>? OnEntityComponentChanged;
	public event Action<Entity>? OnEntityAdded;
	public event Action<Entity>? OnEntityRemoved;

	public EntityManager() {
		this._entities = new Dictionary<Guid, Entity>();
		this._entitiesByName = new Dictionary<string, Entity>();
	}

	public void AddEntity( Entity entity ) {
		if ( entity.Manager is not null ) {
			this.LogWarning( $"{entity} alread has a manager!" );
			return;
		}
		lock ( this._entities )
			if ( this._entities.TryAdd( entity.Guid, entity ) ) {
				entity.SetManager( this );
				this._entitiesByName.TryAdd( entity.Name, entity );
				OnEntityAdded?.Invoke( entity );
				entity.ComponentAdded += EntityComponentAdded;
				entity.ComponentRemoved += EntityComponentRemoved;
				entity.ComponentChanged += EntityComponentChanged;
			}
	}

	public void RemoveEntity( Entity entity ) {
		lock ( this._entities )
			if ( this._entities.Remove( entity.Guid ) ) {
				this._entitiesByName.Remove( entity.Name );
				InternalRemoveEntity( entity );
			}
	}

	public void RemoveEntity( string entityName ) {
		lock ( this._entities )
			if ( this._entitiesByName.Remove( entityName, out Entity? entity ) ) {
				this._entities.Remove( entity.Guid );
				InternalRemoveEntity( entity );
			}
	}

	public void RemoveEntity( Guid entityId ) {
		lock ( this._entities )
			if ( this._entities.Remove( entityId, out Entity? entity ) ) {
				this._entitiesByName.Remove( entity.Name );
				InternalRemoveEntity( entity );
			}
	}

	private void InternalRemoveEntity( Entity entity ) {
		OnEntityRemoved?.Invoke( entity );
		entity.SetManager( null );
		entity.ComponentAdded -= EntityComponentAdded;
		entity.ComponentRemoved -= EntityComponentRemoved;
		entity.ComponentChanged -= EntityComponentChanged;
	}

	public void Update( float time, float deltaTime ) {
		foreach ( Entity entity in this._entities.Values ) {
			entity.Update( time, deltaTime );
		}
	}

	private void EntityComponentAdded( Entity e, Component c ) => OnEntityComponentAdded?.Invoke( e, c );
	private void EntityComponentRemoved( Entity e, Component c ) => OnEntityComponentRemoved?.Invoke( e, c );
	private void EntityComponentChanged( Entity e, Component c ) => OnEntityComponentChanged?.Invoke( e, c );

	public byte[][] SerializeAll() {
		int i = 0;
		byte[][] data = new byte[ this.Entities.Count ][];
		foreach ( Entity e in this.Entities )
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
		if ( !Component.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( parent.Guid != parentGuid )
			return;
		try {
			object? newComponentObject = Activator.CreateInstance( componentType );
			if ( newComponentObject is Component newComponent ) {
				newComponent.SetFromSerializedData( componentData );
				parent.AddComponent( newComponent );
			}
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
	}

	public void AddComponent( byte[] data ) {
		if ( !Component.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( !this._entities.TryGetValue( parentGuid.Value, out Entity? e ) )
			return;
		try {
			object? newComponentObject = Activator.CreateInstance( componentType );
			if ( newComponentObject is Component newComponent ) {
				newComponent.SetFromSerializedData( componentData );
				e.AddComponent( newComponent );
			}
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
	}

	public void UpdateComponent( byte[] data ) {
		if ( !Component.GetFromSerializedData( data, out Guid? parentGuid, out Guid? typeGuid, out byte[]? componentData ) )
			return;

		Type? componentType = IdentificationRegistry.Get( typeGuid.Value );
		if ( componentType is null )
			return;
		if ( !this._entities.TryGetValue( parentGuid.Value, out Entity? e ) )
			return;
		e.GetComponent( componentType )?.SetFromSerializedData( componentData );
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
