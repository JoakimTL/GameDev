using Engine.Entities.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {
	public class EntityRegion3Identifier {



		public bool GetRegion( Entity e, out int regionId) {
			regionId = -1;
			if( !e.Get( out Transform3Module tm ) )
				return false;



			return true;
		}
	}
}
