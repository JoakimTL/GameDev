namespace Engine.Modules.Entity;

public abstract class SerializableComponentBase : ComponentBase {

	public delegate void ComponentSerializationEventHandler( SerializableComponentBase component );

	event ComponentSerializationEventHandler? ComponentSerialized;
	/// <summary>
	/// Happens when an existing component has been changed from deserialization.
	/// </summary>
	event ComponentSerializationEventHandler? ComponentDeserialized;
	
	public uint Serialize( Span<byte> data ) {
		if ( InternalSerialize( data, out uint writtenBytes ) )
			ComponentSerialized?.Invoke( this );
		return writtenBytes;
	}
	public void Deserialize( ReadOnlySpan<byte> data ) {
		if ( InternalDeserialize( data ) )
			ComponentDeserialized?.Invoke( this );
	}

	/// <param name="data">The data span length is at most <see cref="EntitySerializer.MaxComponentSerializationSizeBytes"/>!</param>
	/// <param name="writtenBytes">NMumber of bytes written to</param>
	/// <returns>True if serialization succeeded</returns>
	protected abstract bool InternalSerialize( Span<byte> data, out uint writtenBytes );

	/// <returns>True if deserialization succeeded</returns>
	protected abstract bool InternalDeserialize( ReadOnlySpan<byte> data );
}
