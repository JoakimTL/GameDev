using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.PostFX {
	public static class SummedAreaVariance {
		public static void SAV( uint texture, int width, int height, float blurAmount, bool resizeOutput, Framebuffer output ) {
			Mem.Mesh2.Square.Bind();
			output.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			Mem.Shaders.Get<SummedAreaVarianceShader>().Bind();
			Mem.Shaders.Get<SummedAreaVarianceShader>().Set( "tSize", new Vector2( width, height ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );
			Mem.Mesh2.Square.RenderMesh();
			Mem.Shaders.Get<SummedAreaVarianceShader>().Unbind();
			Mem.Mesh2.Square.Unbind();

		}
	}
}
