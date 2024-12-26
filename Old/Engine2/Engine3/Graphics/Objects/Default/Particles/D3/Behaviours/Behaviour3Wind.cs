using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3.Behaviours {
	public class Behaviour3Wind : IBehaviour3 {

		public float Baseline { get; set; }
		public MutableSinglet<Vector3> Acceleration { get; set; }

		public Behaviour3Wind( MutableSinglet<Vector3> acc, float baseline ) {
			Baseline = baseline;
			Acceleration = acc;
		}

		public void Apply( Particle3 p, Behaviour3Manager b ) {
			b.Acceleration += Acceleration.Value * ( p.Data.Translation.Y - Baseline );
		}
	}
}
