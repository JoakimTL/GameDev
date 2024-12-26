using Engine.LinearAlgebra;
using Engine.Physics.D3;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public abstract class VoxelChunk {

		public const int SIZE = 128;

		public VoxelWorld World { get; private set; }
		public Vector3i Position { get; private set; }
		public Vector3i BlockPosition { get; private set; }
		public bool Generated { get; private set; }
		public ManualResetEvent GeneratedWaiter { get; private set; }
		public bool IsDataCreated { get; private set; }
		public ManualResetEvent DataCreationWaiter { get; private set; }
		private volatile float lastPulse;
		public float LastPulse { get => lastPulse; }

		public abstract Physics3Model BlockCollider { get; }

		public VoxelChunk( VoxelWorld world, int x, int y, int z ) {
			World = world;
			Position = (x, y, z);
			BlockPosition = Position * SIZE;
			Generated = false;
			GeneratedWaiter = new ManualResetEvent( false );
			IsDataCreated = false;
			DataCreationWaiter = new ManualResetEvent( false );
			Pulse();
		}

		public void Pulse() {
			lastPulse = Clock32.Standard.Time;
		}

		public bool Expired( float t, float unloadTime ) {
			return lastPulse + unloadTime < t;
		}

		protected void MarkDataCreated() {
			IsDataCreated = true;
			DataCreationWaiter.Set();
		}

		public void SetGenerated() {
			Generated = true;
			GeneratedWaiter.Set();
		}

		public abstract void GenerateBlockCollision();
		public abstract void CreateData();
		public abstract bool CreateSaveData( ref ushort[] ids, out KeyValuePair<int, float>[] damage );
		public abstract bool AttemptLoad();
		public abstract void SetIdUnchecked( Vector3i xyz, ushort id );
		public abstract void SetIdUnchecked( int x, int y, int z, ushort id );
		public abstract bool SetId( Vector3i xyz, ushort id );
		public abstract bool SetId( int x, int y, int z, ushort id );
		public abstract ushort GetId( Vector3i xyz );
		public abstract ushort GetId( int x, int y, int z );
		public abstract int GetIndex( int x, int y, int z );
		public abstract float IncreaseDamage( Vector3i xyz, float dmgVal );
		public abstract float IncreaseDamage( int x, int y, int z, float dmgVal );
		public abstract float IncreaseDamage( int index, float dmgVal );
		public abstract void SetDamage( Vector3i xyz, float dmgVal );
		public abstract void SetDamage( int x, int y, int z, float dmgVal );
		public abstract void SetDamage( int index, float dmgVal );
		public abstract float GetDamage( Vector3i xyz );
		public abstract float GetDamage( int x, int y, int z );
		public abstract float GetDamage( int index );
	}
}
