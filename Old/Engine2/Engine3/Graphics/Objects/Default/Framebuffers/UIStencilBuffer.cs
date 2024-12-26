using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class UIStencilBuffer : Framebuffer {

		public uint Texture {
			get; protected set;
		}

		public UIStencilBuffer( string name, GLWindow window, float scale ) : base( name, window.Size, scale ) {
			BindToWindow( window, scale );
		}

		protected override void CreateFBO() {
			Texture = CreateAndBindTexture();
			Gl.TexImage2D( TextureTarget.Texture2d, 0, InternalFormat.R8, Size.X, Size.Y, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge );

			GenerateDepthStencilbuffer();
			Gl.BindRenderbuffer( RenderbufferTarget.Renderbuffer, DepthStencilBuffer );
			Gl.RenderbufferStorage( RenderbufferTarget.Renderbuffer, InternalFormat.DepthStencil, Size.X, Size.Y );
			Gl.FramebufferRenderbuffer( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, DepthStencilBuffer );

			Gl.FramebufferTexture( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Texture, 0 );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0 );
		}
	}
}
