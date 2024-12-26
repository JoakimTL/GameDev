//using Engine.Graphics.Objects;
//using Engine.Graphics.Objects.Default.Transforms;
//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old {
//	public abstract class BaseModel<P, V> where P : PointMass<V> {

//		protected readonly List<BaseShape<P, V>> shapes;
//		public IReadOnlyList<BaseShape<P, V>> Shapes { get => shapes; }
//		public int ShapeCount { get => shapes.Count; }

//		public ReadOnly Readonly { get; }

//		public BaseModel() {
//			shapes = new List<BaseShape<P, V>>();
//			Readonly = new ReadOnly( this );
//		}

//		internal void Add( BaseShape<P, V> shape ) {
//			shapes.Add( shape );
//		}

//		public void ClearShapes() {
//			for( int i = 0; i < shapes.Count; i++ ) {
//				shapes[ i ].RemovedFromModel();
//			}
//			shapes.Clear();
//		}

//		public void UpdateShapes() {
//			for( int i = 0; i < shapes.Count; i++ ) {
//				shapes[ i ].UpdateShape();
//			}
//		}

//		public void Dispose() {
//			for( int i = 0; i < shapes.Count; i++ ) {
//				shapes[ i ].Dispose();
//			}
//		}

//		public class ReadOnly {

//			private BaseModel<P, V> model;
//			public IReadOnlyList<BaseShape<P, V>> Shapes { get => model.Shapes; }

//			public ReadOnly( BaseModel<P, V> model ) {
//				this.model = model;
//			}

//		}

//	}
//}
