using Engine;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Materials;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading;
using VoxDev.Voxels;

namespace VoxDevClient.Rendering.Voxels {
	public class VoxelWorldRenderer {


		private VoxelWorld world;

		private VoxelWorldRenderThreadManager threadManager;
		private Dictionary<Vector3i, VoxelChunkRenderManager> chunkRenderMngs;
		private HashSet<VoxelChunkRenderManager> renderManagerSet;
		private HashSet<VoxelChunkRenderManager> tRenderMngSet;
		private Scene<SceneObjectData3> scene;
		private Thread updateThread;
		private ConcurrentQueue<Vector3i> updateQueue;
		private HashSet<Vector3i> updates;
		private AutoResetEvent updateSignal;

		public bool wireframe;

		public ShaderBundle ShaderBundle { get; private set; }
		public PBRMaterial Material { get { return world.BlockSet.Material; } }
		public Camera3 Camera { get; private set; }

		public VoxelWorldRenderer( VoxelWorld world, Scene<SceneObjectData3> scene, Camera3 camera ) {
			this.world = world;
			world.BlockChange += BlockChange;
			this.scene = scene;
			this.Camera = camera;
			threadManager = new VoxelWorldRenderThreadManager( VoxelWorldRenderThreadManager.MAX_GEN_THREADS );
			chunkRenderMngs = new Dictionary<Vector3i, VoxelChunkRenderManager>();
			renderManagerSet = new HashSet<VoxelChunkRenderManager>();
			tRenderMngSet = new HashSet<VoxelChunkRenderManager>();
			ShaderBundle = new ShaderBundle( "Voxel Chunk Shaders",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<VoxelShader>() ),
				new ImmutableDuo<uint, Shader>( 1, Mem.Shaders.Get<VoxelDirectionalShadowShader>() )
			//////assdfsfgewsfdsfs need point light shader
			);
			updateQueue = new ConcurrentQueue<Vector3i>();
			updates = new HashSet<Vector3i>();
			updateSignal = new AutoResetEvent( false );
			updateThread = Mem.Threads.StartNew( UpdateFunc, "Voxel World Render Update" );
		}

		private void UpdateFunc() {
			while( Mem.Threads.Running ) {
				updateSignal.WaitOne( 1000 );
				if( !updateQueue.IsEmpty ) {
					updates.Clear();
					while( updateQueue.TryDequeue( out Vector3i xyz ) ) {
						updates.Add( xyz );
					}

					renderManagerSet.Clear();
					foreach( Vector3i xyz in updates ) {
						Vector3i chunkPosition = world.GetChunkPosition( xyz );
						if( chunkRenderMngs.TryGetValue( chunkPosition, out VoxelChunkRenderManager rMng ) ) {
							rMng.BlockChanged( xyz - chunkPosition * VoxelChunk.SIZE );
							renderManagerSet.Add( rMng );
						}
					}

					foreach( VoxelChunkRenderManager rMng in renderManagerSet ) {
						rMng.StartUpdate();
					}
				}
			}
		}

		private void BlockChange( Vector3i xyz ) {
			updateQueue.Enqueue( xyz );
		}

		public void Update( Vector3 cameraPosition, Vector3i renderRadius ) {
			Vector3i chunkPosition = world.GetChunkPosition( cameraPosition );
			BoundedUpdate( chunkPosition - renderRadius, chunkPosition + renderRadius );
		}

		public void BoundedUpdate( Vector3i lower, Vector3i upper ) {
			if( !world.Loaded ) {
				foreach( VoxelChunkRenderManager rMng in chunkRenderMngs.Values )
					rMng.Dispose( scene );
				chunkRenderMngs.Clear();
				Logging.Routine( "World not loaded anymore, disposing all meshes!" );
				return;
			}
			tRenderMngSet.Clear();
			foreach( VoxelChunkRenderManager rMng in chunkRenderMngs.Values )
				tRenderMngSet.Add( rMng );
			int rem = 4096;
			for( int x = Math.Min(lower.X,upper.X); x <= Math.Max( lower.X, upper.X ); x++ )
				for( int y = Math.Min( lower.Y, upper.Y ); y <= Math.Max( lower.Y, upper.Y ); y++ )
					for( int z = Math.Min( lower.Z, upper.Z ); z <= Math.Max( lower.Z, upper.Z ); z++ ) {
						Vector3i p = (x, y, z);
						if( chunkRenderMngs.TryGetValue( p, out VoxelChunkRenderManager rMng ) ) {
							rMng.Update( ref rem );
							tRenderMngSet.Remove( rMng );
						} else {
							double t = Clock64.Standard.Time;
							chunkRenderMngs.Add( p, CreateRender( world.GetChunk( p ) ) );
							double t2 = Clock64.Standard.Time;
							Logging.Routine( $"Adding chunk render for [{p}] took [{t2 - t}]s!" );
						}
					}

			foreach( VoxelChunkRenderManager rMng in tRenderMngSet ) {
				chunkRenderMngs.Remove( rMng.Chunk.Position );
				rMng.Dispose( scene );
			}
			tRenderMngSet.Clear();
			updateSignal.Set();
		}

		private VoxelChunkRenderManager CreateRender( VoxelChunk chunk ) {
			VoxelChunkRenderManager rMng = new VoxelChunkRenderManager( this, chunk, scene );
			threadManager.AddChunk( rMng );
			return rMng;
		}

	}
}
