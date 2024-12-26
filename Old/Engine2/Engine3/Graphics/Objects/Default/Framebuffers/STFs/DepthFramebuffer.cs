using Engine.Graphics.Objects.Default.Materials;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers.STFs {
	public class DepthFramebuffer : SingleTextureFramebuffer {

		public DepthFramebuffer( string name, Vector2i size ) : base( name, size ) { }
		public DepthFramebuffer( string name, Framebuffer buffer, float scale = 1 ) : base( name, buffer.Size ) {
			BindToBuffer( buffer, scale );
		}

		protected override void CreateFBO() {
			Texture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.R32f, PixelFormat.Red, PixelType.Float );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, Texture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0 );
		}
	}
}
