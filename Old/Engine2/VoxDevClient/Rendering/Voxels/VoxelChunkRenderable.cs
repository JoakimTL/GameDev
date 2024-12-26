using Engine;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Physics.D3.Shapes;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using VoxDev.Voxels;

namespace VoxDevClient.Rendering.Voxels {
	public class VoxelChunkRenderable : Renderable3 {

		public static readonly Shape3Polygon.Mold CollisionMold = new Shape3Polygon.Mold(
			new List<Vector3>() {
				new Vector3( 0, 0, 0 ), new Vector3( VoxelChunk.SIZE, 0, 0 ), new Vector3( VoxelChunk.SIZE, VoxelChunk.SIZE, 0 ), new Vector3( 0, VoxelChunk.SIZE, 0 ),
				new Vector3( 0, 0, VoxelChunk.SIZE ), new Vector3( VoxelChunk.SIZE, 0, VoxelChunk.SIZE ), new Vector3( VoxelChunk.SIZE, VoxelChunk.SIZE, VoxelChunk.SIZE ), new Vector3( 0, VoxelChunk.SIZE, VoxelChunk.SIZE )
			}
		);

		private VoxelChunkRenderManager mng;
		private VoxelChunkMesh.Direction direction;
		public VoxelChunkMesh VoxelMesh { get; private set; }

		private ConcurrentDictionary<int, int> tileMapping;
		private ConcurrentDictionary<int, VoxFace> tileData;
		private List<VoxFace> tempGenData;
		private ConcurrentQueue<VoxFace>[] genQueues;
		private Queue<VoxFace> genData;
		private ConcurrentQueue<int> clearQueue;
		private Queue<int> failedTiles;
		private bool[] updateRequired;
		private bool[] updatedThisFrame;

		private volatile int genCount;
		private AutoResetEvent generateSignal;
		private ManualResetEvent notGenerating;
		private Thread updateGenThread;

		private Vector3i dir;
		public VoxelChunkRenderable( VoxelChunkRenderManager mng, int i ) {
			Warn = false;
			this.mng = mng;
			direction = (VoxelChunkMesh.Direction) i;
			dir = VoxelChunkMesh.DirectionVectors[ (int) direction ];
			ShaderBundle = mng.WorldRender.ShaderBundle;
			Material = mng.WorldRender.Material;
			CollisionMold.MoldNew( Data.CollisionModel );
			Data.Transform.SetParentFromInterface( mng.Chunk.World.TransformInterface );
			Data.Transform.Translation = mng.Chunk.BlockPosition.AsFloat;
			RenderFunction = ( SceneObject<SceneObjectData3> so, Shader s, IView view ) => {
				s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * view.VPMatrix );
				s.Set( "uM_mat", so.Data.TransformObject.Matrix );
				if( mng.WorldRender.wireframe )
					Gl.PolygonMode( MaterialFace.Front, PolygonMode.Line );
				so.Mesh.RenderMesh();
				Gl.PolygonMode( MaterialFace.Front, PolygonMode.Fill );
			};
			tileMapping = new ConcurrentDictionary<int, int>();
			tileData = new ConcurrentDictionary<int, VoxFace>();
			genQueues = new ConcurrentQueue<VoxFace>[ VoxelChunk.SIZE ];
			genData = new Queue<VoxFace>();
			clearQueue = new ConcurrentQueue<int>();
			tempGenData = new List<VoxFace>( 64 );
			generateSignal = new AutoResetEvent( false );
			notGenerating = new ManualResetEvent( false );
			updateRequired = new bool[ VoxelChunk.SIZE ];
			updatedThisFrame = new bool[ VoxelChunk.SIZE ];
			failedTiles = new Queue<int>();
			for( int gq = 0; gq < VoxelChunk.SIZE; gq++ ) {
				genQueues[ gq ] = new ConcurrentQueue<VoxFace>();
			}
		}

