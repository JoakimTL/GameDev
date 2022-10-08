using Engine.Data;

namespace Engine.Modularity.ECS;

public abstract class SerializableComponent : Component, ISerializableComponent {
	public byte[]? Serialize() {
		Guid? guid = IdentificationRegistry.Get( GetType() );
		if ( !guid.HasValue )
			return null;
		byte[]? data = null;
		try {
			data = GetSerializedData();
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		if ( data is null )
			return null;
		return Segmentation.Segment( this.Parent.Guid.ToByteArray(), guid.Value.ToByteArray(), data );
	}
	protected virtual byte[]? GetSerializedData() => Array.Empty<byte>();
	//TODO: Better control of WHAT is set from the server, or clients. Ownership and tickIds should determine this.
	/*
	 * An owner can set the values whenever and should expect this to override any new message.
	 * If a tickId is lower than the current one, but we're not the owner, we should "rubber-band".
	 */
	public virtual void SetFromSerializedData( byte[] data ) { }

}
