using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourAcceleration : IBehaviour2 {

		public Vector2 Acceleration { get; set; }

		public BehaviourAcceleration( Vector2 acc ) {
			Acceleration = acc;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.Acceleration += Acceleration;
		}
	}
}
