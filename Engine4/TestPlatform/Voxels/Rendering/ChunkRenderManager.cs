using System.Collections.Concurrent;
using Engine;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using TestPlatform.Voxels.World;

namespace TestPlatform.Voxels.Rendering;

public class ChunkRenderManager : DisposableIdentifiable {

	private readonly VoxelWorldRenderManager _worldRenderManager;
	private readonly VoxelChunk _chunk;
	private readonly AutoResetEvent _updated;
	private readonly ManualResetEvent _updateWait;
	private readonly bool _isUpdating;
	private readonly ConcurrentQueue<Vector3i> _updates;
	private readonly bool[] _updatesX;
	private readonly bool[] _updatesY;
	private readonly bool[] _updatesZ;
	private readonly Dictionary<ChunkRenderDirectionIndex, List<ChunkFaceManager>> _opaqueChunks;
	private readonly Dictionary<ChunkRenderDirectionIndex, List<ChunkFaceManager>> _transparentChunks;
	private readonly List<ChunkFaceManager> _chunkRenderData;
	private readonly ConcurrentQueue<(ChunkRenderDirectionIndex, ChunkFaceManager, bool)> _newSceneData;
	private readonly List<ChunkRender> _chunkRenders;
	private float _lastAccess;

	protected override string UniqueNameTag => this._chunk.ToString();

	public ChunkRenderManager( VoxelWorldRenderManager worldRenderManager, VoxelChunk chunk ) {
		this._worldRenderManager = worldRenderManager;
		this._chunk = chunk;
		this._updated = new AutoResetEvent( false );
		this._updates = new ConcurrentQueue<Vector3i>();
		this._updatesX = new bool[ VoxelChunk.SideLength ];
		this._updatesY = new bool[ VoxelChunk.SideLength ];
		this._updatesZ = new bool[ VoxelChunk.SideLength ];
		this._opaqueChunks = new Dictionary<ChunkRenderDirectionIndex, List<ChunkFaceManager>>();
		this._transparentChunks = new Dictionary<ChunkRenderDirectionIndex, List<ChunkFaceManager>>();
		this._chunkRenderData = new List<ChunkFaceManager>();
		this._newSceneData = new ConcurrentQueue<(ChunkRenderDirectionIndex, ChunkFaceManager, bool)>();
		this._chunkRenders = new List<ChunkRender>();
		this._updateWait = new ManualResetEvent( true );
		this._isUpdating = false;
	}

	public ChunkFaceManager GetAvailableMesh( ChunkRenderDirectionIndex index, bool transparent ) {
		List<ChunkFaceManager>? renders;
		if ( !transparent ) {
			if ( !this._opaqueChunks.TryGetValue( index, out renders ) )
				this._opaqueChunks.Add( index, renders = new List<ChunkFaceManager>() );
		} else {
			if ( !this._transparentChunks.TryGetValue( index, out renders ) )
				this._transparentChunks.Add( index, renders = new List<ChunkFaceManager>() );
		}
		if ( renders is null )
			throw new NullReferenceException( "ChunkRender is null, can't render chunk." );
		for ( int i = 0; i < renders.Count; i++ ) {
			if ( renders[ i ].HasSpace )
				return renders[ i ];
		}
		VertexMesh<Vertex3>? m = this._worldRenderManager.GetMesh( (int) index );
		if ( m is null )
			throw new NullReferenceException( "Mesh is null, can't render chunk." );
		ChunkFaceManager newChunkRenderFaceManager = new( $"{this}:{index}" );
		this._newSceneData.Enqueue( (index, newChunkRenderFaceManager, transparent) );
		renders.Add( newChunkRenderFaceManager );
		return newChunkRenderFaceManager;
	}

	public IReadOnlyList<ChunkFaceManager>? GetMeshes( ChunkRenderDirectionIndex index, bool transparent ) {
		List<ChunkFaceManager>? renders;
		if ( !transparent ) {
			if ( !this._opaqueChunks.TryGetValue( index, out renders ) )
				return null;
		} else {
			if ( !this._transparentChunks.TryGetValue( index, out renders ) )
				return null;
		}
		if ( renders is null )
			return null;
		return renders;
	}

