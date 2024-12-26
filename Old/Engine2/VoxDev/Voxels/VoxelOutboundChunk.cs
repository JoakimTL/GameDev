using Engine.LinearAlgebra;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.Voxels {
	public class VoxelOutboundChunk : VoxelChunk {
		public override Physics3Model BlockCollider => null;

		public VoxelOutboundChunk( VoxelWorld world, int x, int y, int z ) : base( world, x, y, z ) {
			SetGenerated();
		}

		public override float GetDamage( Vector3i xyz ) {
			return 0;
		}

		public override float GetDamage( int x, int y, int z ) {
			return 0;
		}

		public override float GetDamage( int index ) {
			return 0;
		}

		public override ushort GetId( Vector3i xyz ) {
			return 0;
		}

		public override ushort GetId( int x, int y, int z ) {
			return 0;
		}

		public override int GetIndex( int x, int y, int z ) {
			return 0;
		}

		public override float IncreaseDamage( Vector3i xyz, float dmgVal ) {
			return 0;
		}

		public override float IncreaseDamage( int x, int y, int z, float dmgVal ) {
			return 0;
		}

		public override float IncreaseDamage( int index, float dmgVal ) {
			return 0;
		}

		public override void SetDamage( Vector3i xyz, float dmgVal ) { }

		public override void SetDamage( int x, int y, int z, float dmgVal ) { }

		public override void SetDamage( int index, float dmgVal ) { }

		public override void SetIdUnchecked( Vector3i xyz, ushort id ) { }

		public override void SetIdUnchecked( int x, int y, int z, ushort id ) { }

		public override bool SetId( Vector3i xyz, ushort id ) {
			return false;
		}

		public override bool SetId( int x, int y, int z, ushort id ) {
			return false;
		}

		public override void CreateData() { }

		public override bool CreateSaveData( ref ushort[] ids, out KeyValuePair<int, float>[] damage ) {
			damage = null;
			return false;
		}

		public override bool AttemptLoad() {
			return false;
		}

		public override void GenerateBlockCollision() { }
	}
}
