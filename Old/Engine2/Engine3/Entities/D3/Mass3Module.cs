using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities.D3 {
	public class Mass3Module : Module {

		public Mass3 Mass { get; private set; }

		public Mass3Module( float massKg ) {
			Mass = new Mass3( massKg );
		}

		protected override void Initialize() {
			Entity.ModuleAdded += ModuleAdded;
			Entity.ModuleRemoved += ModuleRemoved;
			if( Entity.Get( out Collision3Module cm ) ) {
				Mass.SetModel( cm.Model );
			}
		}

		private void ModuleAdded( Entity e, Module m ) {
			if( m is Collision3Module cm ) {
				Mass.SetModel( cm.Model );
			}
		}

		private void ModuleRemoved( Entity e, Module m ) {
			if( m is Collision3Module )
				Mass.SetModel( null );
		}

		public override void Update( float time, float deltaTime ) {
			Mass.Update( time, deltaTime );
		}

		public override string ToString() {
			return Mass.ToString();
		}
	}
}
