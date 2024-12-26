using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.PostFX {
	public class OutlineEffect {
		public static void Outline( uint depthTexture, uint normalTexture, float zNear, float zFar, float normalThreshold, float depthThreshold, float range, Vector3 color, float searchScale, Vector2i size, Framebuffer output ) {
			if( !( output is null ) ) {
				Framebuffer.Unbind( FramebufferTarget.DrawFramebuffer );
				output.SetSize( size );
			}
			Mem.Mesh2.Square.Bind();
			if( !( output is null ) ) {
				output.Bind( FramebufferTarget.DrawFramebuffer );
				Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			}

			OutlineShader shader = Mem.Shaders.Get<OutlineShader>();
			shader.Bind();
			shader.Set( "uNearZ", zNear );
			shader.Set( "uFarZ", zFar );
			shader.Set( "uNorThreshold", normalThreshold );
			shader.Set( "uDepThreshold", depthThreshold );
			shader.Set( "uOutputSize", searchScale / size.AsFloat );
			shader.Set( "uRange", range );
			shader.Set( "uColor", color );

			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, depthTexture );
			Gl.ActiveTexture( TextureUnit.Texture1 );
			Gl.BindTexture( TextureTarget.Texture2d, normalTexture );

			Mem.Mesh2.Square.RenderMesh();
			if( !( output is null ) )
				Framebuffer.Unbind( FramebufferTarget.DrawFramebuffer );
		}
	}
}
