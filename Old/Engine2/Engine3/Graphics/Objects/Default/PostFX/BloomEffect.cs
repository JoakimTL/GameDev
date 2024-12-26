using Engine.Graphics.Objects.Default.Framebuffers.STFs;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Settings;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.PostFX {
	public class BloomEffect {

		private DiffuseFramebuffer tempBuffer;
		private BlurEffect blurEffect;

		private float threshold;
		private float intensity;
		private float blurIntensity;
		private float gaussScale;
		private float bufferScaling;
		private int blurSteps;
		private Vector3 colorWeights;

		private SettingsFile bloomSettings;

		public BloomEffect( Vector3 colorWeights ) {
			bloomSettings = Mem.Settings.Add( "bloom",
				new Setting( "Threshold", 0.6f ),
				new Setting( "Intensity", 0.8f ),
				new Setting( "BlurDiffuseIntensity", 0.1f ),
				new Setting( "BlurScaleIntensity", 1.75f ),
				new Setting( "BlurResolutionScale", 0.5f ),
				new Setting( "BlurSteps", 1 )
				);
			bloomSettings.SettingsChanged += BloomSettingsChanged;
			this.colorWeights = colorWeights;
			bloomSettings.TryGet( "Threshold", out threshold );
			bloomSettings.TryGet( "Intensity", out intensity );
			bloomSettings.TryGet( "BlurDiffuseIntensity", out blurIntensity );
			bloomSettings.TryGet( "BlurScaleIntensity", out gaussScale );
			bloomSettings.TryGet( "BlurResolutionScale", out bufferScaling );
			bloomSettings.TryGet( "BlurSteps", out blurSteps );

			tempBuffer = new DiffuseFramebuffer( "Bloom Temporary Buffer", 1 );
			blurEffect = new BlurEffect( new DiffuseFramebuffer( "Bloom Blur Buffer", tempBuffer, 1 ) );
		}

		private void BloomSettingsChanged() {
			bloomSettings.TryGet( "Threshold", out threshold );
			bloomSettings.TryGet( "Intensity", out intensity );
			bloomSettings.TryGet( "BlurDiffuseIntensity", out blurIntensity );
			bloomSettings.TryGet( "BlurScaleIntensity", out gaussScale );
			bloomSettings.TryGet( "BlurResolutionScale", out bufferScaling );
			bloomSettings.TryGet( "BlurSteps", out blurSteps );
		}

		public void Bloom( uint texture, Vector2i size, Framebuffer output ) {
			tempBuffer.SetSize( ( size * bufferScaling ).IntRounded );
			tempBuffer.Bind( FramebufferTarget.DrawFramebuffer );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			Mem.Shaders.Get<BloomShader>().Bind();
			Mem.Shaders.Get<BloomShader>().Set( threshold, intensity, colorWeights );

			Gl.ActiveTexture( TextureUnit.Texture0 );
			Gl.BindTexture( TextureTarget.Texture2d, texture );

			Mem.Mesh2.Square.Bind();
			Mem.Mesh2.Square.RenderMesh();
			Mem.Mesh2.Square.Unbind();

			Mem.Shaders.Get<BloomShader>().Unbind();
			Framebuffer.Unbind( FramebufferTarget.DrawFramebuffer );

			for( int i = 0; i < blurSteps - 1; i++ ) {
				blurEffect.BlurBloom( tempBuffer.Texture, tempBuffer.Size, gaussScale, blurIntensity, true, tempBuffer );
			}
			blurEffect.BlurBloom( tempBuffer.Texture, tempBuffer.Size, gaussScale, blurIntensity, true, output );
		}
	}
}