	public void Update( float lastAccess ) {
		this._lastAccess = lastAccess;

		if ( this._isUpdating )
			this._updateWait.WaitOne();
		while ( this._newSceneData.TryDequeue( out (ChunkRenderDirectionIndex, ChunkFaceManager, bool) sceneData ) ) {
			VertexMesh<Vertex3>? m = this._worldRenderManager.GetMesh( (int) sceneData.Item1 );
			if ( m is null )
				throw new NullReferenceException( "Mesh is null, can't render chunk." );
			ChunkRender chunkRender = new( m, sceneData.Item2.VoxelData, sceneData.Item3 );
			this._chunkRenders.Add( chunkRender );
			this._worldRenderManager.Add( chunkRender );
			this._chunkRenderData.Add( sceneData.Item2 );
		}

	}

	public bool ShouldDispose( float time, float lifetime ) => time - this._lastAccess > lifetime;

	public void GenerateMesh() {
		this.LogLine( $"Generating mesh!", Log.Level.LOW );
		this._chunk.VoxelChanged += VoxelUpdate;
		for ( uint x = 0; x < VoxelChunk.SideLength; x++ ) {
			GenerateYZ( x, false );
			GenerateYZ( x, true );
		}
		for ( uint y = 0; y < VoxelChunk.SideLength; y++ ) {
			GenerateXZ( y, false );
			GenerateXZ( y, true );
		}
		for ( uint z = 0; z < VoxelChunk.SideLength; z++ ) {
			GenerateXY( z, false );
			GenerateXY( z, true );
		}
		this._chunk.VoxelRenderChange += VoxelRenderUpdate;
		//this.LogLine( $"Mesh generated, containing {this._opaqueChunks.SelectMany( p => p.Value.Select( q => q.FaceCount ) ).Aggregate( ( p, q ) => p + q )} faces!", Log.Level.NORMAL );
		Resources.Get<ThreadManager>().Start( UpdateMesh, $"{this.FullName}" );
	}

	private void VoxelRenderUpdate() => this._updated.Set();
	private void VoxelUpdate( Vector3i local ) => this._updates.Enqueue( local );

	private static int GetIndex( int x, int y ) => ( x * (int) VoxelChunk.SideLength ) + y;

	public void ClearAll() {
		for ( int i = 0; i < this._chunkRenderData.Count; i++ )
			this._chunkRenderData[ i ].ClearAll();
	}

	public void ClearYZ( uint x, bool right ) {
		IReadOnlyList<ChunkFaceManager>? opaqueRender = GetMeshes( right ? ChunkRenderDirectionIndex.RIGHT : ChunkRenderDirectionIndex.LEFT, false );
		IReadOnlyList<ChunkFaceManager>? transparentRender = GetMeshes( right ? ChunkRenderDirectionIndex.RIGHT : ChunkRenderDirectionIndex.LEFT, true );
		if ( opaqueRender is not null )
			for ( int i = 0; i < opaqueRender.Count; i++ )
				opaqueRender[ i ].Clear( x );
		if ( transparentRender is not null )
			for ( int i = 0; i < transparentRender.Count; i++ )
				transparentRender[ i ].Clear( x );
	}

	public void ClearXZ( uint y, bool up ) {
		IReadOnlyList<ChunkFaceManager>? opaqueRender = GetMeshes( up ? ChunkRenderDirectionIndex.UP : ChunkRenderDirectionIndex.DOWN, false );
		IReadOnlyList<ChunkFaceManager>? transparentRender = GetMeshes( up ? ChunkRenderDirectionIndex.UP : ChunkRenderDirectionIndex.DOWN, true );
		if ( opaqueRender is not null )
			for ( int i = 0; i < opaqueRender.Count; i++ )
				opaqueRender[ i ].Clear( y );
		if ( transparentRender is not null )
			for ( int i = 0; i < transparentRender.Count; i++ )
				transparentRender[ i ].Clear( y );
	}

	public void ClearXY( uint z, bool forward ) {
		IReadOnlyList<ChunkFaceManager>? opaqueRender = GetMeshes( forward ? ChunkRenderDirectionIndex.FORWARD : ChunkRenderDirectionIndex.BACKWARD, false );
		IReadOnlyList<ChunkFaceManager>? transparentRender = GetMeshes( forward ? ChunkRenderDirectionIndex.FORWARD : ChunkRenderDirectionIndex.BACKWARD, true );
		if ( opaqueRender is not null )
			for ( int i = 0; i < opaqueRender.Count; i++ )
				opaqueRender[ i ].Clear( z );
		if ( transparentRender is not null )
			for ( int i = 0; i < transparentRender.Count; i++ )
				transparentRender[ i ].Clear( z );
	}

