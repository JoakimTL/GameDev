using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class GeometryBuffer : Framebuffer {
		public uint DiffuseTexture { get; private set; }

		public uint NormalTexture { get; private set; }

		public uint LightPropertiesTexture { get; private set; }

		public uint GlowTexture { get; private set; }

		public uint DepthTexture { get; private set; }

		public GeometryBuffer( string name, GLWindow window, float scale = 1 ) : base( name, window.Size, scale ) {
			BindToWindow( window, scale );
		}

		protected override void CreateFBO() {
			DiffuseTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			NormalTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgb16, PixelFormat.Rgb, PixelType.Int );
			SetFilter( minMagFiler: (int) TextureMagFilter.Nearest );

			LightPropertiesTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgb, PixelFormat.Rgb, PixelType.Float );
			SetFilter();

			GlowTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			DepthTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, DiffuseTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment1, NormalTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment2, LightPropertiesTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment3, GlowTexture );
			AttachTexture( FramebufferAttachment.DepthAttachment, DepthTexture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0, (int) ReadBufferMode.ColorAttachment1, (int) ReadBufferMode.ColorAttachment2, (int) ReadBufferMode.ColorAttachment3, (int) ReadBufferMode.ColorAttachment4 );

		}
	}
}
