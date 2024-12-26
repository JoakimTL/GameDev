using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using System;
using System.Collections.Generic;
using System.Text;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Cameras.Views {
	public class View2 : IView {
		public Orthographic.Dynamic Projection { get; private set; }
		private Transform2 transform;
		public TransformInterface<Vector2, float, Vector2> TranformInterface => transform.Interface;
		public Matrix4 PMatrix => Projection.Matrix;
		public Matrix4 IPMatrix => Projection.IMatrix;
		public Matrix4 VMatrix { get; private set; }
		public Matrix4 IVMatrix { get; private set; }
		public Matrix4 VPMatrix { get; private set; }
		public Matrix4 IVPMatrix { get; private set; }

		public View2( Orthographic.Dynamic projection, Transform2 transform ) {
			this.transform = transform;
			Projection = projection;
			UpdateMatrices();
		}

		public View2( GLWindow dw, float depth, Vector2 axisScale, float scale, Vector2 translation ) {
			transform = new Transform2 { Translation = translation };
			Projection = new Orthographic.Dynamic( "Camera2ViewOrthoProjection", dw, axisScale.X, axisScale.Y, -depth, depth, scale );
			UpdateMatrices();
		}

		public void UpdateMatrices() {
			{
				Matrix4 translation = Matrix4Factory.CreateTranslation( -( transform.GlobalTranslation.X ), -( transform.GlobalTranslation.Y ), 0 );
				Matrix4 rotation = Matrix4Factory.CreateRotationZ( -transform.GlobalRotation );
				Matrix4 scale = Matrix4Factory.CreateScale( 1f / transform.GlobalScale.X, 1f / transform.GlobalScale.Y, 0 );
				VMatrix = scale * rotation * translation;
				IVMatrix = Matrix4.Invert( VMatrix );
			}
			VPMatrix = VMatrix * PMatrix;
			IVPMatrix = Matrix4.Invert( VPMatrix );
		}
	}
}