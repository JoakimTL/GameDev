using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Meshes.Instancing {
	public abstract class Instance {

		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );

		public uint ID { get; private set; }

		public readonly int Index;

		public Instance( int index ) {
			ID = GetNextID();
			Index = index;
		}

		public abstract bool Update();

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is Instance o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( Instance o ) {
			return o.ID == ID;
		}

	}
}
