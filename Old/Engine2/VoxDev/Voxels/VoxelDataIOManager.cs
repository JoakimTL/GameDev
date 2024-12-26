using Engine;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public class VoxelDataIOManager {

		private VoxelWorld world;
		private AutoResetEvent saveWaiter;

		private ushort[] idArray;
		private byte[] dataArray;
		private int[] dataSaturationArray;

		public VoxelDataIOManager( VoxelWorld world ) {
			this.world = world;
			saveWaiter = new AutoResetEvent( true );
			idArray = new ushort[ VoxelChunk.SIZE * VoxelChunk.SIZE * VoxelChunk.SIZE ];
			dataArray = new byte[ VoxelChunk.SIZE * VoxelChunk.SIZE * VoxelChunk.SIZE * sizeof( ushort ) ];
			dataSaturationArray = new int[ world.BlockSet.TypeCount ];
		}

		public bool LoadChunk( Vector3i position, ref Dictionary<int, float> chunkDamageValues, ref ushort[,,] chunkIdData ) {
			if( !Directory.Exists( $"worlds/{world.Name}/chunks" ) )
				return false;

			if( !File.Exists( $"worlds/{world.Name}/chunks/{position.X},{position.Y},{position.Z}.cnk" ) )
				return false;

			try {
				byte[] data = File.ReadAllBytes( $"worlds/{world.Name}/chunks/{position.X},{position.Y},{position.Z}.cnk" );

				if( Segmentation.Parse( data, out byte[][] segments, out _ ) ) {
					byte[] damageData = segments[ 0 ];
					byte[] idData = segments[ 1 ];


					if( damageData.Length / 4 % 2 == 0 ) {
						int[] indices = new int[ damageData.Length / 8 ];
						float[] damageValues = new float[ damageData.Length / 8 ];

						unsafe {
							fixed( int* iP = indices ) {
								byte* intBytes = (byte*) iP;
								fixed( float* dP = damageValues ) {
									byte* floatBytes = (byte*) dP;
									for( int i = 0; i < damageData.Length; i++ ) {
										intBytes[ i * 4 + 0 ] = damageData[ i * 8 + 0 ];
										intBytes[ i * 4 + 1 ] = damageData[ i * 8 + 1 ];
										intBytes[ i * 4 + 2 ] = damageData[ i * 8 + 2 ];
										intBytes[ i * 4 + 3 ] = damageData[ i * 8 + 3 ];
										damageValues[ i * 4 + 0 ] = damageData[ i * 8 + 4 ];
										damageValues[ i * 4 + 1 ] = damageData[ i * 8 + 5 ];
										damageValues[ i * 4 + 2 ] = damageData[ i * 8 + 6 ];
										damageValues[ i * 4 + 3 ] = damageData[ i * 8 + 7 ];
									}
								}
							}
						}

						for( int i = 0; i < damageData.Length / 8; i++ ) {
							chunkDamageValues.Add( indices[ i ], damageValues[ i ] );
						}
					} else {
						Logging.Warning( $"[{position}] Damage values were assumed corrupt and thus not loaded." );
					}

					if( !DecompressIdData( idData, ref chunkIdData ) ) {
						Logging.Warning( $"[{position}] Error!" );
						return false;
					}
					return true;
				} else {
					Logging.Warning( "Parsing of chunk data failed." );
				}
			} catch( Exception e ) {
				Logging.Warning( "Reading and loading chunk data failed. Either the chunk data is corrupt or the file was busy.\n" + e.ToString() );
			}
			return false;
		}

		public void SaveChunk( VoxelChunk chunk ) {
			saveWaiter.WaitOne();
			try {
				if( !Directory.Exists( $"worlds/{world.Name}/chunks" ) )
					Directory.CreateDirectory( $"worlds/{world.Name}/chunks" );

				if( chunk.CreateSaveData( ref idArray, out KeyValuePair<int, float>[] damage ) ) {
					byte[] damageData = new byte[ damage.Length * ( sizeof( int ) + sizeof( float ) ) ];
					if( damage.Length > 0 ) {
						int[] indices = new int[ damage.Length ];
						float[] damageValues = new float[ damage.Length ];
						unsafe {
							fixed( int* iP = indices ) {
								byte* intBytes = (byte*) iP;
								fixed( float* dP = damageValues ) {
									byte* floatBytes = (byte*) dP;
									for( int i = 0; i < damage.Length; i++ ) {
										damageData[ i * 8 + 0 ] = intBytes[ i * 4 + 0 ];
										damageData[ i * 8 + 1 ] = intBytes[ i * 4 + 1 ];
										damageData[ i * 8 + 2 ] = intBytes[ i * 4 + 2 ];
										damageData[ i * 8 + 3 ] = intBytes[ i * 4 + 3 ];
										damageData[ i * 8 + 4 ] = floatBytes[ i * 4 + 0 ];
										damageData[ i * 8 + 5 ] = floatBytes[ i * 4 + 1 ];
										damageData[ i * 8 + 6 ] = floatBytes[ i * 4 + 2 ];
										damageData[ i * 8 + 7 ] = floatBytes[ i * 4 + 3 ];
									}
								}
							}
						}
					}

					ushort[] data = CompressIdData( idArray );
					byte[] dataBytes = new byte[ data.Length * sizeof( ushort ) ];

					unsafe {
						fixed( ushort* rP = data ) {
							fixed( byte* wP = dataBytes ) {
								byte* readidBytes = (byte*) rP;
								for( int i = 0; i < dataBytes.Length; i++ ) {
									wP[ i ] = readidBytes[ i ];
								}
							}
						}
					}

					File.WriteAllBytes( $"worlds/{world.Name}/chunks/{chunk.Position.X},{chunk.Position.Y},{chunk.Position.Z}.cnk", Segmentation.Segment( damageData, dataBytes ) );
				}
			} catch( Exception e ) {
				Logging.Warning( e );
			} finally {
				saveWaiter.Set();
			}
		}

		private ushort[] CompressIdData( ushort[] idArray ) {
			ushort curId = idArray[ 0 ];
			ushort numId = 1;

			List<ushort> data = new List<ushort>();
			for( int i = 1; i < idArray.Length; i++ ) {
				ushort id = idArray[ i ];
				if( id != curId || numId == ushort.MaxValue ) {
					data.Add( curId );
					data.Add( numId );
					curId = id;
					numId = 1;
				} else {
					numId++;
				}
			}

			data.Add( curId );
			data.Add( numId );

			return data.ToArray();
		}

		private bool DecompressIdData( byte[] data, ref ushort[,,] idArray ) {
			if( data.Length / 2 % 2 != 0 ) {
				Logging.Warning( "Save data is corrupt!" );
				return false;
			}
			int ind = 0;
			unsafe {
				fixed( byte* rd = data ) {
					fixed( ushort* wd = idArray ) {
						ushort* dataP = (ushort*) rd;
						for( int i = 0; i < data.Length / sizeof( ushort ); i += 2 ) {
							ushort id = dataP[ i ];
							ushort len = dataP[ i + 1 ];
							for( int j = 0; j < len; j++ ) {
								wd[ ind++ ] = id;
							}
						}
					}
				}
			}
			Console.WriteLine( ind + ":" + idArray.Length );
			if( ind == idArray.Length )
				return true;
			Logging.Warning( "Mismatch in index endoff and chunk content!" );
			return false;
		}
	}
}
