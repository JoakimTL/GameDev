using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3 {
	public delegate void Behaviour3ApplicationHandler( Particle3 p, Behaviour3Manager m );
	public interface IBehaviour3 {
		void Apply( Particle3 p, Behaviour3Manager m );
	}

	public class Behaviour3Manager {

		private Particle3 p;

		public Vector3 Velocity { get; set; }
		public Vector3 Acceleration { get; set; }
		public float Momentum { get; set; }
		public float Torque { get; set; }
		public float Growth { get; set; }
		public float GrowthChange { get; set; }

		private HashSet<IBehaviour3> behaviours;

		public Behaviour3Manager( Particle3 p ) {
			this.p = p;
			behaviours = new HashSet<IBehaviour3>();
		}

		public void Update() {
			Acceleration = 0;
			Torque = 0;
			GrowthChange = 0;

			foreach( IBehaviour3 b in behaviours ) {
				b.Apply( p, this );
			}

			Velocity += Acceleration * p.System.DeltaTime;
			p.Data.Translation += Velocity * p.System.DeltaTime;
			Momentum += Torque * p.System.DeltaTime;
			p.Data.Rotation += Momentum * p.System.DeltaTime;
			Growth += GrowthChange * p.System.DeltaTime;
			p.Data.Scale += Growth * p.System.DeltaTime;
		}

		public void Add( IBehaviour3 behaviour ) {
			behaviours.Add( behaviour );
		}

	}
}
