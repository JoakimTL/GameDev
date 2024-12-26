using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	/// <summary>
	/// Attraction becomes stronger with distance
	/// </summary>
	public class BehaviourInverseAttraction : IBehaviour2 {

		public Vector2 Point { get; set; }
		public float Magnitude { get; set; }

		public BehaviourInverseAttraction( Vector2 point, float mag ) {
			Point = point;
			Magnitude = mag;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.Acceleration += ( Point - p.Data.Translation ) * Magnitude;
		}
	}
}
