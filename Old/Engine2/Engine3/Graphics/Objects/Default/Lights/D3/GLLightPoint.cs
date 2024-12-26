using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Framebuffers;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.QuickstartKit;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Graphics.Utilities;
using OpenGL;
using System;
using System.Diagnostics;
using System.Net;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public class GLLightPoint {

		public const int RESOLUTION = 512;

		private static TextureTarget[] renderTargets = {
			TextureTarget.TextureCubeMapPositiveX,
			TextureTarget.TextureCubeMapNegativeX,
			TextureTarget.TextureCubeMapPositiveY,
			TextureTarget.TextureCubeMapNegativeY,
			TextureTarget.TextureCubeMapPositiveZ,
			TextureTarget.TextureCubeMapNegativeZ
		};

		private static Quaternion[] rotationList = new Quaternion[] {
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( 1.0f, 0.0f, 0.0f ), new Vector3( 0.0f, -1.0f, 0.0f ) ) ),
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( -1.0f, 0.0f, 0.0f ), new Vector3( 0.0f, -1.0f, 0.0f ) ) ),
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( 0.0f, 1.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 1.0f ) ) ),
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( 0.0f, -1.0f, 0.0f ), new Vector3( 0.0f, 0.0f, -1.0f ) ) ),
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( 0.0f, 0.0f, 1.0f ), new Vector3( 0.0f, -1.0f, 0.0f ) ) ),
			Quaternion.FromMatrix( Matrix4Factory.LookAt( 0, new Vector3( 0.0f, 0.0f, -1.0f ), new Vector3( 0.0f, -1.0f, 0.0f ) ) )
		};

		public LightPoint Light { get; private set; }

		private Transform3 transform;
		private View3[] shadowViews;
		private Projection shadowProjection;

		public Matrix4 Matrix { get => transform.Matrix; }

		private bool changed;

		#region Shadow Stuff

		public CubeShadowBuffer Buffer { get; private set; }
		private OcclusionScene3 occSceneEntity;
		private OcclusionScene3 occSceneTransparent;
		private OcclusionScene3 occSceneParticle;

		#endregion

		public GLLightPoint( LightManager lmg, LightPoint light, Scene3Collection scenes ) {
			Light = light;
			light.AttributesChanged += SetChanged;

			transform = new Transform3();
			shadowProjection = new Projection( "POINT SHADOW PROJECTION", Matrix4.Identity );

			if( light.HasShadows ) {
				shadowViews = new View3[ 6 ];
				for( int i = 0; i < shadowViews.Length; i++ ) {
					Transform3 shadowTransform = new Transform3();
					shadowTransform.SetParent( transform );
					shadowTransform.Rotation = rotationList[ i ];
					shadowViews[ i ] = new View3( shadowProjection, shadowTransform );
				}
				occSceneEntity = new OcclusionScene3( "POINT SHADOW SCENE E", scenes.Entity, transform );
				occSceneTransparent = new OcclusionScene3( "POINT SHADOW SCENE T", scenes.Transparency, transform );
				occSceneParticle = new OcclusionScene3( "POINT SHADOW SCENE P", scenes.Particle, transform );
				Mem.CollisionMolds.CubeUniform.MoldNew( occSceneEntity.CollisionModel );
				Mem.CollisionMolds.CubeUniform.MoldNew( occSceneTransparent.CollisionModel );
				Mem.CollisionMolds.CubeUniform.MoldNew( occSceneParticle.CollisionModel );
				Buffer = new CubeShadowBuffer( RESOLUTION, RESOLUTION );
			}

			changed = true;
		}

		private void ShadowParticleRenderMethod( SceneObject<SceneObjectData3> so, Shader s, IView view ) {
			s.Set( "uVP_mat", view.VPMatrix );
			so.Mesh.RenderMesh();
		}

		internal void Dispose() {
			if( Light.HasShadows ) {
				Buffer.Dispose();
			}
		}

		private void SetChanged() {
			changed = true;
		}

		public void UpdateMatrices( Camera3 camera ) {
			if( !changed )
				return;
			changed = false;

			transform.Translation = Light.Translation;
			transform.Scale = Light.Range;
			shadowProjection.SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( (float) Math.PI / 2, 1, 0.00390625f, Light.Range ) );
		}

		public void FillBuffers() {
			if( Light.HasShadows ) {
				Gl.ClearColor( 1, 1, 1, 1 );
				Mem.Shaders.Get<ShadowPointShader>().Bind();
				Mem.Shaders.Get<ShadowPointShader>().Set( "lightPos", Light.Translation );
				Mem.Shaders.Get<ShadowPointShader>().Set( "lRange", Light.Range );
				for( int i = 0; i < 6; i++ ) {
					shadowViews[ i ].UpdateMatrices();
					Buffer.BindDraw( renderTargets[ i ], Buffer.CubeDepthMap, FramebufferAttachment.ColorAttachment0 );
					Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
					occSceneEntity.Render( shadowViews[ i ], 2 );
					RenderParticles( shadowViews[ i ], i );
				}
				Gl.ClearColor( 0, 0, 0, 0 );
			}
		}

		public void UpdateBufferSize( int res ) {
			Buffer.SetSize( res );
		}

		private void RenderParticles( IView view, int i ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.One );
			Gl.BlendEquation( BlendEquationMode.FuncReverseSubtract );
			Gl.DepthMask( false );
			Mem.Shaders.Get<ShadowPointParticleShader>().Bind();
			Mem.Shaders.Get<ShadowPointParticleShader>().Set( "lightPos", Light.Translation );
			Buffer.BindDraw( renderTargets[ i ], Buffer.CubeDiffuseMap, FramebufferAttachment.ColorAttachment1 );
			Gl.Clear( ClearBufferMask.ColorBufferBit );
			occSceneParticle.Render( view, 2, ShadowParticleRenderMethod );
			Gl.DepthMask( true );
			Gl.BlendEquation( BlendEquationMode.FuncAdd );
			Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			Gl.Disable( EnableCap.Blend );
		}
	}
}