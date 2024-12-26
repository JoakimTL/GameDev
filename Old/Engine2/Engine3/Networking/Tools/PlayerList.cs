using Engine.Entities;
using Engine.Networking.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Engine.Networking.Tools {
	/// <summary>
	/// Server and Client side
	/// </summary>
	public class PlayerList : IPacketListener {

		private PacketReader reader;
		private ConcurrentDictionary<string, Entity> players;

		public delegate void PlayerListChangeHandler( Entity p );
		private HashSet<PlayerListChangeHandler> newPlayerEvent;
		private HashSet<PlayerListChangeHandler> lostPlayerEvent;

		private static readonly Type[] packetListeningList = {
			typeof(PacketConnection),
			typeof(PacketDisconnection),
			typeof(PacketPlayerList)
		};
		public IReadOnlyCollection<Type> ListeningPacketTypes => packetListeningList;

		public PlayerList( PacketReader reader, Connection con ) {
			this.reader = reader;
			players = new ConcurrentDictionary<string, Entity>();
			reader.Add( this );
			newPlayerEvent = new HashSet<PlayerListChangeHandler>();
			lostPlayerEvent = new HashSet<PlayerListChangeHandler>();
			con.ConnectionClosedEvent += ConnectionLost;
		}

		public PlayerList( PacketReader reader ) {
			this.reader = reader;
			players = new ConcurrentDictionary<string, Entity>();
			reader.Add( this );
			newPlayerEvent = new HashSet<PlayerListChangeHandler>();
			lostPlayerEvent = new HashSet<PlayerListChangeHandler>();
		}

		private void ConnectionLost( Connection c ) {
			List<string> cons = new List<string>( players.Keys );
			for( int i = 0; i < cons.Count; i++ )
				players.TryRemove( cons[ i ], out Entity e );
		}

		public void AddNewPlayerListener( PlayerListChangeHandler e, bool catchup ) {
			newPlayerEvent.Add( e );
			if( catchup ) {
				foreach( var p in players.Values )
					e.Invoke( p );
			}
		}

		public void RemoveNewPlayerListener( PlayerListChangeHandler e ) {
			newPlayerEvent.Remove( e );
		}

		public void AddLostPlayerListener( PlayerListChangeHandler e ) {
			lostPlayerEvent.Add( e );
		}

		public void RemoveLostPlayerListener( PlayerListChangeHandler e ) {
			lostPlayerEvent.Remove( e );
		}

		public void HandlePacket( PacketMessage m ) {
			if( m.Packet.DataType == typeof( PacketConnection ) ) {
				PacketConnection p0 = m.Packet as PacketConnection;
				AddPlayer( p0.Username );
			} else if( m.Packet.DataType == typeof( PacketDisconnection ) ) {
				PacketDisconnection p0 = m.Packet as PacketDisconnection;
				RemovePlayer( p0.Username );
			} else if( m.Packet.DataType == typeof( PacketPlayerList ) ) {
				PacketPlayerList p0 = m.Packet as PacketPlayerList;
				for( int i = 0; i < p0.Usernames.Count; i++ ) {
					AddPlayer( p0.Usernames[ i ] );
				}
			}
		}

		public void PlayerConnected( Connection c ) {
			AddPlayer( c.Name );
		}

		public void PlayerDisconnected( Connection c ) {
			RemovePlayer( c.Name );
		}

		private void AddPlayer( string username ) {
			Entity e;
			if( players.TryAdd( username, e = new Entity( username ) ) ) {
				e.Add( new PingComponent( reader ) );
				foreach( var v in newPlayerEvent )
					v.Invoke( e );
			}
		}

		private void RemovePlayer( string username ) {
			if( players.TryRemove( username, out Entity e ) ) {
				foreach( var v in lostPlayerEvent )
					v?.Invoke( e );
			}
		}
	}
}
