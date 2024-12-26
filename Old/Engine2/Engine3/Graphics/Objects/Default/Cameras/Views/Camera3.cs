using Engine.Graphics.Objects.Default.Transforms;
using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using Engine.Graphics.Objects.Default.Cameras.Projections;

namespace Engine.Graphics.Objects.Default.Cameras.Views {
	public class Camera3 : IView {
		public Perspective.Dynamic PerspectiveProjection { get; private set; }
		private Transform3 transform;
		public TransformInterface<Vector3, Quaternion, Vector3> TranformInterface => transform.Interface;
		public Matrix4 PMatrix => PerspectiveProjection.Matrix;
		public Matrix4 IPMatrix => PerspectiveProjection.IMatrix;
		public Matrix4 VMatrix { get; private set; }
		public Matrix4 IVMatrix { get; private set; }
		public Matrix4 VPMatrix { get; private set; }
		public Matrix4 IVPMatrix { get; private set; }

		public Camera3( Perspective.Dynamic projection, Transform3 transform ) {
			this.transform = transform;
			PerspectiveProjection = projection;
		}

		public bool HasParent { get => !( transform.Parent is null ); }

		public void SetParent( Transform3 transform ) {
			this.transform.SetParent( transform );
		}

		public void UpdateMatrices() {
			{
				Matrix4 translation = Matrix4Factory.CreateTranslation( -transform.GlobalTranslation );
				Matrix4 rotation = Matrix4Factory.CreateFromQuaternion( Quaternion.Conjugate( transform.GlobalRotation ) );
				VMatrix = translation * rotation;
				IVMatrix = Matrix4.Invert( VMatrix );
			}
			VPMatrix = VMatrix * PMatrix;
			IVPMatrix = Matrix4.Invert( VPMatrix );
		}
	}
}
