using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public class InstanceSystemManager<D> where D : SceneObjectData {

		private readonly HashSet<InstanceHandler<D>> instanceHandlers;

		public InstanceSystemManager() {
			instanceHandlers = new HashSet<InstanceHandler<D>>();
		}

		public void Add( InstanceHandler<D> ih ) {
			instanceHandlers.Add( ih );
		}

		public void Remove( InstanceHandler<D> ih ) {
			instanceHandlers.Remove( ih );
		}

		public void Update() {
			foreach( InstanceHandler<D> ih in instanceHandlers )
				ih.Update();
		}

	}
}
