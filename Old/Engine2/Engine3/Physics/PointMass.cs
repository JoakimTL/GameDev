using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics {
	public struct PointMass<V> where V : struct {
		public V Offset { get; private set; }
		/// <summary>
		/// The proportion of mass centered around this point. This is an arbitrary number, but it's size compared to the other points is important.<br></br>
		/// If all points have the same massproportion, e.g. 1, the mass is uniform across the shape. If one is unbalanced the center of mass shifts
		/// </summary>
		public float MassProportion { get; private set; }

		public PointMass( V offset, float massProp ) {
			Offset = offset;
			MassProportion = massProp;
		}
	}
}
