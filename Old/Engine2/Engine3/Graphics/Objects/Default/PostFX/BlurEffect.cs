using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.PostFX {
	public class BlurEffect {

		private SingleTextureFramebuffer tempBuffer;

		public BlurEffect( SingleTextureFramebuffer buffer ) {
			tempBuffer = buffer;
		}

		public void Blur<SHADER>( uint texture, int width, int height, float blurAmount, bool resizeOutput, Framebuffer output ) where SHADER : Shader {
			tempBuffer.SetSize( (width, height) );
			Mem.Mesh2.Square.Bind();
			tempBuffer.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			Mem.Shaders.Get<SHADER>().Bind();
			Mem.Shaders.Get<SHADER>().Set( "tSize", new Vector2( width, height ) );
			Mem.Shaders.Get<SHADER>().Set( "uBlur", new Vector2( blurAmount, 0 ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );

			Mem.Mesh2.Square.RenderMesh();

			if( resizeOutput )
				output.SetSize( tempBuffer.Size );
			output.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			Mem.Shaders.Get<SHADER>().Set( "uBlur", new Vector2( 0, blurAmount ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, tempBuffer.Texture );

			Mem.Mesh2.Square.RenderMesh();
			Mem.Shaders.Get<SHADER>().Unbind();

			Mem.Mesh2.Square.Unbind();

		}

		public void BlurDefault( uint texture, int width, int height, float blurAmount, bool resizeOutput, Framebuffer output ) => Blur<GaussianBlurDefaultShader>( texture, width, height, blurAmount, resizeOutput, output );

		/*public void BlurRadial( uint texture, int width, int height, float blurAmount, Vector2 point, bool resizeOutput, FrameBuffer output ) {
			if( !( output is null ) )
				if( resizeOutput )
					output.SetSize( tBuffer.Width, tBuffer.Height );
			MemLib.Ezmem.Mesh2.Square.Bind();
			if( !( output is null ) )
				output.Bind( FramebufferTarget.DrawFramebuffer, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			MemLib.Ezmem.Shaders.Get<RadialBlurShader>().Bind();
			MemLib.Ezmem.Shaders.Get<RadialBlurShader>().Set( "tSize", new Vector2( width, height ) );
			MemLib.Ezmem.Shaders.Get<RadialBlurShader>().Set( "uBlur", blurAmount );
			MemLib.Ezmem.Shaders.Get<RadialBlurShader>().Set( "uBlurCenter", point );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );

			MemLib.Ezmem.Mesh2.Square.RenderMesh();
			MemLib.Ezmem.Shaders.Get<RadialBlurShader>().Unbind();
			if( !( output is null ) )
				FrameBuffer.Unbind();
		}*/

		public void BlurBloom( uint texture, Vector2i size, float blurAmount, float intensity, bool resizeOutput, Framebuffer output ) {
			tempBuffer.SetSize( size );
			Mem.Mesh2.Square.Bind();
			tempBuffer.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			Mem.Shaders.Get<GaussianBlurBloomShader>().Bind();
			Mem.Shaders.Get<GaussianBlurBloomShader>().Set( "tSize", size.AsFloat );
			Mem.Shaders.Get<GaussianBlurBloomShader>().Set( "uIntensity", intensity );
			Mem.Shaders.Get<GaussianBlurBloomShader>().Set( "uBlur", new Vector2( blurAmount, 0 ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );

			Mem.Mesh2.Square.RenderMesh();

			if( resizeOutput )
				output.SetSize( tempBuffer.Size );
			output.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			Mem.Shaders.Get<GaussianBlurBloomShader>().Set( "uBlur", new Vector2( 0, blurAmount ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, tempBuffer.Texture );

			Mem.Mesh2.Square.RenderMesh();
			Mem.Shaders.Get<GaussianBlurBloomShader>().Unbind();

			Mem.Mesh2.Square.Unbind();
			Framebuffer.Unbind(FramebufferTarget.DrawFramebuffer);

		}


		public void BlurShadow( uint texture, int width, int height, float blurAmount, Framebuffer output ) => Blur<GaussianBlurShadowShader>( texture, width, height, blurAmount, false, output );
		public void BlurParticleShadow( uint texture, int width, int height, float blurAmount, Framebuffer output ) => Blur<GaussianBlurParticleShadowShader>( texture, width, height, blurAmount, false, output );

		/*public void BlurShadowCube( uint texture, int width, int height, float blurAmount, FrameBuffer output, TextureTarget target, uint targetTexture ) {
			tBuffer.SetSize( width, height );
			MemLib.Ezmem.Mesh2.Square.Bind();
			output.BindRead( target, texture );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.TextureCubeMap, texture );
			tBuffer.Bind( FramebufferTarget.DrawFramebuffer, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			MemLib.Ezmem.Shaders.Get<GaussianBlurShadowShader>().Bind();
			MemLib.Ezmem.Shaders.Get<GaussianBlurShadowShader>().Set( "tSize", new Vector2( width, height ) );
			MemLib.Ezmem.Shaders.Get<GaussianBlurShadowShader>().Set( "uBlur", new Vector2( blurAmount, 0 ) );

			MemLib.Ezmem.Mesh2.Square.RenderMesh();
			FrameBuffer.Unbind();

			output.BindDraw( target, targetTexture );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			MemLib.Ezmem.Shaders.Get<GaussianBlurShadowShader>().Set( "uBlur", new Vector2( 0, blurAmount ) );
			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, tBuffer.DiffuseTexture );

			MemLib.Ezmem.Mesh2.Square.RenderMesh();
			MemLib.Ezmem.Shaders.Get<GaussianBlurShadowShader>().Unbind();

			MemLib.Ezmem.Mesh2.Square.Unbind();
			FrameBuffer.Unbind();
		}*/
	}
}
