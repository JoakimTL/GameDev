using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class ShadowBuffer : Framebuffer {
		public uint DiffuseTexture { get; private set; }
		public uint DepthTexture { get; private set; }

		public ShadowBuffer( int width, int height ) : base( "2d Shadow Buffer", (width, height) ) { }

		protected override void CreateFBO() {
			DepthTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rg32f, PixelFormat.Rg, PixelType.Int );
			SetFilter( wrapMode: (int) TextureWrapMode.ClampToBorder );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureBorderColor, new float[] { 1f, 1f } );

			DiffuseTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter( wrapMode: (int) TextureWrapMode.ClampToBorder );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureBorderColor, new float[] { 0F, 0F, 0F, 0F } );

			GenerateDepthStencilbuffer();
			Gl.BindRenderbuffer( RenderbufferTarget.Renderbuffer, DepthStencilBuffer );
			Gl.RenderbufferStorage( RenderbufferTarget.Renderbuffer, InternalFormat.DepthStencil, Size.X, Size.Y );
			Gl.FramebufferRenderbuffer( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, DepthStencilBuffer );

			AttachTexture( FramebufferAttachment.ColorAttachment0, DepthTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment1, DiffuseTexture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0, (int) ReadBufferMode.ColorAttachment1, (int) ReadBufferMode.ColorAttachment2 );

		}
	}
}
