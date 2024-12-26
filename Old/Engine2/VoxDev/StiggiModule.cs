using Engine.Entities;
using Engine.Entities.D3;
using Engine.Graphics.Objects.Default.Transforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev {
	public class StiggiModule : Module {

		private Transform3 lol;

		public StiggiModule( Transform3 transform ) {
			lol = transform;
		}

		public override void Update( float time, float deltaTime ) {
			if( Entity.Get( out CustomRigidbodyModule crm ) ) {
				Entity.Get( out Transform3Module tm );

				crm.ApplyForce( lol.GlobalTranslation - tm.Transform.GlobalTranslation );
			}
		}

		protected override void Initialize() {

		}
	}
}
