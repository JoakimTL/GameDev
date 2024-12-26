using Engine;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Physics.D3;
using Engine.Utilities.Data;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public class VoxelWorld {

		public string Name { get; private set; }
		public bool HasBounds { get; private set; }
		public bool SavesUnloaded { get; private set; }
		public bool SaveOnExit { get; private set; }
		public float UnloadTime { get; private set; }
		public bool Loaded { get; private set; }
		public Vector3i LowerBounds { get; private set; }
		public Vector3i UpperBounds { get; private set; }
		public VoxelBlockSet BlockSet { get; private set; }
		private VoxelWorldGenerator worldGen;
		private Dictionary3Layered<int, VoxelChunk> chunks;
		private HashSet<VoxelChunk> chunkSaveSet;
		private List<VoxelChunk> unloaded;

		public delegate void WorldBlockChangeHandler( Vector3i xyz );
		public event WorldBlockChangeHandler BlockChange;

		private float saveUpdateTime;
		private volatile bool saveForced;
		private AutoResetEvent saveEvent;
		private Thread saveThread;
		public VoxelDataIOManager WorldFileIO { get; private set; }

		private Transform3 transform;
		internal Transform3 Transform { get => transform; }
		public TransformInterface<Vector3, Quaternion, Vector3> TransformInterface => transform.Interface;

		public VoxelWorld( string name, VoxelWorldGenerator worldGen, VoxelBlockSet set, Vector3 scale, bool saveOnExit, float unloadTimeS = 60 ) {
			Name = name;
			this.worldGen = worldGen;
			SaveOnExit = saveOnExit;
			SavesUnloaded = unloadTimeS > 0;
			BlockSet = set;
			HasBounds = false;
			UnloadTime = unloadTimeS;
			chunks = new Dictionary3Layered<int, VoxelChunk>();
			chunkSaveSet = new HashSet<VoxelChunk>();
			unloaded = new List<VoxelChunk>();
			transform = new Transform3() {
				Scale = scale
			};

			Loaded = true;
			saveEvent = new AutoResetEvent( false );
			WorldFileIO = new VoxelDataIOManager( this );
			saveThread = Mem.Threads.StartNew( SaveFunc, "World Saver", false );
		}

		/// <summary>
		/// Saves the world as soon as possible
		/// </summary>
		public void Save() {
			saveForced = true;
			saveEvent.Set();
		}

		private void SaveFunc() {
			while( Mem.Threads.Running && Loaded ) {
				saveEvent.WaitOne( 1000 );
				if( !Loaded )
					break;
				if( saveForced ) {
					saveForced = false;
					SaveWorld();
				} else if( SavesUnloaded ) {
					unloaded.Clear();
					float time = Clock32.Standard.Time;
					lock( chunkSaveSet ) {
						foreach( VoxelChunk chunk in chunkSaveSet ) {
							if( chunk.Expired( time, UnloadTime ) ) {
								unloaded.Add( chunk );
								WorldFileIO.SaveChunk( chunk );
							}
						}
						for( int i = 0; i < unloaded.Count; i++ ) {
							chunkSaveSet.Remove( unloaded[ i ] );
							chunks.Remove( unloaded[ i ].Position.X, unloaded[ i ].Position.Y, unloaded[ i ].Position.Z );
						}
					}
				}
			}
			if( saveForced || SaveOnExit ) {
				saveForced = false;
				SaveWorld();
			}
		}

		public static bool CompressIdData( ushort[] idData, byte[] saveData, out int dataLength, out byte compressionLevel ) {
			compressionLevel = 0;
			dataLength = 0;
			if( idData.Length * 2 != saveData.Length )
				return false;
			return true;
		}

		private void SaveWorld() {
			Logging.Routine( $"Saving world [{Name}]!" );
			double t = Clock64.Standard.Time;
			saveUpdateTime = Clock32.Standard.Time;
			float c = 0;
			foreach( VoxelChunk chunk in chunkSaveSet ) {
				WorldFileIO.SaveChunk( chunk );
				c++;
				if( Clock32.Standard.Time > saveUpdateTime + 1 ) {
					Logging.Routine( $"Still saving [{Name}]... {Math.Round( c / chunkSaveSet.Count * 10000 ) / 100}%!" );
					saveUpdateTime = Clock32.Standard.Time;
				}
			}
			Logging.Routine( $"Saving [{Name}] took [{Math.Round( ( Clock64.Standard.Time - t ) * 10000 ) / 10}]ms!" );
		}

		/// <summary>
		/// Use ForceSave to save the world before unloading!
		/// </summary>
		public void Unload( bool save ) {
			Loaded = false;
			saveForced = save;
		}

		public void SetBounds( Vector3i boundA, Vector3i boundB ) {
			LowerBounds = (Math.Min( boundA.X, boundB.X ), Math.Min( boundA.Y, boundB.Y ), Math.Min( boundA.Z, boundB.Z ));
			UpperBounds = (Math.Max( boundA.X, boundB.X ), Math.Max( boundA.Y, boundB.Y ), Math.Max( boundA.Z, boundB.Z ));
			HasBounds = true;
		}

		public void DisableBounds() {
			HasBounds = false;
		}

		public void SetId( Vector3 worldPosition, ushort id ) {
			Vector3i blockPosition = GetBlockPosition( worldPosition );
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			if( GetChunk( chunkPosition ).SetId( blockPosition - chunkPosition * VoxelChunk.SIZE, id ) ) {
				SetBlockUpdated( blockPosition );
				SetBlockUpdated( blockPosition + (1, 0, 0) );
				SetBlockUpdated( blockPosition - (1, 0, 0) );
				SetBlockUpdated( blockPosition + (0, 1, 0) );
				SetBlockUpdated( blockPosition - (0, 1, 0) );
				SetBlockUpdated( blockPosition + (0, 0, 1) );
				SetBlockUpdated( blockPosition - (0, 0, 1) );
			}
		}

		public void SetId( Vector3i blockPosition, ushort id ) {
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			if( GetChunk( chunkPosition ).SetId( blockPosition - chunkPosition * VoxelChunk.SIZE, id ) ) {
				SetBlockUpdated( blockPosition );
				SetBlockUpdated( blockPosition + (1, 0, 0) );
				SetBlockUpdated( blockPosition - (1, 0, 0) );
				SetBlockUpdated( blockPosition + (0, 1, 0) );
				SetBlockUpdated( blockPosition - (0, 1, 0) );
				SetBlockUpdated( blockPosition + (0, 0, 1) );
				SetBlockUpdated( blockPosition - (0, 0, 1) );
			}
		}

		public ushort GetIdBlocking( Vector3 worldPosition ) {
			Vector3i blockPosition = GetBlockPosition( worldPosition );
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			VoxelChunk chunk = GetChunk( chunkPosition );
			chunk.GeneratedWaiter.WaitOne();
			return chunk.GetId( blockPosition - chunkPosition * VoxelChunk.SIZE );
		}

		public ushort GetIdBlocking( Vector3i blockPosition ) {
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			VoxelChunk chunk = GetChunk( chunkPosition );
			chunk.GeneratedWaiter.WaitOne();
			return chunk.GetId( blockPosition - chunkPosition * VoxelChunk.SIZE );
		}

		public bool GetBlockType( Vector3 worldPosition, out VoxelBlockType type ) {
			Vector3i blockPosition = GetBlockPosition( worldPosition );
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			VoxelChunk chunk = GetChunk( chunkPosition );
			type = null;
			if( !chunk.Generated )
				return false;
			type = BlockSet.GetBlockType( chunk.GetId( blockPosition - chunkPosition * VoxelChunk.SIZE ) );
			return true;
		}

		public bool GetId( Vector3 worldPosition, out ushort id ) {
			Vector3i blockPosition = GetBlockPosition( worldPosition );
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			VoxelChunk chunk = GetChunk( chunkPosition );
			id = 0;
			if( !chunk.Generated )
				return false;
			id = chunk.GetId( blockPosition - chunkPosition * VoxelChunk.SIZE );
			return true;
		}

		public bool GetId( Vector3i blockPosition, out ushort id ) {
			Vector3i chunkPosition = GetChunkPosition( blockPosition );
			VoxelChunk chunk = GetChunk( chunkPosition );
			id = 0;
			if( !chunk.Generated )
				return false;
			id = chunk.GetId( blockPosition - chunkPosition * VoxelChunk.SIZE );
			return true;
		}

		public bool GetId( int x, int y, int z, out ushort id ) {
			return GetId( new Vector3i( x, y, z ), out id );
		}

		public Vector3i GetBlockPosition( Vector3 worldPosition ) {
			return ( Vector3.Transform( worldPosition - transform.GlobalTranslation, Quaternion.Invert( transform.GlobalRotation ) ) / transform.GlobalScale ).IntFloored;
			//Vector3.Transform( worldPosition, transform.Matrix.Transposed() ).IntFloored; DOESN'T WORK
		}

		public Vector3i GetChunkPosition( Vector3 worldPosition ) {
			return ( GetBlockPosition( worldPosition ).AsFloat / VoxelChunk.SIZE ).IntFloored;
		}

		public Vector3i GetChunkPosition( Vector3i blockPosition ) {
			return ( blockPosition.AsFloat / VoxelChunk.SIZE ).IntFloored;
		}

		public VoxelChunk GetChunk( Vector3i chunkPosition ) {
			return GetChunk( chunkPosition.X, chunkPosition.Y, chunkPosition.Z );
		}

		public VoxelChunk GetChunk( int chunkX, int chunkY, int chunkZ ) {
			if( chunks.TryGet( chunkX, chunkY, chunkZ, out VoxelChunk chunk ) ) {
				chunk.Pulse();
				return chunk;
			}
			if( HasBounds ) {
				if( chunkX >= LowerBounds.X && chunkX <= UpperBounds.X && chunkY >= LowerBounds.Y && chunkY <= UpperBounds.Y && chunkZ >= LowerBounds.Z && chunkZ <= UpperBounds.Z ) {
					//Inside boundaries, create an in bound chunk.
					chunks.Add( chunkX, chunkY, chunkZ, chunk = new VoxelInboundChunk( this, chunkX, chunkY, chunkZ ) );
					lock( chunkSaveSet )
						chunkSaveSet.Add( chunk );
					worldGen.Enqueue( chunk );
					return chunk;
				} else {
					//Outside boundaries, create an out bound chunk.
					chunks.Add( chunkX, chunkY, chunkZ, chunk = new VoxelOutboundChunk( this, chunkX, chunkY, chunkZ ) );
					return chunk;
				}
			} else {
				//No boundaries, create an in bound chunk.
				chunks.Add( chunkX, chunkY, chunkZ, chunk = new VoxelInboundChunk( this, chunkX, chunkY, chunkZ ) );
				lock( chunkSaveSet )
					chunkSaveSet.Add( chunk );
				worldGen.Enqueue( chunk );
				return chunk;
			}
		}

		public VoxelChunk GetChunk( Vector3 blockPosition ) {
			Vector3i chunkPosition = ( blockPosition / VoxelChunk.SIZE ).IntFloored;
			return GetChunk( chunkPosition.X, chunkPosition.Y, chunkPosition.Z );
		}

		public void SetBlockUpdated( Vector3i xyz ) {
			BlockChange?.Invoke( xyz );
		}

		public bool TraceFrom( Vector3 pos, Vector3 dir, Vector3 radius, out TraceResult result ) {
			return VoxelTracer.Find( this, pos, dir, radius, out result );
		}

	}
}
