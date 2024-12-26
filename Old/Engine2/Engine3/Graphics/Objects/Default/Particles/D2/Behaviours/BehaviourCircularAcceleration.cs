using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourCircularAcceleration : IBehaviour2 {

		public float Magnitude { get; set; }
		public float Frequency { get; set; }
		public float FrequencyOffset { get; set; }

		public BehaviourCircularAcceleration( float mag, float hz, float hzOffset ) {
			Magnitude = mag;
			Frequency = hz;
			FrequencyOffset = hzOffset;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			b.Acceleration += new Vector2( (float) Math.Cos( FrequencyOffset + p.System.Time * 3.1415f ) * Magnitude, (float) Math.Sin( FrequencyOffset + p.System.Time * 3.1415f ) * Magnitude );
		}
	}
}