		internal void InitiateThread() {
			updateGenThread = Mem.Threads.StartNew( GenFunc, "VoxelGen" );
		}

		private void GenFunc() {
			while( Mem.Threads.Running && !mng.Disposed ) {
				generateSignal.WaitOne();
				notGenerating.WaitOne();
				for( int i = 0; i < VoxelChunk.SIZE; i++ ) {
					if( Volatile.Read( ref updateRequired[ i ] ) ) {
						if( Volatile.Read( ref updatedThisFrame[ i ] ) )
							genQueues[ i ].Clear();
						Volatile.Write( ref updatedThisFrame[ i ], true );
						Volatile.Write( ref updateRequired[ i ], false );
						GenerateUpdate( i );
						genCount--;
					}
				}
			}
		}

		internal void SignalGeneration() {
			notGenerating.WaitOne();
			generateSignal.Set();
		}

		internal void UpdateData( Vector3i xyz, int index ) {
			if( dir.X != 0 ) {
				if( !Volatile.Read( ref updateRequired[ xyz.X ] ) ) {
					genCount++;
					Volatile.Write( ref updateRequired[ xyz.X ], true );
				}
			} else if( dir.Y != 0 ) {
				if( !Volatile.Read( ref updateRequired[ xyz.Y ] ) ) {
					genCount++;
					Volatile.Write( ref updateRequired[ xyz.Y ], true );
				}
			} else {
				if( !Volatile.Read( ref updateRequired[ xyz.Z ] ) ) {
					genCount++;
					Volatile.Write( ref updateRequired[ xyz.Z ], true );
				}
			}

			if( tileMapping.TryGetValue( index, out int tileIndex ) ) {
				clearQueue.Enqueue( tileIndex );
				if( tileData.TryGetValue( tileIndex, out VoxFace td ) )
					RemoveTileMapping( td );
			}
		}

		public bool HasTile( int x, int y, int z ) {
			return tileMapping.ContainsKey( mng.Chunk.GetIndex( x, y, z ) );
		}

		protected override void OnDispose() {
			if( Mesh is null )
				return;
			Mesh.Dispose();
			Data.Transform.SetParent( null );
		}

		internal void Update( ref int remaining ) {
			if( Mesh is null )
				Mesh = VoxelMesh = new VoxelChunkMesh( $"Chunk Mesh [{mng.Chunk.Position}:{direction}]", direction, VoxelChunk.SIZE * VoxelChunk.SIZE * VoxelChunk.SIZE / 2 );

			if( genCount == 0 ) {
				notGenerating.Reset();
				while( clearQueue.TryDequeue( out int tile ) ) {
					VoxelMesh.Clear( tile );
					if( !tileData.TryRemove( tile, out _ ) )
						Logging.Warning( $"Failed to remove voxelface from tile data." );
				}

				for( int i = 0; i < VoxelChunk.SIZE; i++ ) {
					if( Volatile.Read( ref updatedThisFrame[ i ] ) ) {
						while( genQueues[ i ].TryDequeue( out VoxFace face ) ) {
							genData.Enqueue( face );
						}
						Volatile.Write( ref updatedThisFrame[ i ], false );
					}
				}

				if( genData.Count > 0 ) {
					bool meshfail = false;
					tempGenData.Clear();
					int i = 0;
					while( i < 512 && remaining > 0 && genData.TryDequeue( out VoxFace face ) ) {
						tempGenData.Add( face );

						i++;
						remaining--;
					}

					if( VoxelMesh.FindRangeStart( tempGenData.Count ) != -1 ) {
						int startIndex = VoxelMesh.FillMultiple( tempGenData.ToArray() );

						failedTiles.Clear();
						for( i = 0; i < tempGenData.Count; i++ )
							if( !SetTileMapping( startIndex + i, tempGenData[ i ] ) )
								failedTiles.Enqueue( startIndex + i );

						while( failedTiles.TryDequeue( out int tileIndex ) ) {
							meshfail = true;
							VoxelMesh.Clear( tileIndex );
						}
					} else {
						for( i = 0; i < tempGenData.Count; i++ ) {
							VoxFace face = tempGenData[ i ];
							int index = VoxelMesh.FillSingle( face.translation, face.scale, face.uv );
							if( !SetTileMapping( index, face ) ) {
								meshfail = true;
								VoxelMesh.Clear( index );
							}
						}
					}
					if( meshfail ) {
						Logging.Warning( $"Mesh generation for [{mng.Chunk.Position},{direction}] failed this frame. Some ghost voxel faces may exist." );
					}
				}
				notGenerating.Set();
			}
		}

