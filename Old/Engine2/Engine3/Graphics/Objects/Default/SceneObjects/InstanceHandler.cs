using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class InstanceHandler<D> : SceneObject<D> where D : SceneObjectData {
		protected InstanceHandler( D data ) : base( data ) { }

		internal abstract void Update();
	}

	public abstract class InstanceHandlerDatabuffered<T, T2, D> : InstanceHandler<D> where T : InstanceDatabuffered where T2 : InstancedMesh where D : SceneObjectData {

		protected readonly int maxInstances;
		protected readonly int instanceSizeBytes;

		protected readonly HashSet<T> instances;
		private readonly List<T> deleted;
		protected T2 mesh;
		private bool sharedMesh;

		public InstanceHandlerDatabuffered( D data, T2 mesh, int maxInstances, int instanceSizeBytes, bool sharedMesh ) : base( data ) {
			Mesh = mesh;
			this.mesh = mesh;
			this.maxInstances = maxInstances;
			this.instanceSizeBytes = instanceSizeBytes;
			instances = new HashSet<T>();
			deleted = new List<T>();
			this.sharedMesh = sharedMesh;
		}

		internal override void Update() {
			PreUpdate();
			lock( instances ) {
				foreach( T instance in instances ) {
					if( !instance.Update() )
						deleted.Add( instance );
				}

				for( int i = 0; i < deleted.Count; i++ ) {
					instances.Remove( deleted[ i ] );
					InstanceRemoved( deleted[ i ] );
				}
				deleted.Clear();
			}
			BufferData();
			mesh.SetActiveInstances( instances.Count );
		}

		protected override void OnDispose() {
			if( !sharedMesh )
				mesh.Dispose();
		}

		protected abstract void BufferData();
		protected abstract void PreUpdate();
		protected abstract void CreationMethod( out T o );

		protected virtual void InstanceAdded( T t ) { }
		protected virtual void InstanceRemoved( T t ) { }

		protected bool TryCreateInstance( out T o ) {
			o = default;
			if( instances.Count >= maxInstances )
				return false;
			CreationMethod( out o );
			if( o is null )
				return false;
			lock( instances )
				instances.Add( o );
			InstanceAdded( o );
			return true;
		}

	}
}
