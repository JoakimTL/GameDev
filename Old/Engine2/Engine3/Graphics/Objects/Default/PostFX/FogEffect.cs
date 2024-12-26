using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.PostFX {
	public static class FogEffect {
		public static void Fog( uint depthTexture, float zNear, float zFar, float depth, LinearAlgebra.Vector3 color, Vector2i size, Framebuffer output ) {
			output.SetSize( size );
			Mem.Mesh2.Square.Bind();
			output.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			FogShader shader = Mem.Shaders.Get<FogShader>();
			shader.Bind();
			shader.Set( "zNear", zNear );
			shader.Set( "zFar", zFar );
			shader.Set( "uDepth", depth );
			shader.Set( "uColor", color );

			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, depthTexture );

			Mem.Mesh2.Square.RenderMesh();
		}
	}
}
