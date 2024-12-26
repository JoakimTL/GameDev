//using Engine.Graphics.Objects;
//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;

//namespace Engine.Physics.Old {
//	public abstract class BaseShape<P, V> where P : PointMass<V> {

//		protected readonly BaseModel<P, V> model;

//		public BaseShape( BaseModel<P, V> model ) {
//			this.model = model;
//			model.Add( this );
//		}

//		public abstract void GetAABB( out V min, out V max );
//		public abstract V GetInitial();
//		public abstract V GetFurthest( V dir );
//		public abstract void GetClosest( V target, out V closest, out V nextClosest );
//		internal abstract void RemovedFromModel();
//		internal abstract void UpdateShape();
//		internal abstract void Dispose();
//	}
//}
