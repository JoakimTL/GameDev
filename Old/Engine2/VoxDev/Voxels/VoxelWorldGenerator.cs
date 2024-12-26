using Engine;
using Engine.MemLib;
using Engine.Utilities.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public class VoxelWorldGenerator {

		public static readonly int MAX_GEN_THREADS = Environment.ProcessorCount / 2;

		public delegate void VoxelChunkGenerationHandler( VoxelChunk chunk );
		public VoxelChunkGenerationHandler GeneratorFunc { get; private set; }

		private BlockingCollection<VoxelChunk> chunkQueue;
		private readonly BlockingCollection<VoxelWorldGeneratorThread> readyThreads;
		private VoxelWorldGeneratorThread[] worldGenThreads;
		private Thread taskMasterThread;

		public VoxelWorldGenerator( VoxelChunkGenerationHandler generatorFunc, int numThreads ) {
			GeneratorFunc = generatorFunc;
			chunkQueue = new BlockingCollection<VoxelChunk>();
			worldGenThreads = new VoxelWorldGeneratorThread[ Math.Max( Math.Min( numThreads, MAX_GEN_THREADS ), 1 ) ];
			readyThreads = new BlockingCollection<VoxelWorldGeneratorThread>();
			for( int i = 0; i < worldGenThreads.Length; i++ ) {
				worldGenThreads[ i ] = new VoxelWorldGeneratorThread( this, i );
				readyThreads.Add( worldGenThreads[ i ] );
			}
			taskMasterThread = Mem.Threads.StartNew( TaskMaster, "Voxel World Gen Taskmaster" );
		}

		public void Enqueue( VoxelChunk chunk ) {
			chunkQueue.Add( chunk );
		}

		public void ThreadReady( VoxelWorldGeneratorThread thread ) {
			if( thread is null )
				return;
			readyThreads.Add( thread );
		}

		private void TaskMaster() {
			while( Mem.Threads.Running ) {
				if( chunkQueue.TryTake( out VoxelChunk chunk, 1000 ) ) {
					VoxelWorldGeneratorThread thread = null;
					thread = readyThreads.Take();
					thread.SetTask( chunk );
				}
			}
		}
	}
}
