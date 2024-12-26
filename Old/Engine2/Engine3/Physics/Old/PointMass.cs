//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old {
//	public class PointMass<V> {
//		/// <summary>
//		/// Is this point collidable? Points set to false will not be used in collision detection, but will apply it's mass to the object.
//		/// </summary>
//		public bool Collidable { get; private set; }
//		public V Offset { get; private set; }
//		public float Weight { get; private set; }

//		public PointMass( V offset, float weight, bool collides = true ) {
//			Collidable = collides;
//			Offset = offset;
//			Weight = weight;
//		}
//	}
//}
