using Engine.Entities;
using Engine.Networking.Packets;
using System;
using System.Collections.Generic;

namespace Engine.Networking.Tools {
	public class PingComponent : Module, IPacketListener {
		public float PingTCPms { get; private set; }
		public float PingUDPms { get; private set; }

		private static readonly Type[] packetListeningList = {
			typeof(PacketPingTimes)
		};
		public IReadOnlyCollection<Type> ListeningPacketTypes => packetListeningList;

		public PingComponent( PacketReader reader ) {
			PingTCPms = -1;
			PingUDPms = -1;
			reader.Add( this );
		}

		protected override void Initialize() {

		}

		public override void Update( float time, float deltaTime ) {

		}

		public void SetPing( float msTCP, float msUDP ) {
			PingTCPms = msTCP;
			PingUDPms = msUDP;
		}

		public void HandlePacket( PacketMessage m ) {
			if( m.Packet.DataType == typeof( PacketPingTimes ) ) {
				PacketPingTimes p0 = m.Packet as PacketPingTimes;
				if( p0.Timings.TryGetValue( Entity.Name, out ConnectionTiming timing ) )
					SetPing( timing.TCP, timing.UDP );
			}
		}
	}
}
