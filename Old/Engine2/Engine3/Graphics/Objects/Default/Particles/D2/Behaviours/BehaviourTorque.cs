using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourTorque : IBehaviour2 {
		public float Torque { get; set; }

		public BehaviourTorque( float torque ) {
			Torque = torque;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.Torque += Torque;
		}
	}
}
