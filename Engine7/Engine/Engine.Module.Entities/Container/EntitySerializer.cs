using Engine.Algorithms;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Engine.Module.Entities.Container;

[Guid( "F78140DD-DC4D-46A5-BA8B-BA57F0570813" )]
public sealed class EntitySerializer( SerializerProvider serializerProvider ) : SerializerBase<Entity>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, Entity entity ) {
		const string ecsBufferName = "ecs-comp";
		if (buffer.Identity == ecsBufferName)
			throw new InvalidOperationException( $"Buffer name cannot be '{ecsBufferName}'." );

		Span<byte> initialBuffer = stackalloc byte[ 32 ];
		MemoryMarshal.Write( initialBuffer, entity.EntityId );
		MemoryMarshal.Write( initialBuffer[ 16.. ], entity.ParentId ?? Guid.Empty );
		Segmenter segmenter = new( initialBuffer );
		foreach (KeyValuePair<Type, ComponentBase> kvp in entity.ComponentsWithKeys) {
			ISerializer? serializer = SerializerProvider.GetSerializerFor( kvp.Key );
			if (serializer is null)
				continue;
			ThreadedByteBuffer ecsBuffer = ThreadedByteBuffer.GetBuffer( ecsBufferName );
			serializer.SerializeInto( ecsBuffer, kvp.Value );
			using (PooledBufferData data = ecsBuffer.GetData( tag: "ecs-segment"))
				segmenter.Append( data.Payload.Span );
		}
		using (PooledBufferData output = segmenter.Flush(name: "ecs-entity" )) 
			buffer.AddRange( output.Payload.Span );
	}

	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, Entity target ) {
		Desegmenter desegmenter = new();
		Span<byte> output = stackalloc byte[ serializedData.DetermineSpanLength() ];
		bool hasComponents = desegmenter.ReadInto( serializedData, output, out int readBytes );
		Guid entityId = MemoryMarshal.Read<Guid>( output[ ..16 ] );
		if (target.EntityId != entityId)
			throw new Exception( "Entity ID does not match." );
		Guid parentId = MemoryMarshal.Read<Guid>( output[ 16.. ] );
		if ((target.ParentId ?? Guid.Empty) != parentId)
			throw new Exception( "Parent ID does not match." );
		if (!hasComponents)
			return;
		while (desegmenter.ReadInto( serializedData, output, out readBytes )) {
			ISerializer? serializer = SerializerProvider.GetSerializerFor( MemoryMarshal.Read<Guid>( output[ (readBytes - 16)..readBytes ] ) );
			if (serializer is null)
				continue;
			ComponentBase component = target.CreateComponent( serializer.Target );
			serializer.DeserializeInto( output[ ..readBytes ], component );
			target.AddComponent( serializer.Target, component );
		}
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) {
		Desegmenter desegmenter = new();
		Span<byte> output = stackalloc byte[ serializedData.DetermineSpanLength() ];
		bool hasComponents = desegmenter.ReadInto( serializedData, output, out int readBytes );
		if (!hasComponents)
			return true;
		while (desegmenter.ReadInto( serializedData, output, out readBytes )) {
			ISerializer? serializer = SerializerProvider.GetSerializerFor( MemoryMarshal.Read<Guid>( output[ (readBytes - 16)..readBytes ] ) );
			if (serializer is null)
				return false;
			if (!serializer.CanDeserialize( output[ ..readBytes ] ))
				return false;
		}
		return true;
	}

	///// <summary>
	///// Returns only complete components from the serialized data.
	///// </summary>
	//public void DeserializeComponents( ReadOnlySpan<byte> serializedData, Entity target ) {
	//	Desegmenter desegmenter = new();
	//	Span<byte> output = stackalloc byte[ serializedData.DetermineSpanLength() ];
	//	bool hasComponents = desegmenter.ReadInto( serializedData, output, out int readBytes );
	//	if (target.EntityId != MemoryMarshal.Read<Guid>( output[ ..8 ] ))
	//		throw new Exception( "Entity ID does not match." );
	//	if (target.ParentId != MemoryMarshal.Read<Guid>( output[ 8.. ] ))
	//		throw new Exception( "Parent ID does not match." );
	//	if (!hasComponents)
	//		return true;
	//	while (desegmenter.ReadInto( serializedData, output, out readBytes )) {
	//		ISerializer? serializer = SerializerProvider.GetSerializerFor( MemoryMarshal.Read<Guid>( output[ ^16.. ] ) );
	//		if (serializer is null)
	//			continue;
	//		Span<byte> componentData = output[ ..^16 ];
	//		var component = target.AddComponent( serializer.Target );
	//		serializer.DeserializeInto( componentData, component );
	//	}
	//	return true;
	//}
}
