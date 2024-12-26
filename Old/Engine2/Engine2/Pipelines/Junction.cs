using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Pipelines {
	public class Junction {

		public string Name { get; private set; }
		public bool Active { get; protected set; }
		public uint ID { get; private set; }
		public Action Effect { get; protected set; }

		public Junction( string name, Action effect ) {
			Name = name;
			Effect = effect;
			ID = 0;
			Active = false;
		}

		internal void SetID( uint i ) {
			if( i == 0 ) {
				Mem.Logs.Error.WriteLine( $"[{Name}] Index for a junction cannot be 0." );
				return;
			}
			if( ID > 0 ) {
				Mem.Logs.Error.WriteLine( $"[{Name}] Index for this junction has already been set." );
				return;
			}
			ID = i;
		}

		public void SetActive(bool a ) {
			Active = a;
		}

		public override string ToString() {
			return $"[Junction][{Name}:{ID}][{Effect}][{Active}]";
		}

	}
}
