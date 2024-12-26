using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourDrag : IBehaviour2 {
		public float Drag { get; set; }

		public BehaviourDrag( float drag ) {
			Drag = drag;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.Acceleration -= b.Momentum * Drag;
		}
	}
}
