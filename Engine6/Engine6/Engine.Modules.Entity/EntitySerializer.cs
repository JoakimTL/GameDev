using Engine.Data;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Modules.ECS;

public class EntitySerializer {

	public const int MaxComponentSerializationSizeBytes = ushort.MaxValue + 1;

	public EntityManager EntityManager { get; }

	public EntitySerializer( EntityManager entityManager ) {
		this.EntityManager = entityManager;
	}

	public bool Serialize( SerializableComponentBase component, UnmanagedList data ) {
		Guid? guid = SerializableComponentTypeHelper.GetComponentTypeId( component.GetType() );
		if (!guid.HasValue) {
			this.LogWarning( $"Failed to get guid for component of type {component.GetType().Name}." );
			return false;
		}
		Span<byte> serializedData = stackalloc byte[ MaxComponentSerializationSizeBytes ];
		int len = (int) component.Serialize( serializedData );
		data.Add( guid.Value );
		data.Add( component.Entity.EntityId );
		data.Add( (ushort) len );
		data.AddRange( serializedData[ ..len ] );
		return true;
	}

	public void SerializeEntityWithoutComponents( Entity e, UnmanagedList data ) {
		data.Add( e.EntityId );
		data.Add( e.OwnerId );
		data.Add( e.Parent?.EntityId ?? e.EntityId );
	}

	private unsafe bool TryDeserializeComponentInternal( byte* dataPtr, uint dataLength, ref uint index, [NotNullWhen( true )] out SerializableComponentBase? component, [NotNullWhen( true )] out Guid? entityId ) {
		const uint componentHeaderSize = 34; //sizeof(Guid) * 2 + sizeof(ushort)
		component = null;
		entityId = null;
		if (dataLength < componentHeaderSize) {
			this.LogWarning( $"Component data is too small to deserialize. Expected at least {componentHeaderSize} bytes, but got {dataLength} bytes." );
			return false;
		}
		Guid typeGuid = *(Guid*) (dataPtr + index);
		index += (uint) sizeof( Guid );

		entityId = *(Guid*) (dataPtr + index);
		index += (uint) sizeof( Guid );

		ushort size = *(ushort*) (dataPtr + index);
		index += sizeof( ushort );

		Entity? entity = this.EntityManager.Get( entityId.Value );
		if (entity is null) {
			this.LogWarning( $"Failed to find entity with id {entityId}." );
			return false;
		}

		component = entity.GetOrCreateComponent( typeGuid );

		if (component is null) {
			this.LogWarning( $"Failed to create component." );
			return false;
		}

		if (size > index)
			component.Deserialize( new Span<byte>( dataPtr + index, (ushort) (size - index) ) );
		index += size;

		return true;
	}

	public bool TryDeserializeComponent( ReadOnlySpan<byte> data, [NotNullWhen( true )] out SerializableComponentBase? component, [NotNullWhen( true )] out Guid? entityId ) {
		unsafe {
			fixed (byte* ptr = data) {
				uint index = 0;
				return TryDeserializeComponentInternal( ptr, (uint) data.Length, ref index, out component, out entityId );
			}
		}
	}

	public byte[] Serialize( IEnumerable<Entity> entities, out uint entitiesSerialized, out uint componentsSerialized ) {
		Entity[] entityArray = entities.ToArray();
		SerializableComponentBase[] components = entityArray.SelectMany( p => p.Components ).OfType<SerializableComponentBase>().ToArray();
		entitiesSerialized = (uint) entityArray.Length;
		componentsSerialized = 0;

		using (UnmanagedList data = new()) {
			data.Add( (uint) entityArray.Length );
			for (int i = 0; i < entityArray.Length; i++)
				SerializeEntityWithoutComponents( entityArray[ i ], data );
			uint position = (uint) data.BytesUsed;
			data.Add( (uint) 0 );
			for (int i = 0; i < components.Length; i++)
				if (Serialize( components[ i ], data ))
					componentsSerialized++;
			data.Set( componentsSerialized, position );
			return data.ToArray<byte>(0, data.BytesUsed);
		}
	}

	public IEnumerable<Entity> Deserialize( byte[] data ) {
		unsafe {
			fixed (byte* ptr = data) {
				uint index = 0;
				uint entityCount = *(uint*) ptr;
				index += sizeof( uint );
				List<EntityData> entities = new();
				for (int i = 0; i < entityCount; i++) {
					entities.Add( *(EntityData*) (ptr + index) );
					index += (uint) sizeof( EntityData );
				}
				this.EntityManager.CreateEntities( entities );
				uint componentCount = *(uint*) ptr;
				index += sizeof( uint );

				for (int i = 0; i < componentCount; i++)
					TryDeserializeComponentInternal( ptr, (uint) data.Length, ref index, out _, out _ );
				return entities.Select( p => this.EntityManager.Get( p.EntityId )! ).ToArray();
			}
		}
	}
}
