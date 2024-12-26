using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoxDev.VoxelRendering {
	public class VoxelWorldRenderThreadManager {

		public static readonly int MAX_GEN_THREADS = Environment.ProcessorCount / 2;

		public int ThreadCount { get; private set; }
		private readonly Queue<VoxelChunkRenderManager> queue;
		private readonly Queue<VoxelWorldRenderThread> readyThreads;
		private readonly VoxelWorldRenderThread[] genThreads;
		private readonly Thread taskMasterThread;
		private readonly AutoResetEvent newChunkToGen;
		private readonly AutoResetEvent readyThread;
		public bool Active { get; private set; }

		public VoxelWorldRenderThreadManager( int threads ) {
			ThreadCount = Math.Max( Math.Min( threads, MAX_GEN_THREADS ), 1 );
			Active = true;
			queue = new Queue<VoxelChunkRenderManager>();
			readyThreads = new Queue<VoxelWorldRenderThread>();
			newChunkToGen = new AutoResetEvent( false );
			readyThread = new AutoResetEvent( false );
			taskMasterThread = Mem.Threads.StartNew( TaskMaster, "World Mesh Gen Taskmaster" );
			genThreads = new VoxelWorldRenderThread[ ThreadCount ];
			for( int i = 0; i < ThreadCount; i++ ) {
				genThreads[ i ] = new VoxelWorldRenderThread( this, $"World Mesh Gen #[{i}]" );
				readyThreads.Enqueue( genThreads[ i ] );
			}
			readyThread.Set();
		}

		public void Destruct() {
			Active = false;
		}

		public void AddChunk( VoxelChunkRenderManager rmng ) {
			lock( queue )
				queue.Enqueue( rmng );
			newChunkToGen.Set();
		}

		internal void ThreadReady( VoxelWorldRenderThread thread ) {
			lock( readyThreads )
				readyThreads.Enqueue( thread );
			readyThread.Set();
		}

		private void TaskMaster() {
			while( Mem.Threads.Running && Active ) {
				newChunkToGen.WaitOne();
				int count = 0;
				lock( queue )
					count = queue.Count;
				while( count > 0 ) {

					int threadsReady = 0;
					lock( readyThreads )
						threadsReady = readyThreads.Count;

					if( threadsReady > 0 ) {
						VoxelWorldRenderThread thread = null;
						lock( readyThreads )
							thread = readyThreads.Dequeue();
						if( thread is null )
							continue;
						thread.SetTask( queue.Dequeue() );
					} else {
						readyThread.WaitOne();
					}

					lock( queue )
						count = queue.Count;
				}
			}
		}
	}
}
