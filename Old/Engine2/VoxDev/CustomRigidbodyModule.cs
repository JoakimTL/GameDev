using Engine.Entities;
using Engine.Entities.D3;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Physics.Collision;
using Engine.Physics.D3;
using Engine.Physics.D3.Shapes;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using VoxDev.Voxels;

namespace VoxDev {
	public class CustomRigidbodyModule : Module {

		private VoxelWorld world;
		public Physics3Model playerModel;
		public Physics3Model nearbyWorldModel;
		private Vector3 motion;
		private Vector3 acceleration;
		private Transform3 worldTransform;
		private CollisionResult<Vector3> result;

		public bool Colliding { get; private set; }

		public CustomRigidbodyModule( VoxelWorld world, Physics3Model playerModel ) {
			this.world = world;
			this.playerModel = playerModel;
			worldTransform = new Transform3();
			worldTransform.SetParentFromInterface( world.TransformInterface );
			nearbyWorldModel = new Physics3Model( "world", worldTransform );
			result = new CollisionResult<Vector3>();
		}

		public void ApplyForce( Vector3 f ) {
			acceleration += f;
		}

		public override void Update( float time, float deltaTime ) {
			if( !Entity.Get( out Transform3Module tm ) )
				return;

			double time2 = Clock64.Standard.Time;
			ApplyForce( (0, -9.81f, 0) );
			motion += acceleration * deltaTime;
			UpdateWorldModel( motion * deltaTime );

			double time2_ = Clock64.Standard.Time;

			Colliding = false;
			//float len = motion.Length;
			//for( float step = 0; step < len; step += world.TransformInterface.Scale.Length ) {

			//X-axis
			{
				tm.Transform.Translation += motion * deltaTime * (1, 0, 0);
				CollisionChecker.CheckCollision( nearbyWorldModel, playerModel, result, true );
				if( result.Colliding ) {
					Vector3 movement = 0;
					foreach( var item in result.Data ) {
						movement += item.Normal * item.Depth;
					}
					movement /= result.Data.Count;
					tm.Transform.Translation += movement;
					Vector3 normal = movement.Normalized;
					motion += normal * Vector3.Dot( -normal, motion ) * (1, 0, 0);
					Colliding = true;
				}
			}
			//Y-axis
			{
				tm.Transform.Translation += motion * deltaTime * (0, 1, 0);
				CollisionChecker.CheckCollision( nearbyWorldModel, playerModel, result, true );
				if( result.Colliding ) {
					Vector3 movement = 0;
					foreach( var item in result.Data ) {
						movement += item.Normal * item.Depth;
					}
					movement /= result.Data.Count;
					tm.Transform.Translation += movement;
					Vector3 normal = movement.Normalized;
					motion += normal * Vector3.Dot( -normal, motion ) * (0, 1, 0);
					Colliding = true;
				}
			}
			//Z-axis
			{
				tm.Transform.Translation += motion * deltaTime * (0, 0, 1);
				CollisionChecker.CheckCollision( nearbyWorldModel, playerModel, result, true );
				if( result.Colliding ) {
					Vector3 movement = 0;
					foreach( var item in result.Data ) {
						movement += item.Normal * item.Depth;
					}
					movement /= result.Data.Count;
					tm.Transform.Translation += movement;
					Vector3 normal = movement.Normalized;
					motion += normal * Vector3.Dot( -normal, motion ) * (0, 0, 1);//Need to figure this out though.
					Colliding = true;
				}
			}
			//}

			motion -= motion * 0.98f * deltaTime;
			acceleration = 0;

			double time2__ = Clock64.Standard.Time;

			Entry.AddDebug( "ct: " + Math.Round( ( time2_ - time2 ) * 1_000_000 ) / 1_000 + ", " + Math.Round( ( time2__ - time2_ ) * 1_000_000 ) / 1_000 );
		}

