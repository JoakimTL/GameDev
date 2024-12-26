using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class Renderable3 : SceneObject<SceneObjectData3> {

		public override event RenderableTransformChangeHandler TransformChanged;

		public Renderable3( uint layer = 0, Material mat = null, Mesh mesh = null, ShaderBundle shaderBundle = null ) : base( new SceneObjectData3(), layer, mat, mesh, shaderBundle ) {
			Data.TransformObject.OnChangedEvent += TransformChangeHandler;
		}

		private void TransformChangeHandler() {
			TransformChanged?.Invoke( this );
		}

	}
}
