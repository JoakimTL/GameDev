using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourGrowth : IBehaviour2 {
		public float Growth { get; set; }

		public BehaviourGrowth( float growth ) {
			Growth = growth;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.GrowthChange += Growth;
		}
	}
}
