using Engine.Graphics.Objects.Default.Materials;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers.STFs {
	public class DepthBlurFramebuffer : SingleTextureFramebuffer {

		public DepthBlurFramebuffer( string name, Framebuffer buffer, float scale ) : base( name, buffer.Size ) {
			BindToBuffer( buffer, scale );
		}

		protected override void CreateFBO() {
			Texture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rg32f, PixelFormat.Rg, PixelType.Float );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, Texture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0 );
		}
	}
}
