using Engine.Graphics.Objects.Default.Transforms;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities.D3 {
	public class Collision3Module : Module {

		public Physics3Model Model { get; private set; }

		public Collision3Module( Physics3Model model ) {
			Model = model;
		}

		public Collision3Module() {
			Model = null;
		}

		protected override void Initialize() {
			Model = new Physics3Model( "Collision3Module for " + Entity.Name, new Transform3() );
			Entity.ModuleAdded += ModuleAdded;
			Entity.ModuleRemoved += ModuleRemoved;
			if( Entity.Get( out Transform3Module tm ) ) {
				Model.SetTransform( tm.Transform );
			}
		}

		private void ModuleAdded( Entity e, Module m ) {
			if( m is Transform3Module tm ) {
				Model.SetTransform( tm.Transform );
			}
		}

		private void ModuleRemoved( Entity e, Module m ) {
			if( m is Transform3Module )
				Model.SetTransform( null );
		}

		public override void Update( float time, float deltaTime ) {
			//woaw
		}

		public override string ToString() {
			return Model.ToString();
		}
	}
}
