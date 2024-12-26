using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public delegate void Behaviour2ApplicationHandler( Particle2 p, Behaviour2Manager m );
	public interface IBehaviour2 {
		void Apply( Particle2 p, Behaviour2Manager m );
	}

	public class Behaviour2Manager {

		private Particle2 p;

		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; }
		public float Momentum { get; set; }
		public float Torque { get; set; }
		public float Growth { get; set; }
		public float GrowthChange { get; set; }

		private HashSet<IBehaviour2> behaviours;

		public Behaviour2Manager( Particle2 p ) {
			this.p = p;
			behaviours = new HashSet<IBehaviour2>();
		}

		public void Update() {
			Acceleration = 0;
			Torque = 0;
			GrowthChange = 0;

			foreach( IBehaviour2 b in behaviours ) {
				b.Apply( p, this );
			}

			Velocity += Acceleration * p.System.DeltaTime;
			p.Data.Translation += Velocity * p.System.DeltaTime;
			Momentum += Torque * p.System.DeltaTime;
			p.Data.Rotation += Momentum * p.System.DeltaTime;
			Growth += GrowthChange * p.System.DeltaTime;
			p.Data.Scale += Growth * p.System.DeltaTime;
		}

		public void Add( IBehaviour2 behaviour ) {
			behaviours.Add( behaviour );
		}

	}
}
