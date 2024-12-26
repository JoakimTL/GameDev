using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Data {
	public delegate float InterpolationMethod( float t );
	public static class InterpolationMethods {

		public static float LinearInterpolation( float t ) {
			return t;
		}

		public static float CosineInterpolation( float t ) {
			return (float) ( 1 - Math.Cos( t * Math.PI ) ) * 0.5f;
		}
	}
}
