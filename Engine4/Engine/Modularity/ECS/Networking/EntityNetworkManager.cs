using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Engine.Modularity.ECS.Networking.Packets;
using Engine.Modularity.Modules;
using Engine.Networking;
using Engine.Structure;

namespace Engine.Modularity.ECS.Networking;
public class EntityNetworkManager : ModuleSingletonBase, IPacketListener, IUpdateable {
	private readonly NetworkManager _networkManager;
	private readonly EntityManager _entityManager;
	private readonly HashSet<Component> _newComponents;
	private readonly HashSet<Component> _changedComponents;
	private readonly HashSet<Component> _removedComponents;
	private ulong _changeIndex;

	public bool Active => this._networkManager.Open;

	//TODO: implement changeIndex

	public EntityNetworkManager( NetworkManager networkManager, EntityManager entityManager ) {
		this._networkManager = networkManager;
		this._entityManager = entityManager;
		this._entityManager.OnEntityAdded += EntityAdded;
		this._entityManager.OnEntityRemoved += EntityRemoved;
		this._entityManager.OnEntityComponentAdded += ComponentAdded;
		this._entityManager.OnEntityComponentRemoved += ComponentRemoved;
		this._entityManager.OnEntityComponentChanged += ComponentChanged;
		this._networkManager.AddListener( this );
		this._newComponents = new HashSet<Component>();
		this._changedComponents = new HashSet<Component>();
		this._removedComponents = new HashSet<Component>();
		if ( !this._networkManager.IsServer )
			this._networkManager.Send( new RequestEntities( Array.Empty<byte>() ) );
	}

	//An entity was added to the entity manager. This should be broadcasted if we're on the server!
	private void EntityAdded( Entity e ) {
		if ( !this._networkManager.IsServer )
			return;
		byte[]? data = Entity.Serialize( e );
		if ( data is null )
			return;
		this._networkManager.Send( new EntityAdded( e ) );
	}

	//An entity was removed from the entity manager. This should be broadcasted if we're on the server!
	private void EntityRemoved( Entity e ) {
		if ( !this._networkManager.IsServer )
			return;
		this._networkManager.Send( new EntityRemoved( e.Name ) );
	}

	//An entity component was added to an entity. If we're on the client and the network !
	private void ComponentAdded( Entity e, Component c ) {
		if ( !ShouldComponentBroadcast( e.Owner, c.GetType(), this._networkManager.IsServer ) )
			return;
		this._newComponents.Add( c );
	}

	private void ComponentRemoved( Entity e, Component c ) {
		if ( !ShouldComponentBroadcast( e.Owner, c.GetType(), this._networkManager.IsServer ) )
			return;
		this._removedComponents.Add( c );
	}

	private void ComponentChanged( Entity e, Component c ) {
		if ( !ShouldComponentBroadcast( e.Owner, c.GetType(), this._networkManager.IsServer ) )
			return;
		this._changedComponents.Add( c );
	}

	public bool ShouldComponentBroadcast( ulong ownerId, Type componentType, bool serverSide ) {
		NetworkAttribute? networkAttribute = componentType.GetCustomAttribute<NetworkAttribute>();
		if ( networkAttribute is null ) {
#if DEBUG
			this.LogWarning( "Missing network attribute!" );
#endif
			return false;
		}
		if ( networkAttribute.Broadcast == ComponentBroadcastSide.CLIENT && serverSide )
			return false;
		if ( networkAttribute.Broadcast == ComponentBroadcastSide.SERVER && !serverSide )
			return false;
		if ( !this._networkManager.IsServer && ownerId != this._networkManager.ServerProvidedId )
			return false;
		return true;
	}

	protected override bool OnDispose() => true;

	public bool Listening( Type packetType ) => true;

	public bool ListeningTcp( IPEndPoint endpoint ) {
		return true;
		/*if ( _networkManager.IsServer )
			return true;
		return this._networkManager.ServerConnection?.TcpEndPoint == endpoint;*/
	}
	public bool ListeningUdp( IPEndPoint endpoint ) => true;

	public void NewPacket( Packet packet, Type packetType ) {
		{
			if ( packet is ComponentAdded compAdded ) {
				if ( compAdded.SerializedData is null ) {
					this.LogWarning( "ComponentAdded packet contained no serialized data." );
					return;
				}
				this._entityManager.AddComponent( compAdded.SerializedData );
				return;
			}
		}
		{
			if ( packet is ComponentChangedTcp compChanged ) {
				if ( compChanged.SerializedData is null ) {
					this.LogWarning( "ComponentChanged packet contained no serialized data." );
					return;
				}
				this._entityManager.UpdateComponent( compChanged.SerializedData );
				return;
			}
		}
		{
			if ( packet is ComponentChangedUdp compChanged ) {
				if ( compChanged.SerializedData is null ) {
					this.LogWarning( "ComponentChanged packet contained no serialized data." );
					return;
				}
				this._entityManager.UpdateComponent( compChanged.SerializedData );
				return;
			}
		}
		{
			if ( packet is ComponentRemoved compRemoved ) {
				if ( compRemoved.ComponentType is null ) {
					this.LogWarning( "ComponentRemoved packet contained no type data." );
					return;
				}
				this._entityManager.RemoveComponent( compRemoved.EntityGuid, compRemoved.ComponentType );
				return;
			}
		}
		if ( this._networkManager.IsServer ) {
			//TODO: Discard packets the opposite part has no ownership over.
			//Server will only accept component change packets that clients are known to own.
			if ( packet is RequestEntities entityRequest ) {
				this.LogLine( "New connection, sending all entity data!", Log.Level.NORMAL );
				this._networkManager.Send( new AllEntities( this._entityManager.SerializeAll() ) { RemoteTarget = entityRequest.RemoteSender } );
			}
		} else {
			//Clients will only accept packets that servers are known to own.
			if ( packet is EntityAdded entityAdded ) {
				if ( entityAdded.SerializedData is null ) {
					this.LogWarning( "EntityAdded packet contained no serialized data." );
					return;
				}
				Entity? e = this._entityManager.DeserializeEntity( entityAdded.SerializedData );
				if ( e is null )
					return;
				this._entityManager.AddEntity( e );
				return;
			}
			if ( packet is EntityRemoved entityRemoved ) {
				if ( entityRemoved.EntityName is null ) {
					this.LogWarning( "EntityRemoved packet contained no name for entity." );
					return;
				}
				this._entityManager.RemoveEntity( entityRemoved.EntityName );
				return;
			}
			if ( packet is AllEntities allEntities )
				this._entityManager.DeserializeAll( allEntities.SerializedData );
		}
	}

	public void Update( float time, float deltaTime ) {
		if ( this._newComponents.Count == 0 && this._changedComponents.Count == 0 && this._removedComponents.Count == 0 )
			return;
		this._changeIndex++;
		//TODO: Add change index to packet
		foreach ( Component? component in this._newComponents )
			this._networkManager.Send( new ComponentAdded( component ) );
		foreach ( Component? component in this._changedComponents ) {
			NetworkAttribute? networkAttribute = component.GetType().GetCustomAttribute<NetworkAttribute>();
			if ( networkAttribute is null )
				continue;
			if ( networkAttribute.UpdateProtocol == ProtocolType.Udp )
				this._networkManager.Send( new ComponentChangedUdp( component ) );
			else
				this._networkManager.Send( new ComponentChangedTcp( component ) );
		}
		foreach ( Component? component in this._removedComponents )
			this._networkManager.Send( new ComponentRemoved( component ) );
		this._newComponents.Clear();
		this._changedComponents.Clear();
		this._removedComponents.Clear();
	}
}
