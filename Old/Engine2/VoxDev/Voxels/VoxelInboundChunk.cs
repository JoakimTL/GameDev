using Engine;
using Engine.LinearAlgebra;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	class VoxelInboundChunk : VoxelChunk {

		private ushort[,,] ids;
		private Dictionary<int, float> damage;

		private Physics3Model collider;
		public override Physics3Model BlockCollider => GetCollider();

		private bool updated;

		public VoxelInboundChunk( VoxelWorld world, int x, int y, int z ) : base( world, x, y, z ) {
			damage = new Dictionary<int, float>();
			collider = new Physics3Model( "chunk", world.Transform );
		}

		private Physics3Model GetCollider() {
			if( updated )
				UpdateCollider();
			return collider;
		}

		private void UpdateCollider() {

		}

		public override void GenerateBlockCollision() {

		}

		public override void CreateData() {
			ids = new ushort[ SIZE, SIZE, SIZE ];
			MarkDataCreated();
		}

		public override void SetIdUnchecked( Vector3i xyz, ushort id ) => SetIdUnchecked( xyz.X, xyz.Y, xyz.Z, id );

		public override void SetIdUnchecked( int x, int y, int z, ushort id ) {
			if( IsDataCreated )
				ids[ x, y, z ] = id;
		}

		public override bool SetId( Vector3i xyz, ushort id ) => SetId( xyz.X, xyz.Y, xyz.Z, id );

		public override bool SetId( int x, int y, int z, ushort id ) {
			int pId = GetId( x, y, z );
			if( pId != id ) {
				SetIdUnchecked( x, y, z, id );
				return true;
			}
			return false;
		}

		public override ushort GetId( Vector3i xyz ) => GetId( xyz.X, xyz.Y, xyz.Z );

		public override ushort GetId( int x, int y, int z ) {
			if( IsDataCreated )
				return ids[ x, y, z ];
			return 0;
		}

		public override int GetIndex( int x, int y, int z ) {
			return y * SIZE * SIZE + x * SIZE + z;
		}

		public override float IncreaseDamage( Vector3i xyz, float dmgVal ) {
			return IncreaseDamage( xyz.X, xyz.Y, xyz.Z, dmgVal );
		}

		public override float IncreaseDamage( int x, int y, int z, float dmgVal ) {
			int index = GetIndex( x, y, z );
			return IncreaseDamage( index, dmgVal );
		}

		public override float IncreaseDamage( int index, float dmgVal ) {
			float prev = GetDamage( index );
			damage[ index ] = prev + dmgVal;
			return prev;
		}

		public override void SetDamage( Vector3i xyz, float dmgVal ) {
			SetDamage( xyz.X, xyz.Y, xyz.Z, dmgVal );
		}

		public override void SetDamage( int x, int y, int z, float dmgVal ) {
			int index = GetIndex( x, y, z );
			damage[ index ] = dmgVal;
		}

		public override void SetDamage( int index, float dmgVal ) {
			damage[ index ] = dmgVal;
		}

		public override float GetDamage( Vector3i xyz ) {
			return GetDamage( xyz.X, xyz.Y, xyz.Z );
		}

		public override float GetDamage( int x, int y, int z ) {
			int index = GetIndex( x, y, z );
			return GetDamage( index );
		}

		public override float GetDamage( int index ) {
			if( damage.TryGetValue( index, out float dmgVal ) )
				return dmgVal;
			return 0;
		}

		public override bool CreateSaveData( ref ushort[] ids, out KeyValuePair<int, float>[] damage ) {
			if( ids.Length != this.ids.Length ) {
				Logging.Warning( "Attempted to fetch id data with a mismatched array length!" );
				damage = null;
				return false;
			}
			lock( this.ids ) {
				unsafe {
					fixed( ushort* readPointer = this.ids )
					fixed( ushort* writePointer = ids ) {
						Buffer.MemoryCopy( readPointer, writePointer, SIZE * SIZE * SIZE * sizeof( ushort ), SIZE * SIZE * SIZE * sizeof( ushort ) );
					}
				}
			}
			lock( this.damage )
				damage = this.damage.ToArray();
			return true;
		}

		public override bool AttemptLoad() {
			return World.WorldFileIO.LoadChunk( Position, ref damage, ref ids );
		}

	}
}
