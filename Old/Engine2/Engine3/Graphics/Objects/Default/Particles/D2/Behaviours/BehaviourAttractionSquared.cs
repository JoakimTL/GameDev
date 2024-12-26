using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	/// <summary>
	/// Attraction becomes weaker with distance. This is inverse square.
	/// </summary>
	public class BehaviourAttractionSquared : IBehaviour2 {

		public Vector2 Point { get; set; }
		public float Magnitude { get; set; }
		public float MinimumDistance { get; set; }

		public BehaviourAttractionSquared( Vector2 point, float mag ) {
			Point = point;
			Magnitude = mag;
			MinimumDistance = 0.01f;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			Vector2 ab = Point - p.Data.Translation;
			float lenSq = Math.Max( ab.LengthSquared, MinimumDistance );
			Vector2 abN = ab / (float) Math.Sqrt( lenSq );
			b.Acceleration += abN * Magnitude / lenSq;
		}
	}
}
