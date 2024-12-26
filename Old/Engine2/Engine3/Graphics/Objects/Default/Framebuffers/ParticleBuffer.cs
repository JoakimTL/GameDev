using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class ParticleBuffer : Framebuffer {

		private GeometryBuffer gBuffer;

		public uint DiffuseTexture { get; private set; }

		public uint GlowTexture { get; private set; }

		public uint ParticleDepthTexture { get; private set; }

		public ParticleBuffer( string name, GeometryBuffer gBuffer, float scale = 1 ) : base( name ) {
			this.gBuffer = gBuffer;
			BindToBuffer( gBuffer, scale );
		}

		protected override void CreateFBO() {
			DiffuseTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			GlowTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			ParticleDepthTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.R32f, PixelFormat.Red, PixelType.Float );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, DiffuseTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment1, GlowTexture );
			AttachTexture( FramebufferAttachment.ColorAttachment2, ParticleDepthTexture );
			AttachTexture( FramebufferAttachment.DepthAttachment, gBuffer.DepthTexture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0, (int) ReadBufferMode.ColorAttachment1, (int) ReadBufferMode.ColorAttachment2 );

		}
	}
}
