using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	public class JunctionBindTexture : Engine.Pipelines.Junction {

		private TextureUnit textureUnit;
		private TextureTarget textureTarget;
		private uint textureId;

		public JunctionBindTexture( string name, TextureUnit textureUnit, uint textureId, TextureTarget textureTarget ) : base( name, null ) {
			this.textureUnit = textureUnit;
			this.textureTarget = textureTarget;
			this.textureId = textureId;
			Effect = Execute;
		}

		private void Execute() {
			Gl.ActiveTexture( textureUnit );
			Gl.BindTexture( textureTarget, textureId );
		}

	}
}
