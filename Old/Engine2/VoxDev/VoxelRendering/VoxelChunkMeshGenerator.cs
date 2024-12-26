using Engine;
using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VoxDev.Voxels;

namespace VoxDev.VoxelRendering {
	public class VoxelChunkMeshGenerator {

		public const int TILESIZE = 64;

		private delegate ushort IDFetch( int x, int y, int z );

		private static void GenerateYZ( VoxelChunkRenderManager chunk, int x, int dir, int dirOff, ConcurrentQueue<VoxFace> queue, ref bool[,] handled ) {
			IDFetch getId = default;
			if( ( x == 0 && dir == -1 ) || ( x == VoxelChunk.SIZE - 1 && dir == 1 ) )
				getId = ( int x, int y, int z ) => { return chunk.Chunk.World.GetIdBlocking( chunk.Chunk.BlockPosition + (x, y, z) ); };
			else
				getId = ( int x, int y, int z ) => { return chunk.Chunk.GetId( x, y, z ); };

			VoxelBlockType type;
			VoxelBlockType typeN;
			for( int y = 0; y < VoxelChunk.SIZE; y++ ) {
				for( int z = 0; z < VoxelChunk.SIZE; z++ ) {
					if( handled[ y, z ] )
						continue;

					ushort id = chunk.Chunk.GetId( x, y, z );
					if( id == 0 )
						continue;
					type = chunk.Chunk.World.BlockSet.GetBlockType( id );

					ushort idN = getId( x + dir, y, z );
					typeN = chunk.Chunk.World.BlockSet.GetBlockType( idN );


					if( !typeN.Opaque ) {
						//Go for tile gen!
						handled[ y, z ] = true;

						int maxReach = Math.Min( y + TILESIZE, VoxelChunk.SIZE );
						int yReach = y + 1;
						for( ; yReach < maxReach; yReach++ ) {
							if( handled[ yReach, z ] )
								break;

							if( getId( x, yReach, z ) != id )
								break;

							typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( x + dir, yReach, z ) );
							if( typeN.Opaque )
								break;

							handled[ yReach, z ] = true;
						}

						maxReach = Math.Min( z + TILESIZE, VoxelChunk.SIZE );
						int zReach = z + 1;
						for( ; zReach < maxReach; zReach++ ) {
							bool ok = true;
							for( int ty = y; ty < yReach; ty++ ) {
								if( handled[ ty, zReach ] ) {
									ok = false;
									break;
								}

								if( getId( x, ty, zReach ) != id ) {
									ok = false;
									break;
								}

								typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( x + dir, ty, zReach ) );
								if( typeN.Opaque ) {
									ok = false;
									break;
								}
							}
							if( !ok )
								break;
							for( int ty = y; ty < yReach; ty++ )
								handled[ ty, zReach ] = true;
						}

						queue.Enqueue( new VoxFace() { translation = new Vector3i( x + dirOff, y, z ).AsByte, scale = new Vector3b( 0, (byte) ( yReach - y ), (byte) ( zReach - z ) ), uv = type.TextureUV } );
					}
				}
			}
		}

		private static void GenerateXZ( VoxelChunkRenderManager chunk, int y, int dir, int dirOff, ConcurrentQueue<VoxFace> queue, ref bool[,] handled ) {
			IDFetch getId;
			if( ( y == 0 && dir == -1 ) || ( y == VoxelChunk.SIZE - 1 && dir == 1 ) )
				getId = ( int x, int y, int z ) => { return chunk.Chunk.World.GetIdBlocking( chunk.Chunk.BlockPosition + (x, y, z) ); };
			else
				getId = ( int x, int y, int z ) => { return chunk.Chunk.GetId( x, y, z ); };

			VoxelBlockType type;
			VoxelBlockType typeN;
			for( int x = 0; x < VoxelChunk.SIZE; x++ ) {
				for( int z = 0; z < VoxelChunk.SIZE; z++ ) {
					if( handled[ x, z ] )
						continue;

					ushort id = chunk.Chunk.GetId( x, y, z );
					if( id == 0 )
						continue;
					type = chunk.Chunk.World.BlockSet.GetBlockType( id );

					ushort idN = getId( x, y + dir, z );
					typeN = chunk.Chunk.World.BlockSet.GetBlockType( idN );

					if( !typeN.Opaque ) {
						//Go for tile gen!
						handled[ x, z ] = true;

						int maxReach = Math.Min( x + TILESIZE, VoxelChunk.SIZE );
						int xReach = x + 1;
						for( ; xReach < maxReach; xReach++ ) {
							if( handled[ xReach, z ] )
								break;

							if( chunk.Chunk.GetId( xReach, y, z ) != id )
								break;

							typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( xReach, y + dir, z ) );
							if( typeN.Opaque )
								break;

							handled[ xReach, z ] = true;
						}

						maxReach = Math.Min( z + TILESIZE, VoxelChunk.SIZE );
						int zReach = z + 1;
						for( ; zReach < maxReach; zReach++ ) {
							bool ok = true;
							for( int tx = x; tx < xReach; tx++ ) {
								if( handled[ tx, zReach ] ) {
									ok = false;
									break;
								}

								if( chunk.Chunk.GetId( tx, y, zReach ) != id ) {
									ok = false;
									break;
								}
								typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( tx, y + dir, zReach ) );
								if( typeN.Opaque ) {
									ok = false;
									break;
								}
							}
							if( !ok )
								break;
							for( int tx = x; tx < xReach; tx++ )
								handled[ tx, zReach ] = true;
						}

						queue.Enqueue( new VoxFace() { translation = new Vector3i( x, y + dirOff, z ).AsByte, scale = new Vector3b( (byte) ( xReach - x ), 0, (byte) ( zReach - z ) ), uv = type.TextureUV } );

					}
				}
			}
		}

		private static void GenerateXY( VoxelChunkRenderManager chunk, int z, int dir, int dirOff, ConcurrentQueue<VoxFace> queue, ref bool[,] handled ) {
			IDFetch getId;
			if( ( z == 0 && dir == -1 ) || ( z == VoxelChunk.SIZE - 1 && dir == 1 ) )
				getId = ( int x, int y, int z ) => { return chunk.Chunk.World.GetIdBlocking( chunk.Chunk.BlockPosition + (x, y, z) ); };
			else
				getId = ( int x, int y, int z ) => { return chunk.Chunk.GetId( x, y, z ); };

			VoxelBlockType type;
			VoxelBlockType typeN;
			for( int x = 0; x < VoxelChunk.SIZE; x++ ) {
				for( int y = 0; y < VoxelChunk.SIZE; y++ ) {
					if( handled[ x, y ] )
						continue;

					ushort id = chunk.Chunk.GetId( x, y, z );
					if( id == 0 )
						continue;
					type = chunk.Chunk.World.BlockSet.GetBlockType( id );

					ushort idN = getId( x, y, z + dir );
					typeN = chunk.Chunk.World.BlockSet.GetBlockType( idN );

					if( !typeN.Opaque ) {
						//Go for tile gen!
						handled[ x, y ] = true;

						int maxReach = Math.Min( x + TILESIZE, VoxelChunk.SIZE );
						int xReach = x + 1;
						for( ; xReach < maxReach; xReach++ ) {
							if( handled[ xReach, y ] )
								break;

							if( chunk.Chunk.GetId( xReach, y, z ) != id )
								break;

							typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( xReach, y, z + dir ) );
							if( typeN.Opaque )
								break;

							handled[ xReach, y ] = true;
						}

						maxReach = Math.Min( y + TILESIZE, VoxelChunk.SIZE );
						int yReach = y + 1;
						for( ; yReach < maxReach; yReach++ ) {
							bool ok = true;
							for( int tx = x; tx < xReach; tx++ ) {
								if( handled[ tx, yReach ] ) {
									ok = false;
									break;
								}

								if( chunk.Chunk.GetId( tx, yReach, z ) != id ) {
									ok = false;
									break;
								}

								typeN = chunk.Chunk.World.BlockSet.GetBlockType( getId( tx, yReach, z + dir ) );
								if( typeN.Opaque ) {
									ok = false;
									break;
								}
							}
							if( !ok )
								break;
							for( int tx = x; tx < xReach; tx++ )
								handled[ tx, yReach ] = true;
						}

						queue.Enqueue( new VoxFace() { translation = new Vector3i( x, y, z + dirOff ).AsByte, scale = new Vector3b( (byte) ( xReach - x ), (byte) ( yReach - y ), 0 ), uv = type.TextureUV } );

					}
				}
			}
		}

		public static void UpdateYZ( VoxelChunkRenderManager chunk, VoxelChunkRenderable r, int x, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			for( int y = 0; y < VoxelChunk.SIZE; y++ ) 
				for( int z = 0; z < VoxelChunk.SIZE; z++ ) 
					if( r.HasTile( x, y, z ) )
						handled[ y, z ] = true;
			GenerateYZ( chunk, x, dir, dirOff, queue, ref handled );
		}

		public static void UpdateXZ( VoxelChunkRenderManager chunk, VoxelChunkRenderable r, int y, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			for( int x = 0; x < VoxelChunk.SIZE; x++ )
				for( int z = 0; z < VoxelChunk.SIZE; z++ )
					if( r.HasTile( x, y, z ) )
						handled[ x, z ] = true;
			GenerateXZ( chunk, y, dir, dirOff, queue, ref handled );
		}

		public static void UpdateXY( VoxelChunkRenderManager chunk, VoxelChunkRenderable r, int z, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			for( int x = 0; x < VoxelChunk.SIZE; x++ )
				for( int y = 0; y < VoxelChunk.SIZE; y++ )
					if( r.HasTile( x, y, z ) )
						handled[ x, y ] = true;
			GenerateXY( chunk, z, dir, dirOff, queue, ref handled );
		}

		public static void PregenerateYZ( VoxelChunkRenderManager chunk, int x, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			GenerateYZ( chunk, x, dir, dirOff, queue, ref handled );
		}

		public static void PregenerateXZ( VoxelChunkRenderManager chunk, int y, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			GenerateXZ( chunk, y, dir, dirOff, queue, ref handled );
		}

		public static void PregenerateXY( VoxelChunkRenderManager chunk, int z, int dir, int dirOff, ConcurrentQueue<VoxFace> queue ) {
			bool[,] handled = new bool[ VoxelChunk.SIZE, VoxelChunk.SIZE ];
			GenerateXY( chunk, z, dir, dirOff, queue, ref handled );
		}
	}
}
