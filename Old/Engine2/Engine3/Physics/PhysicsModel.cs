
using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics {
	public abstract class PhysicsModel<T, V> where T : Transform {

		public string Name { get; }

		public T Transform { get; protected set; }

		private readonly List<PhysicsShape<T, V>> shapes;
		public IReadOnlyList<PhysicsShape<T, V>> Shapes { get => shapes; }
		public int ShapeCount { get => shapes.Count; }

		public abstract V MinAABB { get; }
		public abstract V MaxAABB { get; }

		public PhysicsModel( string name, T transform ) {
			Name = name;
			Transform = transform;
			shapes = new List<PhysicsShape<T, V>>();
		}

		public void SetTransform( T transform ) {
			if( transform is null )
				return;
			Transform = transform;
		}

		public void UpdateShapes() {
			bool updated = false;
			for( int i = 0; i < shapes.Count; i++ )
				if( shapes[ i ].UpdateShape() )
					updated = true;
			if( updated )
				UpdateAABB();
		}


		public void Add( PhysicsShape<T, V> shape ) {
			shapes.Add( shape );
			shape.Added( this );
		}

		public void Clear() {
			for( int i = 0; i < shapes.Count; i++ )
				shapes[ i ].Removed( this );
			shapes.Clear();
		}

		public abstract V GetCenterOfMass();
		public abstract void UpdateAABB();
		public void GetAABB( out V min, out V max ) {
			min = MinAABB;
			max = MaxAABB;
		}
	}
}
