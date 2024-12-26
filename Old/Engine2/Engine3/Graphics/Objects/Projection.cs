using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public class Projection {

		public Matrix4 Matrix { get; private set; }
		public Matrix4 IMatrix { get; private set; }
		public string Name { get; private set; }

		public Projection( string name, Matrix4 matrix ) {
			Name = name;
			Matrix = matrix;
			IMatrix = Matrix4.Invert( matrix );
		}

		public void SetMatrix( Matrix4 mat ) {
			Matrix = mat;
			IMatrix = Matrix4.Invert( mat );
		}

	}
}
