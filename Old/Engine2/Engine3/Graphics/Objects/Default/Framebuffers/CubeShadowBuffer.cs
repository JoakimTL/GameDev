using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public class CubeShadowBuffer : Framebuffer {
		public uint DepthTexture { get; private set; }

		public uint CubeDiffuseMap { get; private set; }
		public uint CubeDepthMap { get; private set; }

		public CubeShadowBuffer( int width, int height ) : base( "3d Shadow Buffer", (width, height) ) {
		}


		protected override void CreateFBO() {

			DepthTexture = CreateAndBindTexture( TextureTarget.Texture2d, InternalFormat.DepthComponent24, PixelFormat.DepthComponent, PixelType.Float );
			Gl.TexImage2D( TextureTarget.Texture2d, 0, InternalFormat.DepthComponent32, Size.X, Size.Y, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero );

			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToBorder );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToBorder );
			Gl.TexParameter( TextureTarget.Texture2d, TextureParameterName.TextureBorderColor, new float[] { 1f, 1f, 1f, 1f } );

			CubeDepthMap = CreateAndBindTexture( TextureTarget.TextureCubeMap );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int) TextureWrapMode.ClampToEdge );

			for( uint i = 0; i < 6; i++ ) {
				Gl.TexImage2D( (TextureTarget) ( (int) TextureTarget.TextureCubeMapPositiveX + i ), 0, InternalFormat.Rg16, Size.X, Size.Y, 0, PixelFormat.Rg, PixelType.UnsignedShort, IntPtr.Zero );
			}

			CubeDiffuseMap = CreateAndBindTexture( TextureTarget.TextureCubeMap );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge );
			Gl.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int) TextureWrapMode.ClampToEdge );

			for( uint i = 0; i < 6; i++ ) {
				Gl.TexImage2D( (TextureTarget) ( (int) TextureTarget.TextureCubeMapPositiveX + i ), 0, InternalFormat.Rgba, Size.X, Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero );
			}

			Gl.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, DepthTexture, 0 );

			AttachTexture( FramebufferAttachment.ColorAttachment0, CubeDepthMap );
			AttachTexture( FramebufferAttachment.ColorAttachment1, CubeDiffuseMap );

			//SetDrawbuffers( (int) ReadBufferMode.ColorAttachment0, (int) ReadBufferMode.ColorAttachment1 );

			/*	// depth cube map
				glGenTextures(1, &tDepthCubeMap);
				glBindTexture(GL_TEXTURE_CUBE_MAP, tDepthCubeMap);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
				for (uint face = 0; face < 6; face++) {
					glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + face, 0, GL_DEPTH_COMPONENT24,
						width, height, 0, GL_DEPTH_COMPONENT, GL_FLOAT, null);
				}

				// color cube map
				glGenTextures(1, &tColorCubeMap);
				glBindTexture(GL_TEXTURE_CUBE_MAP, tColorCubeMap);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
				glTexParameterf(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
				for (uint face = 0; face < 6; face++) {
					glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + face, 0, GL_RGBA,
						width, height, 0, GL_RGBA, GL_FLOAT, null);
				}

				// framebuffer object
				glGenFramebuffersEXT(1, &fbo);
				glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, fbo);
				glFramebufferTextureARB(GL_FRAMEBUFFER_EXT, GL_DEPTH_ATTACHMENT_EXT, tDepthCubeMap, 0);
				glFramebufferTextureARB(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, tColorCubeMap, 0);

				glDrawBuffer(GL_COLOR_ATTACHMENT0_EXT);

				if (!isValidFBO()) {
					glDeleteFramebuffersEXT(1, &fbo);
					fbo = 0;
				}
			 */
		}
	}
}
