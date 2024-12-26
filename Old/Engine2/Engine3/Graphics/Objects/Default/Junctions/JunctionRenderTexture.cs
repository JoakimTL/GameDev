using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	public class JunctionRenderTexture<SHADER> : Engine.Pipelines.Junction where SHADER : Shader {

		private uint texture;

		public JunctionRenderTexture( string name, uint texture ) : base( name, null ) {
			this.texture = texture;

			Effect = Execute;
		}

		private void Execute() {
			Mem.Shaders.Get<SHADER>().Bind();
			Mem.Mesh2.Square.Bind();
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );
			Mem.Mesh2.Square.RenderMesh();
			Mem.Mesh2.Square.Unbind();
			Mem.Shaders.Get<SHADER>().Unbind();
		}
	}

	public class JunctionRenderTexture : JunctionRenderTexture<Surface2Shader> {
		public JunctionRenderTexture( string name, uint texture ) : base( name, texture ) {
		}
	}
}
