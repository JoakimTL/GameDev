using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class Renderable : SceneObject {

		public Vector4 Color { get; protected set; } = Vector4.One;
		public abstract Matrix4 TransformationMatrix { get; }

		public Renderable( uint layer = 0, Material mat = null, Mesh mesh = null, Shader shader = null ) : base( layer, mat, mesh, shader ) {

		}


		public void SetRenderFunction( IView view ) {
			RenderFunction = () => {
				Shader.Set( "uMVP_mat", TransformationMatrix * view.VPMatrix );
				Shader.Set( "uM_mat", TransformationMatrix );
				Shader.Set( "uVP_mat", view.VPMatrix );
				Shader.Set( "uColor", Color );
				Mesh.RenderMesh();
			};
		}

	}
}
