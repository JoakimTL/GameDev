using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public interface IView {
		Matrix4 PMatrix { get; }
		Matrix4 IPMatrix { get; }
		Matrix4 VMatrix { get; }
		Matrix4 IVMatrix { get; }
		Matrix4 VPMatrix { get; }
		Matrix4 IVPMatrix { get; }
		void UpdateMatrices();
	}
}
