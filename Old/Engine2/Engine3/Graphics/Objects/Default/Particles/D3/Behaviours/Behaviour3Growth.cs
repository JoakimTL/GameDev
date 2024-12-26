using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3.Behaviours {
	public class Behaviour3Growth : IBehaviour3 {

		public float Growth { get; set; }

		public Behaviour3Growth( float growth ) {
			Growth = growth;
		}

		public void Apply( Particle3 p, Behaviour3Manager b ) {
			b.GrowthChange += Growth;
		}
	}
}