	public void GenerateXZ( uint y, bool up ) {
		//Remember to clear out the old data on this layer.
		int yDirection = up ? 1 : -1;
		Span<bool> occupation = stackalloc bool[ (int) VoxelChunk.SideLength * (int) VoxelChunk.SideLength ];
		VoxelChunk? checkChunk = this._worldRenderManager.World.GetChunk( this._chunk.Translation + new Vector3i( 0, (int) y + yDirection, 0 ) );

		for ( int x = 0; x < VoxelChunk.SideLength; x++ )
			for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
				Voxel? v = Voxel.Get( this._chunk.GetId( new Vector3i( x, (int) y, z ) ) );
				if ( v is null || v == Voxel.Air ) {
					//These voxels are not rendered, so we skip them by saying they're occupied.
					occupation[ GetIndex( x, z ) ] = true;
				}
			}

		if ( checkChunk is not null ) {
			if ( checkChunk != this._chunk ) {
				for ( int x = 0; x < VoxelChunk.SideLength; x++ )
					for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
						int index = GetIndex( x, z );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( checkChunk.ToLocal( this._chunk.ToGlobal( new Vector3i( x, (int) y + yDirection, z ) ) ) ) );
							if ( v is not null && !v.Transparent ) {
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
							}
						}
					}
			} else {
				for ( int x = 0; x < VoxelChunk.SideLength; x++ )
					for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
						int index = GetIndex( x, z );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( new Vector3i( x, (int) y + yDirection, z ) ) );
							if ( v is not null && !v.Transparent ) {
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
							}
						}
					}
			}
		}

		for ( int startZ = 0; startZ < VoxelChunk.SideLength; startZ++ ) {
			for ( int startX = 0; startX < VoxelChunk.SideLength; startX++ ) {
				//Find the first block on this z level that is not already rendered.
				if ( occupation[ GetIndex( startX, startZ ) ] )
					continue;
				//If the entire row is rendered, go to the next row.
				if ( startX == VoxelChunk.SideLength )
					break;
				//Find the voxel type we'll be rendering here.
				ushort voxelId = this._chunk.GetId( new Vector3i( startX, (int) y, startZ ) );

				Voxel? voxelType = Voxel.Get( voxelId );

				if ( voxelType is null || voxelType == Voxel.Air )
					continue;

				int endX = startX + 1;
				//Find the length of the face we're rendering.
				for ( ; endX < VoxelChunk.SideLength; endX++ ) {
					if ( occupation[ GetIndex( endX, startZ ) ] )
						break;
					if ( this._chunk.GetId( new Vector3i( endX, (int) y, startZ ) ) != voxelId )
						break;
				}

				int endZ = startZ + 1;
				for ( ; endZ < VoxelChunk.SideLength; endZ++ ) {
					for ( int x = startX; x < endX; x++ ) {
						if ( occupation[ GetIndex( x, endZ ) ] )
							goto endSearch;
						if ( this._chunk.GetId( new Vector3i( x, (int) y, endZ ) ) != voxelId )
							goto endSearch;
					}
				}
			endSearch:
				for ( int x = startX; x < endX; x++ )
					for ( int z = startZ; z < endZ; z++ )
						occupation[ GetIndex( x, z ) ] = true;

				VoxelFaceData face = new() {
					TranslationX = this._chunk.Translation.X + startX,
					TranslationY = this._chunk.Translation.Y + (int) y,
					TranslationZ = this._chunk.Translation.Z + startZ,
					ScaleX = (byte) ( endX - startX ),
					ScaleY = 1,
					ScaleZ = (byte) ( endZ - startZ ),
					Id = voxelId
				};

				ChunkFaceManager? render = GetAvailableMesh( up ? ChunkRenderDirectionIndex.UP : ChunkRenderDirectionIndex.DOWN, voxelType.Transparent );

				if ( render is null )
					throw new NullReferenceException( "Chosen chunk render is null." );

				render.AddSimple( face );
			}
		}
	}

	public void GenerateXY( uint z, bool forward ) {
		//Remember to clear out the old data on this layer.
		int zDirection = forward ? 1 : -1;
		Span<bool> occupation = stackalloc bool[ (int) VoxelChunk.SideLength * (int) VoxelChunk.SideLength ];
		VoxelChunk? checkChunk = this._worldRenderManager.World.GetChunk( this._chunk.Translation + new Vector3i( 0, 0, (int) z + zDirection ) );

		for ( int x = 0; x < VoxelChunk.SideLength; x++ )
			for ( int y = 0; y < VoxelChunk.SideLength; y++ ) {
				Voxel? v = Voxel.Get( this._chunk.GetId( new Vector3i( x, y, (int) z ) ) );
				if ( v is null || v == Voxel.Air ) {
					//These voxels are not rendered, so we skip them by saying they're occupied.
					occupation[ GetIndex( x, y ) ] = true;
				}
			}

		if ( checkChunk is not null ) {
			if ( checkChunk != this._chunk ) {
				for ( int x = 0; x < VoxelChunk.SideLength; x++ )
					for ( int y = 0; y < VoxelChunk.SideLength; y++ ) {
						int index = GetIndex( x, y );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( checkChunk.ToLocal( this._chunk.ToGlobal( new Vector3i( x, y, (int) z + zDirection ) ) ) ) );
							if ( v is not null && !v.Transparent )
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
						}
					}
			} else {
				for ( int x = 0; x < VoxelChunk.SideLength; x++ )
					for ( int y = 0; y < VoxelChunk.SideLength; y++ ) {
						int index = GetIndex( x, y );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( new Vector3i( x, y, (int) z + zDirection ) ) );
							if ( v is not null && !v.Transparent )
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
						}
					}
			}
		}

		for ( int startY = 0; startY < VoxelChunk.SideLength; startY++ ) {
			for ( int startX = 0; startX < VoxelChunk.SideLength; startX++ ) {
				//Find the first block on this z level that is not already rendered.
				if ( occupation[ GetIndex( startX, startY ) ] )
					continue;
				//If the entire row is rendered, go to the next row.
				if ( startX == VoxelChunk.SideLength )
					break;
				//Find the voxel type we'll be rendering here.
				ushort voxelId = this._chunk.GetId( new Vector3i( startX, startY, (int) z ) );

				Voxel? voxelType = Voxel.Get( voxelId );

				if ( voxelType is null || voxelType == Voxel.Air )
					continue;

				int endX = startX + 1;
				//Find the length of the face we're rendering.
				for ( ; endX < VoxelChunk.SideLength; endX++ ) {
					if ( occupation[ GetIndex( endX, startY ) ] )
						break;
					if ( this._chunk.GetId( new Vector3i( endX, startY, (int) z ) ) != voxelId )
						break;
				}

				int endY = startY + 1;
				for ( ; endY < VoxelChunk.SideLength; endY++ ) {
					for ( int x = startX; x < endX; x++ ) {
						if ( occupation[ GetIndex( x, endY ) ] )
							goto endSearch;
						if ( this._chunk.GetId( new Vector3i( x, endY, (int) z ) ) != voxelId )
							goto endSearch;
					}
				}
			endSearch:
				for ( int x = startX; x < endX; x++ )
					for ( int y = startY; y < endY; y++ )
						occupation[ GetIndex( x, y ) ] = true;

				VoxelFaceData face = new() {
					TranslationX = this._chunk.Translation.X + startX,
					TranslationY = this._chunk.Translation.Y + startY,
					TranslationZ = this._chunk.Translation.Z + (int) z,
					ScaleX = (byte) ( endX - startX ),
					ScaleY = (byte) ( endY - startY ),
					ScaleZ = 1,
					Id = voxelId
				};

				ChunkFaceManager? render = GetAvailableMesh( forward ? ChunkRenderDirectionIndex.FORWARD : ChunkRenderDirectionIndex.BACKWARD, voxelType.Transparent );

				if ( render is null )
					throw new NullReferenceException( "Chosen chunk render is null." );
				
				render.AddSimple( face );
			}
		}
	}

	public void GenerateYZ( uint x, bool right ) {
		//Remember to clear out the old data on this layer.
		int xDirection = right ? 1 : -1;
		Span<bool> occupation = stackalloc bool[ (int) VoxelChunk.SideLength * (int) VoxelChunk.SideLength ];
		VoxelChunk? checkChunk = this._worldRenderManager.World.GetChunk( this._chunk.Translation + new Vector3i( (int) x + xDirection, 0, 0 ) );

		for ( int y = 0; y < VoxelChunk.SideLength; y++ )
			for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
				Voxel? v = Voxel.Get( this._chunk.GetId( new Vector3i( (int) x, y, z ) ) );
				if ( v is null || v == Voxel.Air ) {
					//These voxels are not rendered, so we skip them by saying they're occupied.
					occupation[ GetIndex( y, z ) ] = true;
				}
			}

		if ( checkChunk is not null ) {
			if ( checkChunk != this._chunk ) {
				for ( int y = 0; y < VoxelChunk.SideLength; y++ )
					for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
						int index = GetIndex( y, z );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( checkChunk.ToLocal( this._chunk.ToGlobal( new Vector3i( (int) x + xDirection, y, z ) ) ) ) );
							if ( v is not null && !v.Transparent )
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
						}
					}
			} else {
				for ( int y = 0; y < VoxelChunk.SideLength; y++ )
					for ( int z = 0; z < VoxelChunk.SideLength; z++ ) {
						int index = GetIndex( y, z );
						if ( !occupation[ index ] ) {
							Voxel? v = Voxel.Get( checkChunk.GetId( new Vector3i( (int) x + xDirection, y, z ) ) );
							if ( v is not null && !v.Transparent )
								//These voxel faces are not neccessary, as they are obscured by other voxels
								occupation[ index ] = true;
						}
					}
			}
		}

		for ( int startZ = 0; startZ < VoxelChunk.SideLength; startZ++ ) {
			for ( int startY = 0; startY < VoxelChunk.SideLength; startY++ ) {
				//Find the first block on this z level that is not already rendered.
				if ( occupation[ GetIndex( startY, startZ ) ] )
					continue;
				//If the entire row is rendered, go to the next row.
				if ( startY == VoxelChunk.SideLength )
					break;
				//Find the voxel type we'll be rendering here.
				ushort voxelId = this._chunk.GetId( new Vector3i( (int) x, startY, startZ ) );

				Voxel? voxelType = Voxel.Get( voxelId );

				if ( voxelType is null || voxelType == Voxel.Air )
					continue;

				int endY = startY + 1;
				//Find the length of the face we're rendering.
				for ( ; endY < VoxelChunk.SideLength; endY++ ) {
					if ( occupation[ GetIndex( endY, startZ ) ] )
						break;
					if ( this._chunk.GetId( new Vector3i( (int) x, endY, startZ ) ) != voxelId )
						break;
				}

				int endZ = startZ + 1;
				for ( ; endZ < VoxelChunk.SideLength; endZ++ ) {
					for ( int y = startY; y < endY; y++ ) {
						if ( occupation[ GetIndex( y, endZ ) ] )
							goto endSearch;
						if ( this._chunk.GetId( new Vector3i( (int) x, y, endZ ) ) != voxelId )
							goto endSearch;
					}
				}
			endSearch:
				for ( int y = startY; y < endY; y++ )
					for ( int z = startZ; z < endZ; z++ )
						occupation[ GetIndex( y, z ) ] = true;

				VoxelFaceData face = new() {
					TranslationX = this._chunk.Translation.X + (int) x,
					TranslationY = this._chunk.Translation.Y + startY,
					TranslationZ = this._chunk.Translation.Z + startZ,
					ScaleX = 1,
					ScaleY = (byte) ( endY - startY ),
					ScaleZ = (byte) ( endZ - startZ ),
					Id = voxelId,
				};

				ChunkFaceManager? render = GetAvailableMesh( right ? ChunkRenderDirectionIndex.RIGHT : ChunkRenderDirectionIndex.LEFT, voxelType.Transparent );

				if ( render is null )
					throw new NullReferenceException( "Chosen chunk render is null." );

				render.AddSimple( face );
			}
		}
	}

	public void UpdateMesh() {
		while ( !this.Disposed ) {
			if ( this._updated.WaitOne( 500 ) ) {
				bool updated = false;
				while ( this._updates.TryDequeue( out Vector3i update ) ) {
					updated = true;
				}

				if ( !updated )
					continue;

				ClearAll();

				for ( uint x = 0; x < VoxelChunk.SideLength; x++ ) {
					GenerateYZ( x, false );
					GenerateYZ( x, true );
				}
				for ( uint y = 0; y < VoxelChunk.SideLength; y++ ) {
					GenerateXZ( y, false );
					GenerateXZ( y, true );
				}
				for ( uint z = 0; z < VoxelChunk.SideLength; z++ ) {
					GenerateXY( z, false );
					GenerateXY( z, true );
				}

				for ( uint i = 0; i < VoxelChunk.SideLength; i++ ) {
					this._updatesX[ i ] = false;
					this._updatesY[ i ] = false;
					this._updatesZ[ i ] = false;
				}
			}
		}
	}

	protected override bool OnDispose() {
		for ( int i = 0; i < this._chunkRenders.Count; i++ )
			this._chunkRenders[ i ].Dispose();
		this._opaqueChunks.Clear();
		this._transparentChunks.Clear();
		return true;
	}
}
