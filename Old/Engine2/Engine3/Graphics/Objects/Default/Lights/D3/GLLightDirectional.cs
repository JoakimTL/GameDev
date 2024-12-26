using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.Graphics.Objects.Default.Framebuffers.STFs;
using Engine.Graphics.Objects.Default.PostFX;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.QuickstartKit;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public class GLLightDirectional {

		public const int MAX_CASCADES = 3;

		public LightDirectional Light { get; private set; }
		public LightManager LightManager { get; private set; }

		private Matrix4 lastIVP;

		private float[] cascadeRanges;
		private float[] cascadeRangesReal;

		public List<Vector3> transformedCascadePoints;
		public Vector3[] cascadePoints;
		public Cascade[] cascades;

		private bool changed;

		public GLLightDirectional( LightManager lmg, LightDirectional light, Scene3Collection scenes ) {
			Light = light;
			LightManager = lmg;
			light.AttributesChanged += SetChanged;
			cascadeRanges = new float[ MAX_CASCADES + 1 ];
			cascadeRangesReal = new float[ cascadeRanges.Length ];
			cascades = new Cascade[ MAX_CASCADES ];
			cascadePoints = new Vector3[ cascadeRanges.Length * 4 ];
			transformedCascadePoints = new List<Vector3>();

			cascadeRanges[ 0 ] = 0;
			for( int i = 0; i < MAX_CASCADES; i++ ) {
				float reach = (float) ( i + 1 ) / MAX_CASCADES;
				cascadeRanges[ i + 1 ] = reach * reach * reach;

				float r = (float) i / MAX_CASCADES;
				float res = 1 - r * r * r * r;
				cascades[ i ] = new Cascade( this, res, scenes );
			}
			changed = true;
		}

		internal void Dispose() {
			for( int i = 0; i < cascades.Length; i++ ) {
				cascades[ i ].Dispose();
			}
		}

		private void SetChanged() {
			changed = true;
		}

		public void UpdateMatrices( Camera3 camera ) {
			if( !changed && lastIVP == camera.IVPMatrix )
				return;
			changed = false;
			lastIVP = camera.IVPMatrix;

			transformedCascadePoints.Clear();
			Mem.CollisionMolds.CubeUniform.InsertTransformed( transformedCascadePoints, camera.IVPMatrix );

			for( int i = 0; i < cascadeRanges.Length; i++ ) {
				int ind = i * 4;
				for( int j = 0; j < 4; j++ )
					cascadePoints[ ind + j ] = transformedCascadePoints[ j ] * ( 1 - cascadeRanges[ i ] ) + transformedCascadePoints[ j + 4 ] * cascadeRanges[ i ];
				cascadeRangesReal[ i ] = ( ( cascadePoints[ ind + 0 ] + cascadePoints[ ind + 1 ] + cascadePoints[ ind + 2 ] + cascadePoints[ ind + 3 ] ) / 4 - camera.TranformInterface.GlobalTranslation ).Length;
				Console.WriteLine(i + "->" + cascadeRangesReal[ i ] );
			}


			Vector3 lightUp = Light.Rotation.Up.Normalized;
			Vector3 lightRight = Light.Rotation.Right.Normalized;
			Vector3 lightForward = Light.Rotation.Backward.Normalized;

			for( int i = 0; i < cascades.Length; i++ ) {
				Vector3 max = float.MinValue, min = float.MaxValue;
				for( int j = 0; j < 8; j++ ) {
					Vector3 frustumPoint = cascadePoints[ i * 4 + j ];
					Vector3 s = (Vector3.Dot( lightRight, frustumPoint ), Vector3.Dot( lightUp, frustumPoint ), Vector3.Dot( lightForward, frustumPoint ));
					if( s.X > max.X )
						max.X = s.X;
					if( s.X < min.X )
						min.X = s.X;
					if( s.Y > max.Y )
						max.Y = s.Y;
					if( s.Y < min.Y )
						min.Y = s.Y;
					if( s.Z > max.Z )
						max.Z = s.Z;
					if( s.Z < min.Z )
						min.Z = s.Z;
				}

				cascades[ i ].Set( Vector3.Transform( max + min, Light.Rotation ) * 0.5f, Light.Rotation, ( max - min ) * 0.5f * (1, 1, 2) );
			}
		}

		internal void BindCascadeTextures() {
			for( int i = 0; i < cascades.Length; i++ ) {
				cascades[ i ].Bind( TextureUnit.Texture10 + ( i * 2 ), TextureUnit.Texture10 + ( i * 2 + 1 ) );
			}
		}

		public void FillBuffers() {
			for( int i = 0; i < cascades.Length; i++ ) {
				cascades[ i ].FillBuffer();
			}
		}

		public void UpdateBufferSize() {
			for( int i = 0; i < cascades.Length; i++ ) {
				cascades[ i ].UpdateSize();
			}
		}

		internal void BindShader( Shader shader ) {
			for( int i = 0; i < cascades.Length; i++ ) {
				shader.Set( $"lvpMat[{i}]", cascades[ i ].View.VPMatrix );
				shader.Set( $"uRange[{i}]", cascadeRangesReal[ i + 1 ] );
			}
		}

		public class Cascade {

			private GLLightDirectional light;
			private float resolutonScale;
			public ShadowBuffer Buffer { get; private set; }
			private BlurEffect blur;
			public DepthBlurFramebuffer DepthBlurred;

			public View3 cascadeView;
			public IView View { get => cascadeView; }

			public OcclusionScene3 occlusionSceneEntity;
			public OcclusionScene3 occlusionSceneTransparency;
			public OcclusionScene3 occlusionSceneParticles;

			public Cascade( GLLightDirectional light, float resScale, Scene3Collection scenes ) {
				this.light = light;
				resolutonScale = resScale;
				Transform3 transform = new Transform3();
				cascadeView = new View3( new Projection( "DIR_ORTHO_PROJ", CreateProjectionMatrix( 1 ) ), transform );
				Buffer = new ShadowBuffer( (int) Math.Floor( light.LightManager.DirectionalResolution * resolutonScale ), (int) Math.Floor( light.LightManager.DirectionalResolution * resolutonScale ) );
				blur = new BlurEffect( new DepthBlurFramebuffer( "Depth Blur Buffer", Buffer, 1 ) );
				DepthBlurred = new DepthBlurFramebuffer( "Depth", Buffer, 1 );
				occlusionSceneEntity = new OcclusionScene3( "CASCADE_OCC_SCENE_E", scenes.Entity, transform );
				occlusionSceneTransparency = new OcclusionScene3( "CASCADE_OCC_SCENE_T", scenes.Transparency, transform );
				occlusionSceneParticles = new OcclusionScene3( "CASCADE_OCC_SCENE_P", scenes.Particle, transform );
				Mem.CollisionMolds.CubeUniform.MoldNew( occlusionSceneEntity.CollisionModel );
				Mem.CollisionMolds.CubeUniform.MoldNew( occlusionSceneTransparency.CollisionModel );
				Mem.CollisionMolds.CubeUniform.MoldNew( occlusionSceneParticles.CollisionModel );
			}

			private Matrix4 CreateProjectionMatrix( Vector3 scale ) {
				return Matrix4Factory.CreateOrthographic( scale.X * 2, scale.Y * 2, -scale.Z / 2, scale.Z / 2 );
			}

			public void Set( Vector3 translation, Quaternion rotation, Vector3 scale ) {
				cascadeView.TranformInterface.Translation = translation;
				cascadeView.TranformInterface.Rotation = rotation;
				cascadeView.TranformInterface.Scale = new Vector3( scale.XY, Math.Max( light.LightManager.DirectionalDepth, scale.Z ) );
				cascadeView.Projection.SetMatrix( CreateProjectionMatrix( cascadeView.TranformInterface.Scale ) );
				cascadeView.UpdateMatrices();
			}

			public void UpdateSize() {
				Buffer.SetSize( (int) Math.Floor( light.LightManager.DirectionalResolution * resolutonScale ) );

			}

			private static readonly float[] clearDepthData = new float[] { 1f, 1f };
			private static readonly byte[] clearDiffuseData = new byte[] { 255, 255, 255, 255 };

			public void FillBuffer() {
				Buffer.Bind( FramebufferTarget.DrawFramebuffer );
				Gl.Clear( ClearBufferMask.DepthBufferBit );
				Gl.ClearTexImage( Buffer.DepthTexture, 0, PixelFormat.Rg, PixelType.Float, clearDepthData );
				Gl.ClearTexImage( Buffer.DiffuseTexture, 0, PixelFormat.Rgba, PixelType.UnsignedByte, clearDiffuseData );
				Gl.Disable( EnableCap.CullFace );
				occlusionSceneEntity.Render( cascadeView, 1, RenderFunction );
				Gl.Enable( EnableCap.CullFace );
				Gl.Enable( EnableCap.Blend );
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.One );
				Gl.BlendEquation( BlendEquationMode.FuncReverseSubtract );
				Gl.DepthMask( false );
				occlusionSceneParticles.Render( cascadeView, 1, RenderFunctionParticle );
				Gl.DepthMask( true );
				Gl.BlendEquation( BlendEquationMode.FuncAdd );
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
				Gl.Disable( EnableCap.Blend );

				blur.BlurShadow( Buffer.DepthTexture, Buffer.Size.X, Buffer.Size.Y, 1, DepthBlurred );
			}

			private void RenderFunction( SceneObject<SceneObjectData3> so, Shader s, IView view ) {
				s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * view.VPMatrix );
				so.Mesh.RenderMesh();
			}

			private void RenderFunctionParticle( SceneObject<SceneObjectData3> so, Shader s, IView _ ) {
				s.Set( "uVP_mat", cascadeView.VPMatrix );
				s.Set( "uV_up", cascadeView.TranformInterface.GlobalRotation.Up );
				s.Set( "uV_right", cascadeView.TranformInterface.GlobalRotation.Right );
				so.Mesh.RenderMesh();
			}

			public void Bind( TextureUnit unitDepth, TextureUnit unitDiffuse ) {
				Gl.ActiveTexture( unitDepth );
				Gl.BindTexture( TextureTarget.Texture2d, DepthBlurred.Texture );
				Gl.ActiveTexture( unitDiffuse );
				Gl.BindTexture( TextureTarget.Texture2d, Buffer.DiffuseTexture );
			}

			internal void Dispose() {
				Buffer.Dispose();
			}
		}
	}
}