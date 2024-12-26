using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3.Behaviours {
	public class Behaviour3Timed : IBehaviour3 {

		public float Start { get; set; }
		public float End { get; set; }

		public delegate void TimedActionHandler( Particle3 p, Behaviour3Manager b, float t );
		public TimedActionHandler TimedAction;

		public Behaviour3Timed( float start, float end, TimedActionHandler timedAction ) {
			Start = start;
			End = end;
			TimedAction = timedAction;
		}

		public void Apply( Particle3 p, Behaviour3Manager b ) {
			if( p.Lifetime >= Start && p.Lifetime < End ) {
				float t = ( p.Lifetime - Start ) / ( End - Start );
				TimedAction?.Invoke( p, b, t );
			}
		}
	}
}
