using System.Diagnostics.CodeAnalysis;
using Engine.Data;

namespace Engine.Modularity.ECS;

public abstract class Component : Identifiable, ISerializableComponent {
	private Entity? _parent;
	public Entity Parent => this._parent ?? throw new NullReferenceException( "Component does not belong to any entity." );
	public event Action<Component>? Changed;

	internal void SetEntity( Entity e ) {
		if ( this._parent is not null )
			this.LogWarning( "Can't set entity manually." );
		this._parent = e;
		ParentSet();
	}

	internal void Removed() => RemovedFromParent();
	protected void TriggerChanged() => Changed?.Invoke( this );
	protected virtual void ParentSet() { }
	protected virtual void RemovedFromParent() { }
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
	protected abstract byte[]? GetSerializedData();
	public static bool GetFromSerializedData( byte[] data,
		[NotNullWhen( true )] out Guid? parentGuid,
		[NotNullWhen( true )] out Guid? typeGuid,
		[NotNullWhen( true )] out byte[]? componentData ) {
		byte[][]? segments = Segmentation.Parse( data );
		parentGuid = null;
		typeGuid = null;
		componentData = null;
		if ( segments is null || segments.Length != 3 )
			return false;
		parentGuid = new Guid( segments[ 0 ] );
		typeGuid = new Guid( segments[ 1 ] );
		componentData = segments[ 2 ];
		return true;
	}
	//TODO: Better control of WHAT is set from the server, or clients. Ownership and tickIds should determine this.
	/*
	 * An owner can set the values whenever and should expect this to override any new message.
	 * If a tickId is lower than the current one, but we're not the owner, we should "rubber-band".
	 */
	public abstract void SetFromSerializedData( byte[] data );

}
