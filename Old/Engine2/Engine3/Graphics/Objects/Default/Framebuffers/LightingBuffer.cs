using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class LightingBuffer : Framebuffer {

		private GeometryBuffer gBuffer;
		public uint Texture { get; private set; }

		public LightingBuffer( string name, GeometryBuffer gBuffer, float scale = 1 ) : base( name ) {
			this.gBuffer = gBuffer;
			BindToBuffer( gBuffer, scale );
		}

		protected override void CreateFBO() {
			Texture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte );
			SetFilter();

			AttachTexture( FramebufferAttachment.ColorAttachment0, Texture );

			SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0 );
		}
	}
}