		private bool SetTileMapping( int index, VoxFace face ) {
			if( !tileData.TryAdd( index, face ) ) {
				Logging.Warning( $"Couldn't add tiledata reference [{index},{direction}] to facedata [{face.translation},{face.scale},{face.uv}]." );
				return false;
			}

			if( dir.X != 0 ) {
				int xNeg = ( dir.X + 1 ) / 2;
				for( int y = 0; y < face.scale.Y; y++ ) {
					for( int z = 0; z < face.scale.Z; z++ ) {
						if( !tileMapping.TryAdd( mng.Chunk.GetIndex( face.translation.X - xNeg, face.translation.Y + y, face.translation.Z + z ), index ) ) {
							Logging.Warning( $"Couldn't add tilemap reference to tile [{index},{direction}] for block [{mng.Chunk.BlockPosition + (face.translation.X - xNeg, face.translation.Y + y, face.translation.Z + z)}]. [{y},{z}]" );
							return false;
						}
					}
				}
			} else if( dir.Y != 0 ) {
				int yNeg = ( dir.Y + 1 ) / 2;
				for( int x = 0; x < face.scale.X; x++ ) {
					for( int z = 0; z < face.scale.Z; z++ ) {
						if( !tileMapping.TryAdd( mng.Chunk.GetIndex( face.translation.X + x, face.translation.Y - yNeg, face.translation.Z + z ), index ) ) {
							Logging.Warning( $"Couldn't add tilemap reference to tile [{index},{direction}] for block [{mng.Chunk.BlockPosition + (face.translation.X + x, face.translation.Y - yNeg, face.translation.Z + z)}]. [{x},{z}]" );
							return false;
						}
					}
				}
			} else {
				int zNeg = ( dir.Z + 1 ) / 2;
				for( int x = 0; x < face.scale.X; x++ ) {
					for( int y = 0; y < face.scale.Y; y++ ) {
						if( !tileMapping.TryAdd( mng.Chunk.GetIndex( face.translation.X + x, face.translation.Y + y, face.translation.Z - zNeg ), index ) ) {
							Logging.Warning( $"Couldn't add tilemap reference to tile [{index},{direction}] for block [{mng.Chunk.BlockPosition + (face.translation.X + x, face.translation.Y + y, face.translation.Z - zNeg)}]. [{x},{y}]" );
							return false;
						}
					}
				}
			}

			return true;
		}

