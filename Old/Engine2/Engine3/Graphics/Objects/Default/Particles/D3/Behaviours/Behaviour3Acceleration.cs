using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3.Behaviours {
	public class Behaviour3Acceleration : IBehaviour3 {

		public Vector3 Acceleration { get; set; }

		public Behaviour3Acceleration( Vector3 acc ) {
			Acceleration = acc;
		}

		public void Apply( Particle3 p, Behaviour3Manager b ) {
			b.Acceleration += Acceleration;
		}
	}
}
