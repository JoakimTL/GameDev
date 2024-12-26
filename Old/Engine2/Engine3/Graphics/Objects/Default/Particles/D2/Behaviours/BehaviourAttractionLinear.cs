using Engine.LinearAlgebra;
using System;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	/// <summary>
	/// Attraction becomes weaker with distance. This is not inverse square, rather inverse linear.
	/// </summary>
	public class BehaviourAttractionLinear : IBehaviour2 {

		public Vector2 Point { get; set; }
		public float Magnitude { get; set; }
		public float MinimumDistance { get; set; }

		public BehaviourAttractionLinear( Vector2 point, float mag ) {
			Point = point;
			Magnitude = mag;
			MinimumDistance = 0.01f;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			Vector2 ab = Point - p.Data.Translation;
			float len = Math.Max( ab.Length, MinimumDistance );
			Vector2 abn = ab / len;
			b.Acceleration += abn * Magnitude / len;
		}
	}
}
