using Engine.Graphics.Objects.Default.Transforms;
using Engine.Graphics.Objects;
using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Cameras.Views {
	public class View3 : IView {
		public Projection Projection { get; private set; }
		private Transform3 transform;
		public TransformInterface<Vector3, Quaternion, Vector3> TranformInterface => transform.Interface;
		public Matrix4 PMatrix => Projection.Matrix;
		public Matrix4 IPMatrix => Projection.IMatrix;
		public Matrix4 VMatrix { get; private set; }
		public Matrix4 IVMatrix { get; private set; }
		public Matrix4 VPMatrix { get; private set; }
		public Matrix4 IVPMatrix { get; private set; }

		public View3( Projection projection, Transform3 transform ) {
			this.transform = transform;
			Projection = projection;
		}

		public void UpdateMatrices() {
			{
				Matrix4 translation = Matrix4Factory.CreateTranslation( -transform.GlobalTranslation );
				Matrix4 rotation = Matrix4Factory.CreateFromQuaternion( Quaternion.Conjugate( transform.GlobalRotation ) );
				VMatrix = translation * rotation;
			}
			VPMatrix = VMatrix * PMatrix;
			IVPMatrix = Matrix4.Invert( VPMatrix );
		}
	}
}
