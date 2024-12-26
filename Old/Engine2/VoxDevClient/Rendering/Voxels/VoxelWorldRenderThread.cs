using Engine;
using Engine.MemLib;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VoxDev.Voxels;

namespace VoxDevClient.Rendering.Voxels {
	public class VoxelWorldRenderThread {

		private volatile bool ready;
		public bool Ready { get => ready; }
		private Thread genThread;
		private AutoResetEvent newChunk;
		private VoxelChunkRenderManager chunk;
		private VoxelWorldRenderThreadManager mng;
		private byte[] data;

		public VoxelWorldRenderThread( VoxelWorldRenderThreadManager mng, string name ) {
			this.mng = mng;
			newChunk = new AutoResetEvent( false );
			data = new byte[ VoxelChunk.SIZE * VoxelChunk.SIZE * VoxelChunk.SIZE / 2 * VoxelChunkMesh.INSTANCESIZE ];
			ready = true;
			genThread = Mem.Threads.StartNew( MeshGenThread, name );
		}

		public bool SetTask( VoxelChunkRenderManager chunk ) {
			if( !ready )
				return false;
			ready = false;
			this.chunk = chunk;
			newChunk.Set();
			return true;
		}

		private void MeshGenThread() {
			while( Mem.Threads.Running && mng.Active ) {
				newChunk.WaitOne();
				if( chunk is null )
					continue;
				chunk.Chunk.GeneratedWaiter.WaitOne();
				Logging.Routine( $"Generating mesh for [{chunk.Chunk.Position}]!" );
				double time = Clock64.Standard.Time;
				chunk.GenerateAll();
				double time2 = Clock64.Standard.Time;
				chunk.StartUpdateThread();
				double time3 = Clock64.Standard.Time;
				Logging.Routine( $"Used [{Math.Round( ( time2 - time ) * 10000 ) / 10}]ms + [{Math.Round( ( time3 - time2 ) * 10000 ) / 10}]ms to generate mesh for [{chunk.Chunk.Position}]!" );
				chunk = null;
				ready = true;
				mng.ThreadReady( this );
			}
		}

	}
}
