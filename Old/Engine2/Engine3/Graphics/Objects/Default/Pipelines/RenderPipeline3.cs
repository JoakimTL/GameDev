using Engine.GLFrameWork;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.Graphics.Objects.Default.Framebuffers.STFs;
using Engine.Graphics.Objects.Default.Junctions;
using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.Graphics.Objects.Default.PostFX;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Pipelines;
using Engine.QuickstartKit;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Graphics.Objects.Default.Pipelines {
	public class RenderPipeline3 : Pipeline {

		public GLWindow Window { get; private set; }

		#region Buffers
		public GeometryBuffer GeometryBuffer { get; private set; }
		public ParticleBuffer ParticleBuffer { get; private set; }
		public DepthFramebuffer ParticleDepthBuffer { get; private set; }
		public LightingBuffer LightingBuffer { get; private set; }
		public LightingBuffer LightingParticleBuffer { get; private set; }
		public DiffuseFramebuffer BloomBuffer { get; private set; }
		public DiffuseFramebuffer FogBuffer { get; private set; }
		public DiffuseFramebuffer OutlineBuffer { get; private set; }
		#endregion

		#region PostFX
		private BlurEffect particleBlur;
		private BloomEffect bloomEffect;
		#endregion

		public MutableSinglet<Camera3> View { get; private set; }
		public MutableSinglet<Vector3> AmbientColor { get; private set; }
		public Scene3Collection Scenes { get; private set; }
		public LightManager LightManager { get; private set; }

		private MutableSinglet<Vector3> FogColor;


		public RenderPipeline3( GLWindow window, Camera3 camera, Scene3Collection scenes, LightManager lightManager ) : base( "Default Rendering Pipeline 3d" ) {
			Window = window;
			View = new MutableSinglet<Camera3>( camera );
			View.Set( ( Camera3 newValue ) => { return !( newValue is null ); } );
			GeometryBuffer = new GeometryBuffer( "G-Buffer", window );
			ParticleBuffer = new ParticleBuffer( "P-Buffer", GeometryBuffer );
			LightingBuffer = new LightingBuffer( "L-Buffer", GeometryBuffer );
			LightingParticleBuffer = new LightingBuffer( "LP-Buffer", GeometryBuffer );
			Scenes = scenes;
			LightManager = lightManager;
			AmbientColor = new MutableSinglet<Vector3>( (0.03f, 0.04f, 0.03f) );
			particleBlur = new BlurEffect( new DepthFramebuffer( "Depth P Blur", GeometryBuffer ) );
			ParticleDepthBuffer = new DepthFramebuffer( "Depth P", GeometryBuffer );

			FogColor = new MutableSinglet<Vector3>( (0.35f, 0.45f, 0.80f) );
			
			FogBuffer = new DiffuseFramebuffer( "Fog", GeometryBuffer, 1 );
			BloomBuffer = new DiffuseFramebuffer( "Bloom", GeometryBuffer, 1 );
			OutlineBuffer = new DiffuseFramebuffer( "Outline", GeometryBuffer, 1 );

			bloomEffect = new BloomEffect( (0.33f, 0.33f, 0.33f) );

			CreatePipeline();
		}

		private void CreatePipeline() {
			InsertLast( new Junction( "Enable 3D Rendering", delegate () {
				Gl.Enable( EnableCap.CullFace );
				Gl.CullFace( CullFaceMode.Back );
				Gl.Enable( EnableCap.DepthTest );
				Gl.Disable( EnableCap.Blend );
				Gl.BlendEquation( BlendEquationMode.FuncAdd );
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			} ) );

			InsertLast( new Junction( "Update Shadows", UpdateLightShadows ) );

			InsertLast( new JunctionBindFramebuffer( $"Bind Geometry Buffer", GeometryBuffer, FramebufferTarget.DrawFramebuffer ) );
			InsertLast( new JunctionClearSceneBuffer( $"Clear current Buffer", ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, new Vector4( 0, 0, 0, 0 ) ) );
			InsertLast( new JunctionRenderScene<SceneObjectData3, Camera3>( "Render 3D Scene", View, Scenes.Entity ) );

			InsertLast( new Junction( "Prepare blend Rendering", delegate () {
				Gl.Enable( EnableCap.Blend );
				Gl.Disable( EnableCap.Blend, 2 );
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.One );
				Gl.Disable( EnableCap.DepthTest );
			} ) );

			InsertLast( new JunctionBindFramebuffer( $"Bind Particle Buffer", ParticleBuffer, FramebufferTarget.DrawFramebuffer ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Depth Texture", TextureUnit.Texture2, GeometryBuffer.DepthTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionClearSceneBuffer( $"Clear current Buffer", ClearBufferMask.ColorBufferBit, new Vector4( 0, 0, 0, 0 ) ) );
			InsertLast( new JunctionClearBuffer( $"Clear current depth Buffer", OpenGL.Buffer.Color, 2, new float[] { 1 } ) );
			InsertLast( new JunctionRenderScene<SceneObjectData3, Camera3>( "Render 3D Particles", View, Scenes.Particle ) );

			InsertLast( new Junction( "Enable 2D Rendering", delegate () {
				Gl.Disable( EnableCap.CullFace );
				Gl.Enable( EnableCap.Blend );
				Gl.BlendEquation( BlendEquationMode.FuncAdd );
				Gl.BlendFunc( BlendingFactor.One, BlendingFactor.One );
			} ) );

			InsertLast( new Junction( "Blur Particle Depth", delegate () {
				particleBlur.BlurParticleShadow( ParticleBuffer.ParticleDepthTexture, ParticleBuffer.Size.X, ParticleBuffer.Size.Y, 5, ParticleDepthBuffer );
				particleBlur.BlurParticleShadow( ParticleDepthBuffer.Texture, ParticleBuffer.Size.X, ParticleBuffer.Size.Y, 5, ParticleDepthBuffer );
			} ) );

			/*InsertLast( new Junction( "PrintImg", () =>{
				unsafe {
					Bitmap bmp = new Bitmap( Geometry.Size.X, Geometry.Size.Y );
					var d = bmp.LockBits( new Rectangle( 0, 0, Geometry.Size.X, Geometry.Size.Y ), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
					Gl.ReadPixels( 0, 0, Geometry.Size.X, Geometry.Size.Y, PixelFormat.Rgba, PixelType.UnsignedByte, d.Scan0 );
					bmp.UnlockBits( d );
					bmp.Save( "test.png" );
				}
			} ));*/

			InsertLast( new JunctionBindFramebuffer( $"Bind Lighting", GeometryBuffer, FramebufferTarget.ReadFramebuffer ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Diffuse Texture", TextureUnit.Texture0, GeometryBuffer.DiffuseTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Normal Texture", TextureUnit.Texture1, GeometryBuffer.NormalTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Depth Texture", TextureUnit.Texture2, GeometryBuffer.DepthTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Specular Texture", TextureUnit.Texture3, GeometryBuffer.LightPropertiesTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Geometry Glow Texture", TextureUnit.Texture4, GeometryBuffer.GlowTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Particle Diffuse Texture", TextureUnit.Texture5, ParticleBuffer.DiffuseTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Particle Glow Texture", TextureUnit.Texture6, ParticleBuffer.GlowTexture, TextureTarget.Texture2d ) );
			InsertLast( new JunctionBindTexture( $"Bind Particle Depth Texture", TextureUnit.Texture7, ParticleDepthBuffer.Texture, TextureTarget.Texture2d ) );

			InsertLast( new JunctionBindFramebuffer( $"Bind Lighting", LightingBuffer, FramebufferTarget.DrawFramebuffer ) );
			InsertLast( new JunctionClearSceneBuffer( $"Clear Lighting", ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, new Vector4( 0, 0, 0, 0 ) ) );
			InsertLast( new Junction( "Apply Ambient", RenderAmbient ) );
			InsertLast( new Junction( "Render Lights", RenderLights ) );
			InsertLast( new JunctionBindFramebuffer( $"Bind Lighting", LightingParticleBuffer, FramebufferTarget.DrawFramebuffer ) );
			InsertLast( new JunctionClearSceneBuffer( $"Clear Lighting", ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, new Vector4( 0, 0, 0, 0 ) ) );
			InsertLast( new Junction( "Apply Ambient Particles", RenderAmbientParticles ) );
			InsertLast( new Junction( "Render Lights Particles", RenderLightsParticles ) );

			InsertLast( new Junction( "Fog Rendering", Fog ) );
			InsertLast( new Junction( "Bloom Rendering", Bloom ) );
			InsertLast( new Junction( "Outline Rendering", Outline ) );

			InsertLast( new Junction( "Enable 2D Rendering", delegate () {
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			} ) );
			InsertLast( new JunctionBindWindow( "Unbind Buffer", Window ) );
			InsertLast( new JunctionRenderTexture( "Render Lights to Screen", LightingBuffer.Texture ) );
			InsertLast( new JunctionRenderTexture( "Render Bloom to Screen", BloomBuffer.Texture ) );
			InsertLast( new JunctionRenderTexture( "Render Outline to Screen", OutlineBuffer.Texture ) );
			InsertLast( new JunctionRenderTexture( "Render Fog to Screen", FogBuffer.Texture ) );
			InsertLast( new JunctionRenderTexture( "Render Lighting Particles to Screen", LightingParticleBuffer.Texture ) );

		}

		private void UpdateLightShadows() {
			LightManager.Update( View.Value );
		}

		private void RenderLights() {
			LightManager.RenderLights( View.Value, LightingBuffer.Size );
		}

		private void RenderAmbient() {
			LightManager.RenderAmbient( AmbientColor.Value );
		}

		private void RenderAmbientParticles() {
			LightManager.RenderAmbientParticles( AmbientColor.Value );
		}

		private void RenderLightsParticles() {
			LightManager.RenderLightsParticles( View.Value, LightingParticleBuffer.Size );
		}

		private void Fog() {
			FogEffect.Fog(
				GeometryBuffer.DepthTexture,
				View.Value.PerspectiveProjection.ZNear, View.Value.PerspectiveProjection.ZFar, View.Value.PerspectiveProjection.ZFar, FogColor.Value,
				GeometryBuffer.Size, FogBuffer );
		}

		private void Outline() {
			OutlineEffect.Outline(
				GeometryBuffer.DepthTexture, GeometryBuffer.NormalTexture,
				View.Value.PerspectiveProjection.ZNear, View.Value.PerspectiveProjection.ZFar, 0.2f, 0.2f, 64f, 0.1f, 1.25f,
				OutlineBuffer.Size, OutlineBuffer );
		}

		private void Bloom() {
			bloomEffect.Bloom( LightingBuffer.Texture, LightingBuffer.Size, BloomBuffer );
		}
	}
}