		private void RemoveTileMapping( VoxFace face ) {
			Vector3i dir = VoxelChunkMesh.DirectionVectors[ (int) direction ];
			if( dir.X != 0 ) {
				int xNeg = ( dir.X + 1 ) / 2;
				for( int y = 0; y < face.scale.Y; y++ ) {
					for( int z = 0; z < face.scale.Z; z++ ) {
						if( !tileMapping.TryRemove( mng.Chunk.GetIndex( face.translation.X - xNeg, face.translation.Y + y, face.translation.Z + z ), out int v ) )
							Logging.Routine( $"Couldn't remove tilemap reference for block [{mng.Chunk.BlockPosition + (face.translation.X - xNeg, face.translation.Y + y, face.translation.Z + z)}] to tile [{v}]!" );
					}
				}
			} else if( dir.Y != 0 ) {
				int yNeg = ( dir.Y + 1 ) / 2;
				for( int x = 0; x < face.scale.X; x++ ) {
					for( int z = 0; z < face.scale.Z; z++ ) {
						if( !tileMapping.TryRemove( mng.Chunk.GetIndex( face.translation.X + x, face.translation.Y - yNeg, face.translation.Z + z ), out int v ) )
							Logging.Routine( $"Couldn't remove tilemap reference for block [{mng.Chunk.BlockPosition + (face.translation.X + x, face.translation.Y - yNeg, face.translation.Z + z)}] to tile [{v}]!" );
					}
				}
			} else {
				int zNeg = ( dir.Z + 1 ) / 2;
				for( int x = 0; x < face.scale.X; x++ ) {
					for( int y = 0; y < face.scale.Y; y++ ) {
						if( !tileMapping.TryRemove( mng.Chunk.GetIndex( face.translation.X + x, face.translation.Y + y, face.translation.Z - zNeg ), out int v ) )
							Logging.Routine( $"Couldn't remove tilemap reference for block [{mng.Chunk.BlockPosition + (face.translation.X + x, face.translation.Y + y, face.translation.Z - zNeg)}] to tile [{v}]!" );
					}
				}
			}
		}

		public void GenerateMesh() {
			switch( direction ) {
				case VoxelChunkMesh.Direction.XPLUS:
					for( int x = 0; x < VoxelChunk.SIZE; x++ ) {
						VoxelChunkMeshGenerator.PregenerateYZ( mng, x, 1, 1, genQueues[ x ] );
						updatedThisFrame[ x ] = true;
					}
					break;
				case VoxelChunkMesh.Direction.XNEG:
					for( int x = 0; x < VoxelChunk.SIZE; x++ ) {
						VoxelChunkMeshGenerator.PregenerateYZ( mng, x, -1, 0, genQueues[ x ] );
						updatedThisFrame[ x ] = true;
					}
					break;
				case VoxelChunkMesh.Direction.YPLUS:
					for( int y = 0; y < VoxelChunk.SIZE; y++ ) {
						VoxelChunkMeshGenerator.PregenerateXZ( mng, y, 1, 1, genQueues[ y ] );
						updatedThisFrame[ y ] = true;
					}
					break;
				case VoxelChunkMesh.Direction.YNEG:
					for( int y = 0; y < VoxelChunk.SIZE; y++ ) {
						VoxelChunkMeshGenerator.PregenerateXZ( mng, y, -1, 0, genQueues[ y ] );
						updatedThisFrame[ y ] = true;
					}
					break;
				case VoxelChunkMesh.Direction.ZPLUS:
					for( int z = 0; z < VoxelChunk.SIZE; z++ ) {
						VoxelChunkMeshGenerator.PregenerateXY( mng, z, 1, 1, genQueues[ z ] );
						updatedThisFrame[ z ] = true;
					}
					break;
				case VoxelChunkMesh.Direction.ZNEG:
					for( int z = 0; z < VoxelChunk.SIZE; z++ ) {
						VoxelChunkMeshGenerator.PregenerateXY( mng, z, -1, 0, genQueues[ z ] );
						updatedThisFrame[ z ] = true;
					}
					break;
			}
		}

		public void GenerateUpdate( int i ) {
			if( dir.X != 0 ) {
				int xAdd = ( dir.X + 1 ) / 2;
				VoxelChunkMeshGenerator.UpdateYZ( mng, this, i, dir.X, xAdd, genQueues[ i ] );
			} else if( dir.Y != 0 ) {
				int yAdd = ( dir.Y + 1 ) / 2;
				VoxelChunkMeshGenerator.UpdateXZ( mng, this, i, dir.Y, yAdd, genQueues[ i ] );
			} else {
				int zAdd = ( dir.Z + 1 ) / 2;
				VoxelChunkMeshGenerator.UpdateXY( mng, this, i, dir.Z, zAdd, genQueues[ i ] );
			}
		}
	}
}
