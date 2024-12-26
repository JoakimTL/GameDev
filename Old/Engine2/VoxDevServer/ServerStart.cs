using Engine.QuickstartKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevServer {
	public class ServerStart : QuickstartServer {

		private VoxDev.Voxels.VoxelWorld world;

		public ServerStart( int port ) : base( port ) {

		}

		public override void Entry() {
			while( Running ) {

			}
		}
	}
}
