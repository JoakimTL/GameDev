using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {
	public class EntityManagerUpdater {

		private List<EntityManager> managers;

		private float prevTime;
		public Clock32 Clock { get; private set; }

		public EntityManagerUpdater( Clock32 clock ) {
			Clock = clock;
			managers = new List<EntityManager>();
		}

		public void UpdateManagers() {
			float time = Clock.Time;
			float deltaTime = time - prevTime;
			for( int i = 0; i < managers.Count; i++ )
				managers[ i ].Update( time, deltaTime );
			prevTime = time;
		}

		public void Add( EntityManager em ) {
			managers.Add( em );
		}

	}
}
