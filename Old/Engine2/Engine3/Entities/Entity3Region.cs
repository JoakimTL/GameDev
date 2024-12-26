using Engine.LinearAlgebra;
using System.Collections.Generic;

namespace Engine.Entities {
	public class Entity3Region : EntityRegion {

		public readonly Vector3i Position;

		public Entity3Region( Vector3i position ) {
			Position = position;
		}

	}
}