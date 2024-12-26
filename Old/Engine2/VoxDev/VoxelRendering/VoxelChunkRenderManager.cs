using Engine;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using VoxDev.Voxels;

namespace VoxDev.VoxelRendering {
	public class VoxelChunkRenderManager {

		public VoxelWorldRenderer WorldRender { get; private set; }
		public VoxelChunk Chunk { get; private set; }
		private Scene<SceneObjectData3> scene;
		private VoxelChunkRenderable[] renderables;
		private ConcurrentQueue<Vector3i> changes;
		private AutoResetEvent updateSignal;
		private HashSet<Vector3i> updates;
		private Thread updateThread;
		private bool updated;
		public bool Disposed { get; private set; }

		public VoxelChunkRenderManager( VoxelWorldRenderer worldRender, VoxelChunk chunk, Scene<SceneObjectData3> scene ) {
			WorldRender = worldRender;
			Chunk = chunk;
			changes = new ConcurrentQueue<Vector3i>();
			this.scene = scene;
			renderables = new VoxelChunkRenderable[ 6 ];

			updateSignal = new AutoResetEvent( false );
			updates = new HashSet<Vector3i>();

			for( int i = 0; i < 6; i++ ) {
				renderables[ i ] = new VoxelChunkRenderable( this, i );
				scene.Add( renderables[ i ] );
			}

		}

		public void StartUpdateThread() {
			if( updateThread is null && !Disposed ) {
				updateThread = Mem.Threads.StartNew( ChangeHandlingThread, $"Chunk Change Handler [{Chunk.Position}]" );
				for( int i = 0; i < 6; i++ ) {
					if( renderables is null )
						break;
					VoxelChunkRenderable r = renderables[ i ];
					if( r is null )
						break;
					r.InitiateThread();
				}
			}
		}

		public void StartUpdate() {
			updated = true;
			updateSignal.Set();
		}

		private void ChangeHandlingThread() {
			while( Mem.Threads.Running && !Disposed ) {
				updateSignal.WaitOne( 1000 );
				if( Disposed )
					return;
				if( updated ) {
					updated = false;
					updates.Clear();
					while( changes.TryDequeue( out Vector3i xyz ) ) {
						updates.Add( xyz );
					}
					foreach( Vector3i xyz in updates ) {
						int index = Chunk.GetIndex( xyz.X, xyz.Y, xyz.Z );
						for( int i = 0; i < 6; i++ ) {
							if( renderables is null )
								break;
							VoxelChunkRenderable r = renderables[ i ];
							if( r is null )
								break;
							r.UpdateData( xyz, index );
						}
					}
					for( int i = 0; i < 6; i++ ) {
						if( renderables is null )
							break;
						VoxelChunkRenderable r = renderables[ i ];
						if( r is null )
							break;
						r.SignalGeneration();
					}
				}
			}
		}

		public void BlockChanged( Vector3i xyz ) {
			changes.Enqueue( xyz );
		}

		public void GenerateAll() {
			double t = Clock64.Standard.Time;
			Chunk.GeneratedWaiter.WaitOne();
			double t2 = Clock64.Standard.Time;
			if(t2-t > 0.001) {
				Logging.Routine( $"[{Chunk.Position}] Waited [{Math.Round( ( Clock64.Standard.Time - t ) * 10000 ) / 10}]ms for chunk data to generate!" );
			}

			for( int i = 0; i < 6; i++ ) {
				if( !Disposed )
					renderables[ i ].GenerateMesh();
			}

			if( Disposed )
				return;
		}

		internal void Dispose( Scene<SceneObjectData3> scene ) {
			Disposed = true;
			for( int i = 0; i < 6; i++ ) {
				scene.Remove( renderables[ i ] );
				renderables[ i ].Dispose();
			}
			renderables = null;
		}

		internal void Update( ref int remaining ) {
			Chunk.Pulse();
			for( int i = 0; i < 6; i++ )
				renderables[ i ].Update( ref remaining );
		}
	}
}
