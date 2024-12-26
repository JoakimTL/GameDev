using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class InstanceHandler<T, T2> : SceneObject where T : Instance where T2 : InstancedMesh {

		protected readonly int maxInstances;
		protected readonly int instanceSizeBytes;
		protected int activeInstances;

		private readonly BitSet indexManagement;
		protected readonly HashSet<T> instances;
		protected readonly List<T> deleted;
		protected T2 mesh;

		public InstanceHandler( T2 mesh, int maxInstances, int instanceSizeBytes, Shader instanceShader ) {
			this.mesh = mesh;
			this.maxInstances = maxInstances;
			this.instanceSizeBytes = instanceSizeBytes;
			indexManagement = new BitSet( maxInstances );
			instances = new HashSet<T>();
			deleted = new List<T>();
		}

		public void Update() {
			lock( instances ) {
				foreach( T instance in instances ) {
					if( !instance.Update() )
						deleted.Add( instance );
				}

				for( int i = 0; i < deleted.Count; i++ ) {
					instances.Remove( deleted[ i ] );
					indexManagement.Clear( deleted[ i ].Index );
				}

				deleted.Clear();
			}
			mesh.SetActiveInstances( instances.Count );
		}

		protected abstract bool CreationMethod( int index, out T o );

		public bool TryCreateInstance( out T o ) {
			o = default;
			if( indexManagement.Min == -1 )
				return false;
			lock( indexManagement ) {
				int ind = indexManagement.Min;
				if( CreationMethod( ind, out o ) ) {
					if( o is null )
						return false;
					indexManagement.Set( ind );
					lock( instances )
						instances.Add( o );
					mesh.GiveSubSegment( o, ind * instanceSizeBytes );
					return true;
				}
			}
			return false;
		}

	}
}
