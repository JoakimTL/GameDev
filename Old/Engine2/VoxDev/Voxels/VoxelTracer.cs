using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.Voxels {
	public class TraceResult {
		public Vector3i BlockPosition;
		public Vector3 WorldPosition;
		public Vector3i BlockFaceDirection;

		public TraceResult( Vector3i posVoxel, VoxelWorld world, Vector3i face ) {
			BlockPosition = posVoxel;
			WorldPosition = posVoxel * world.TransformInterface.Scale;
			BlockFaceDirection = face;
		}
	}

	public static class VoxelTracer {

		public static TraceResult GetFacing( VoxelWorld world, TraceResult p ) {
			return new TraceResult( p.BlockPosition + p.BlockFaceDirection, world, p.BlockFaceDirection );
		}

		public static bool Find( VoxelWorld world, Vector3 pos, Vector3 dir, Vector3 radius, out TraceResult result ) {
			result = null;

			Vector3 voxelPos = Vector3.Transform( pos - world.TransformInterface.GlobalTranslation, Quaternion.Invert( world.TransformInterface.GlobalRotation ) ) / world.TransformInterface.GlobalScale;
			Vector3i startPoint = voxelPos.IntFloored;
			Vector3i originPoint = startPoint;

			if( ( dir.X == 0 && dir.Y == 0 && dir.Z == 0 ) || float.IsNaN( dir.X ) || float.IsNaN( dir.Y ) || float.IsNaN( dir.Z ) )
				return false;

			dir = Vector3.Transform( dir, Quaternion.Invert( world.TransformInterface.GlobalRotation ) );
			dir.Normalize();

			Vector3i dirSign = new Vector3i( Math.Sign( dir.X ), Math.Sign( dir.Y ), Math.Sign( dir.Z ) );
			Vector3 tStep = new Vector3( Bound( voxelPos.X, dir.X ), Bound( voxelPos.Y, dir.Y ), Bound( voxelPos.Z, dir.Z ) );
			Vector3 tDelta = new Vector3( dirSign.X / dir.X, dirSign.Y / dir.Y, dirSign.Z / dir.Z );
			Vector3i face = new Vector3i();

			while(
				( dirSign.X > 0 ? originPoint.X < startPoint.X + radius.X : originPoint.X > startPoint.X - radius.X ) &&
				( dirSign.Y > 0 ? originPoint.Y < startPoint.Y + radius.Y : originPoint.Y > startPoint.Y - radius.Y ) &&
				( dirSign.Z > 0 ? originPoint.Z < startPoint.Z + radius.Z : originPoint.Z > startPoint.Z - radius.Z ) ) {
				if( world.GetId( originPoint, out ushort id ) ) {
					if( world.BlockSet.GetBlockType( id ).Solid ) {
						result = new TraceResult( originPoint, world, face );
						return true;
					}

					if( tStep.X < tStep.Y ) {
						if( tStep.X < tStep.Z ) {
							if( tStep.X > radius.X )
								break;
							originPoint.X += dirSign.X;
							tStep.X += tDelta.X;
							face = new Vector3i( -dirSign.X, 0, 0 );
						} else {
							if( tStep.Z > radius.Z )
								break;
							originPoint.Z += dirSign.Z;
							tStep.Z += tDelta.Z;
							face = new Vector3i( 0, 0, -dirSign.Z );
						}
					} else {
						if( tStep.Y < tStep.Z ) {
							if( tStep.Y > radius.Y )
								break;
							originPoint.Y += dirSign.Y;
							tStep.Y += tDelta.Y;
							face = new Vector3i( 0, -dirSign.Y, 0 );
						} else {
							if( tStep.Z > radius.Z )
								break;
							originPoint.Z += dirSign.Z;
							tStep.Z += tDelta.Z;
							face = new Vector3i( 0, 0, -dirSign.Z );
						}
					}
				} else {
					break;
				}
			}

			result = new TraceResult( originPoint, world, face );
			return false;
		}

		private static float Bound( float s, float ds ) {
			// Find the smallest positive t such that s+t*ds is an integer.
			if( ds < 0 ) {
				return Bound( -s, -ds );
			} else {
				s = ( s % 1 + 1 ) % 1;
				// problem is now s+t*ds = 1
				return ( 1 - s ) / ds;
			}
		}

	}
}
