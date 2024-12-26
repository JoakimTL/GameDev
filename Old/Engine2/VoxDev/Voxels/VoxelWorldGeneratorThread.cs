using Engine;
using Engine.MemLib;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public class VoxelWorldGeneratorThread {

		private VoxelWorldGenerator worldGenerator;
		private Thread worldGenThread;
		private AutoResetEvent newChunk;
		private VoxelChunk chunk;
		private bool ready;

		public VoxelWorldGeneratorThread( VoxelWorldGenerator worldGenerator, int i ) {
			this.worldGenerator = worldGenerator;

			ready = true;
			newChunk = new AutoResetEvent( false );

			worldGenThread = Mem.Threads.StartNew( WorldGen, $"Voxel Worldgen Thread[{i}]" );
		}

		public bool SetTask( VoxelChunk chunk ) {
			if( !ready )
				return false;
			ready = false;
			this.chunk = chunk;
			newChunk.Set();
			return true;
		}

		private void WorldGen() {
			while( Mem.Threads.Running ) {
				newChunk.WaitOne();
				if( !( chunk is null ) ) {
					bool loaded = true;
					double time = Clock64.Standard.Time;
					chunk.CreateData();
					//LOOK FOR FILE, IF NO FILE EXISTS USE STANDARD CHUNK GEN
					//WAIT FOR FILE IF IT IS BEING WRITTEN TO
					if( !chunk.AttemptLoad() ) {
						loaded = false;
						worldGenerator.GeneratorFunc.Invoke( chunk );
					}
					double time2 = Clock64.Standard.Time;
					if( loaded ) {
						Logging.Routine( $"Loaded terrain for [{chunk.Position}] in [{Math.Round( ( time2 - time ) * 10000 ) / 10}]ms!" );
					} else {
						Logging.Routine( $"Generated terrain for [{chunk.Position}] in [{Math.Round( ( time2 - time ) * 10000 ) / 10}]ms!" );
					}
					chunk.SetGenerated();
				}
				ready = true;
				worldGenerator.ThreadReady( this );
			}
		}
	}
}
