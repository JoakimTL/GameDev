using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers.STFs {
	public class DiffuseFramebuffer : SingleTextureFramebuffer {

		public DiffuseFramebuffer( string name, Vector2i size ) : base( name, size ) { }

		public DiffuseFramebuffer( string name, GLWindow window, float scale ) : base( name, window.Size ) {
			BindToWindow( window, scale );
		}

		public DiffuseFramebuffer( string name, Framebuffer buffer, float scale ) : base( name, buffer.Size ) {
			BindToBuffer( buffer, scale );
		}

		protected override void CreateFBO() {
			Texture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, Texture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0 );
		}
	}
}