		private void UpdateWorldModel( Vector3 frameMotion ) {
			nearbyWorldModel.Clear();
			playerModel.UpdateShapes();
			playerModel.GetAABB( out Vector3 min, out Vector3 max );
			Vector3i minToBlock = world.GetBlockPosition( Vector3.Min( Vector3.Min( min, max ), Vector3.Min( min + frameMotion, max + frameMotion ) ) );
			Vector3i maxToBlock = world.GetBlockPosition( Vector3.Max( Vector3.Max( min, max ), Vector3.Max( min + frameMotion, max + frameMotion ) ) );
			Vector3i minBlockPos = Vector3i.Min( minToBlock, maxToBlock ) - 2;
			Vector3i maxBlockPos = Vector3i.Max( minToBlock, maxToBlock ) + 2;
			//minBlockPos -= 2;
			//maxBlockPos += 2;
			Entry.AddDebug( minBlockPos.ToString() );
			Entry.AddDebug( maxBlockPos.ToString() );

			Vector3i len = (maxBlockPos.X - minBlockPos.X, maxBlockPos.Y - minBlockPos.Y, maxBlockPos.Z - minBlockPos.Z);
			if( len.X <= 0 || len.Y <= 0 || len.Z <= 0 )
				return;
			bool[,,] cData = new bool[ len.X, len.Y, len.Z ];

			for( int y = 0; y < len.Y; y++ )
				for( int x = 0; x < len.X; x++ )
					for( int z = 0; z < len.Z; z++ )
						if( world.GetId( minBlockPos + (x, y, z), out ushort id ) )
							cData[ x, y, z ] = world.BlockSet.GetBlockType( id ).Solid;

			for( int y = 0; y < len.Y; y++ ) {
				for( int x = 0; x < len.X; x++ ) {
					for( int z = 0; z < len.Z; z++ ) {
						if( !cData[ x, y, z ] )
							continue;

						int xReach;
						for( xReach = x + 1; xReach < len.X; xReach++ )
							if( !cData[ xReach, y, z ] )
								break;

						int zReach;
						for( zReach = z + 1; zReach < len.Z; zReach++ ) {
							bool valid = true;
							for( int tx = x; tx < xReach; tx++ )
								if( !cData[ tx, y, zReach ] ) {
									valid = false;
									break;
								}
							if( !valid )
								break;
						}

						int yReach;
						for( yReach = y + 1; yReach < len.Y; yReach++ ) {
							bool valid = true;
							for( int tz = z; tz < zReach; tz++ ) {
								for( int tx = x; tx < xReach; tx++ )
									if( !cData[ tx, yReach, tz ] ) {
										valid = false;
										break;
									}
								if( !valid )
									break;
							}
							if( !valid )
								break;
						}


						for( int ty = y; ty < yReach; ty++ ) {
							for( int tz = z; tz < zReach; tz++ ) {
								for( int tx = x; tx < xReach; tx++ ) {
									cData[ tx, ty, tz ] = false;
								}
							}
						}

						Shape3Polygon block = new Shape3Polygon( 1, Mem.CollisionMolds.CubeUniform );
						block.Transform.Translation = new Vector3( minBlockPos.X + x + ( xReach - x ) / 2f, minBlockPos.Y + y + ( yReach - y ) / 2f, minBlockPos.Z + z + ( zReach - z ) / 2f );
						block.Transform.Scale = new Vector3( xReach - x, yReach - y, zReach - z ) / 2f;
						nearbyWorldModel.Add( block );
					}
				}
			}
			/*
			 * for( int z = minBlockPos.Z; z <= maxBlockPos.Z; z++ ) {
						if( world.GetId( new Vector3i( x, y, z ), out ushort id ) ) {
							if( world.BlockSet.GetBlockType( id ).Solid ) {
								Shape3Polygon block = new Shape3Polygon( 1, Mem.CollisionMolds.CubeUniform );
								block.Transform.Translation = new Vector3( x, y, z ) + .5f;
								block.Transform.Scale = .5f;
								nearbyWorldModel.Add( block );
							}
						}
					}
			 */
		}

		protected override void Initialize() {

		}
	}
}
