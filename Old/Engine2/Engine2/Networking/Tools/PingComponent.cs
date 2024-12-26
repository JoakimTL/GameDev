//using Engine.Entities;
using Engine.Networking.Packets;
using System.Collections.Generic;

namespace Engine.Networking.Tools {
	/*public class PingComponent : EntityModule, IPacketListener {
		public float PingTCPms { get; private set; }
		public float PingUDPms { get; private set; }

		public PingComponent( Entity e, PacketReader reader ) : base( e ) {
			PingTCPms = -1;
			PingUDPms = -1;
			reader.Add( this );
		}

		public void SetPing( float msTCP, float msUDP ) {
			PingTCPms = msTCP;
			PingUDPms = msUDP;
			InvokeDataChanged();
		}
		
		public void SetTCPPing( float ms ) {
			PingTCPms = ms;
			InvokeDataChanged();
		}

		public void SetUDPPing( float ms ) {
			PingUDPms = ms;
			InvokeDataChanged();
		}

		public bool IsListening( uint iD ) {
			return iD == PacketType.PINGTIMES.ID;
		}

		public void HandlePacket( Packet p ) {
			if( p.ID == PacketType.PINGTIMES.ID ) {
				PacketPingTimes p0 = p as PacketPingTimes;
				if (p0.Timings.TryGetValue( Container.Name, out ConnectionTiming timing ) )
					SetPing( timing.TCP, timing.UDP );
			}
		}

		public override void FillTransferData( List<byte> data ) {
		}
	}*/
}
