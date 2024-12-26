using Engine.LinearAlgebra;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities.D3 {
	public class Force3Module : Module {

		public Vector3 Force { get; set; }

		public Force3Module( Vector3 force ) {
			Force = force;
		}

		protected override void Initialize() {

		}

		public override void Update( float time, float deltaTime ) {
			if( Entity.Get( out Mass3Module mm ) )
				mm.Mass.ApplyForce( Force );
		}
	}
}
