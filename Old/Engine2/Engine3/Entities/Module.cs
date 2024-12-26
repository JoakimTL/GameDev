using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {
	public abstract class Module {

		protected Entity Entity { get; private set; }

		internal bool SetOwner( Entity e ) {
			if( Entity is null ) {
				Entity = e;
				Initialize();
				return true;
			}
			return false;
		}

		protected abstract void Initialize();
		public abstract void Update( float time, float deltaTime );

	}
}
