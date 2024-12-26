//using Engine.Graphics.Objects;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old {
//	public class TransformedModel<P, V> where P : PointMass<V> {

//		public ITransform Transform { get; private set; }
//		public BaseModel<P, V>.ReadOnly Base { get; private set; }

//		public TransformedModel( BaseModel<P, V>.ReadOnly baseModel, ITransform transform ) {
//			Base = baseModel;
//			Transform = transform;
//		}
//		public TransformedModel( BaseModel<P, V> baseModel, ITransform transform ) {
//			Base = baseModel.Readonly;
//			Transform = transform;
//		}

//	}
//}
