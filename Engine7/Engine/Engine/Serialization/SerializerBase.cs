using Engine.Algorithms;
using Engine.Buffers;
using System.Runtime.InteropServices;

namespace Engine.Serialization;

public abstract class SerializerBase<TTarget> : ISerializer {

	public Guid Guid { get; }
	public Type Target { get; }

	protected SerializerBase( SerializerProvider serializerProvider ) {
		Guid = GetType().Resolve().Guid ?? throw new InvalidOperationException( "Serializer GUID is not set." );
		Target = typeof( TTarget );
		this.SerializerProvider = serializerProvider;
	}

	protected SerializerProvider SerializerProvider { get; }

	public int SerializeInto( ThreadedByteBuffer buffer, object t ) {
		if (t is not TTarget target)
			throw new InvalidOperationException( $"Invalid type {t.GetType()} for serialization using {this.GetType()}." );
		int bytesBefore = buffer.Count;
		PerformSerialization( buffer, target );
		int bytesAdded = buffer.Count - bytesBefore;
		Span<byte> guidBytes = stackalloc byte[ 16 ];
		MemoryMarshal.Write( guidBytes, Guid );
		buffer.AddRange( guidBytes );
		return bytesAdded + 16;
	}
	protected abstract void PerformSerialization( ThreadedByteBuffer buffer, TTarget t );

	public bool DeserializeInto( ReadOnlyMemory<byte> serializedData, object t ) => DeserializeInto( serializedData.Span, t );
	public bool DeserializeInto( ReadOnlySpan<byte> serializedData, object t ) {
		if (t is not TTarget target)
			throw new InvalidOperationException( $"Invalid type {t.GetType()} for deserialization using {this.GetType()}." );
		if (serializedData.Length < 16)
			throw new InvalidOperationException( $"Expected at least 16 bytes for GUID, but got {serializedData.Length}." );
		//Check if guid at end of data matches
		Guid receivedGuid = MemoryMarshal.Read<Guid>( serializedData[ ^16.. ] );
		if (receivedGuid != Guid)
			throw new InvalidOperationException( $"Received GUID {receivedGuid} does not match expected GUID {Guid}." );
		return PerformDeserialization( serializedData[ ..^16 ], target );
	}
	protected abstract bool PerformDeserialization( ReadOnlySpan<byte> serializedData, TTarget target );
}
