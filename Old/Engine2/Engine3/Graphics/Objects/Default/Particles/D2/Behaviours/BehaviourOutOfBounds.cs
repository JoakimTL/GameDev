using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2.Behaviours {
	public class BehaviourOutOfBounds : IBehaviour2 {
		public Vector2 LowerBounds { get; set; }
		public Vector2 UpperBounds { get; set; }
		public Behaviour2ApplicationHandler Action { get; set; }

		public BehaviourOutOfBounds( Vector2 lowerBounds, Vector2 upperBounds, Behaviour2ApplicationHandler action ) {
			LowerBounds = lowerBounds;
			UpperBounds = upperBounds;
			Action = action;
		}

		public void Apply( Particle2 p, Behaviour2Manager b ) {
			Vector2 min = p.Data.Translation - p.Data.Scale, max = p.Data.Translation + p.Data.Scale;
			if( min.X > UpperBounds.X || min.Y > UpperBounds.Y || max.X < LowerBounds.X || min.Y < LowerBounds.Y )
				Action?.Invoke( p, b );
		}
	}
}
