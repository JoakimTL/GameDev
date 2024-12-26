using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class Renderable2 : SceneObject<SceneObjectData2> {

		public override event RenderableTransformChangeHandler TransformChanged;

		public Renderable2( uint layer = 0, Material mat = null, Mesh mesh = null, ShaderBundle shaderBundle = null ) : base( new SceneObjectData2(), layer, mat, mesh, shaderBundle ) {
			Data.Transform.OnChangedEvent += TransformChangeHandler;
		}

		private void TransformChangeHandler() {
			TransformChanged?.Invoke( this );
		}

	}
}
