using Engine.Graphics.Objects.Default.Meshes.Instancing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class Particle2 : Instance {

		public const int SIZEBYTES = 21;

		public Particle2( int index ) : base( index ) {
		}

		public override bool Update() {
			return false;
		}
	}
}
