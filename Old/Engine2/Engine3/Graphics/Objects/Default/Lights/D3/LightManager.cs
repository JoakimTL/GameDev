using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.QuickstartKit;
using Engine.Settings;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Lights.D3 {
	public sealed class LightManager {

		private Scene3Collection scenes;
		private LightTypeHandler<LightDirectional, GLLightDirectional> directionals;
		private LightTypeHandler<LightPoint, GLLightPoint> points;
		private LightTypeHandler<LightSpot, GLLightSpot> spots;
		private List<GLLightPoint> pointShadow;
		private List<GLLightPoint> pointNoShadow;
		private List<GLLightSpot> spotShadow;
		private List<GLLightSpot> spotNoShadow;

		private bool settingsChanged;
		private SettingsFile settings;
		public float DirectionalDepth { get; private set; }
		public int DirectionalResolution { get; private set; }
		public int PointResolution { get; private set; }
		public int SpotResolution { get; private set; }


		public LightManager( Scene3Collection scenes ) {
			this.scenes = scenes;
			directionals = new LightTypeHandler<LightDirectional, GLLightDirectional>( LightCreation, LightDestruction );
			points = new LightTypeHandler<LightPoint, GLLightPoint>( LightCreation, LightDestruction );
			spots = new LightTypeHandler<LightSpot, GLLightSpot>( LightCreation, LightDestruction );
			pointShadow = new List<GLLightPoint>();
			pointNoShadow = new List<GLLightPoint>();
			spotShadow = new List<GLLightSpot>();
			spotNoShadow = new List<GLLightSpot>();
			settings = Mem.Settings.Add( "lights",
				new Setting( "DirectionalLightShadowDepth", 512f ),
				new Setting( "DirectionalLightShadowResolution", 2048 ),
				new Setting( "PointLightShadowResolution", 512 ),
				new Setting( "SpotLightShadowResolution", 512 )
			);
			settings.SettingsChanged += SettingsChanged;

			{
				if( settings.TryGet( "DirectionalLightShadowDepth", out float o ) )
					DirectionalDepth = o;
			}
			{
				if( settings.TryGet( "DirectionalLightShadowResolution", out int o ) )
					DirectionalResolution = o;
			}
			{
				if( settings.TryGet( "PointLightShadowResolution", out int o ) )
					PointResolution = o;
			}
			{
				if( settings.TryGet( "SpotLightShadowResolution", out int o ) )
					SpotResolution = o;
			}
		}

		private void SettingsChanged() {
			if( settings.TryGet( "DirectionalLightShadowDepth", out float o ) )
				DirectionalDepth = o;
			settingsChanged = true;
		}

		public void Add( LightDirectional ld ) {
			directionals.Add( ld );
		}

		public void Remove( LightDirectional ld ) {
			directionals.Remove( ld );
		}

		public void Add( LightPoint ld ) {
			points.Add( ld );
		}

		public void Remove( LightPoint ld ) {
			points.Remove( ld );
		}

		public void Add( LightSpot ld ) {
			spots.Add( ld );
		}

		public void Remove( LightSpot ld ) {
			spots.Remove( ld );
		}

		public ImmutableDuo<LightDirectional, GLLightDirectional> Get( LightDirectional ld ) {
			if( directionals.Lights.TryGetValue( ld, out ImmutableDuo<LightDirectional, GLLightDirectional> o ) )
				return o;
			return default;
		}

		public void Update( Camera3 camera ) {
			if( settingsChanged ) {
				foreach( ImmutableDuo<LightDirectional, GLLightDirectional> duo in directionals.Lights.Values ) {
					duo.ValueB.UpdateBufferSize();
				}
				foreach( ImmutableDuo<LightPoint, GLLightPoint> duo in points.Lights.Values ) {
					duo.ValueB.UpdateBufferSize( PointResolution );
				}
				foreach( ImmutableDuo<LightSpot, GLLightSpot> duo in spots.Lights.Values ) {
					duo.ValueB.UpdateBufferSize( SpotResolution );
				}
				settingsChanged = false;
			}
			Gl.Disable( EnableCap.Blend );
			directionals.Update();
			foreach( ImmutableDuo<LightDirectional, GLLightDirectional> duo in directionals.Lights.Values ) {
				duo.ValueB.UpdateMatrices( camera );
			}
			foreach( ImmutableDuo<LightDirectional, GLLightDirectional> duo in directionals.Lights.Values ) {
				duo.ValueB.FillBuffers();
			}
			if( points.Update() )
				SortPointLights();
			foreach( ImmutableDuo<LightPoint, GLLightPoint> duo in points.Lights.Values ) {
				duo.ValueB.UpdateMatrices( camera );
			}
			foreach( ImmutableDuo<LightPoint, GLLightPoint> duo in points.Lights.Values ) {
				duo.ValueB.FillBuffers();
			}
			if( spots.Update() )
				SortSpotLights();
			foreach( ImmutableDuo<LightSpot, GLLightSpot> duo in spots.Lights.Values ) {
				duo.ValueB.UpdateMatrices( camera );
			}
			foreach( ImmutableDuo<LightSpot, GLLightSpot> duo in spots.Lights.Values ) {
				duo.ValueB.FillBuffers();
			}
		}

		private void SortPointLights() {
			pointShadow.Clear();
			pointNoShadow.Clear();
			foreach( ImmutableDuo<LightPoint, GLLightPoint> duo in points.Lights.Values ) {
				if( duo.ValueA.HasShadows ) {
					pointShadow.Add( duo.ValueB );
				} else {
					pointNoShadow.Add( duo.ValueB );
				}
			}
		}

		private void SortSpotLights() {
			spotShadow.Clear();
			spotNoShadow.Clear();
			foreach( ImmutableDuo<LightSpot, GLLightSpot> duo in spots.Lights.Values ) {
				if( duo.ValueA.HasShadows ) {
					spotShadow.Add( duo.ValueB );
				} else {
					spotNoShadow.Add( duo.ValueB );
				}
			}
		}

		public void RenderAmbient( Vector3 color ) {
			Mem.Shaders.Get<AmbientLightShader>().Bind();
			Mem.Shaders.Get<AmbientLightShader>().Set( "uCol", color );
			Mem.Mesh2.Square.Bind();
			Mem.Mesh2.Square.RenderMesh();
			Mem.Mesh2.Square.Unbind();
			Mem.Shaders.Get<AmbientLightShader>().Unbind();
		}

		public void RenderAmbientParticles( Vector3 color ) {
			Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			Mem.Shaders.Get<AmbientParticleShader>().Bind();
			Mem.Shaders.Get<AmbientParticleShader>().Set( "uCol", color );
			Mem.Mesh2.Square.Bind();
			Mem.Mesh2.Square.RenderMesh();
			Mem.Mesh2.Square.Unbind();
			Mem.Shaders.Get<AmbientParticleShader>().Unbind();
			Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.One );
		}

		public void RenderLights( Camera3 camera, Vector2i lightingBufferSize ) {
			{
				DirectionalLightShader dirShader = Mem.Shaders.Get<DirectionalLightShader>();
				Mesh mesh = Mem.Mesh2.Square;
				dirShader.Bind();
				mesh.Bind();

				dirShader.SetCamera( camera.TranformInterface.GlobalTranslation, camera.PerspectiveProjection.ZNear, camera.PerspectiveProjection.ZFar );
				dirShader.SetInverseMatrix( camera.IVPMatrix );

				foreach( ImmutableDuo<LightDirectional, GLLightDirectional> duo in directionals.Lights.Values ) {
					duo.ValueB.BindCascadeTextures();

					dirShader.SetLight( duo.ValueB );
					mesh.RenderMesh();
				}

				mesh.Unbind();
				dirShader.Unbind();
			}
			{
				Gl.Enable( EnableCap.CullFace );
				Gl.CullFace( CullFaceMode.Front );
				Mesh mesh = Mem.Mesh3.Icosphere;
				mesh.Bind();
				{
					if( pointShadow.Count > 0 ) {
						PointLightShader shader = Mem.Shaders.Get<PointLightShader>();

						shader.Bind();

						shader.Set( "viewPort", lightingBufferSize.AsFloat );
						shader.SetCamera( camera.TranformInterface.GlobalTranslation );
						shader.SetInverseMatrix( camera.IVPMatrix );

						foreach( GLLightPoint l in pointShadow ) {
							Gl.ActiveTexture( TextureUnit.Texture10 );
							Gl.BindTexture( TextureTarget.TextureCubeMap, l.Buffer.CubeDepthMap );
							Gl.ActiveTexture( TextureUnit.Texture11 );
							Gl.BindTexture( TextureTarget.TextureCubeMap, l.Buffer.CubeDiffuseMap );

							shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
							shader.SetLight( l.Light );
							mesh.RenderMesh();
						}
						shader.Unbind();
					}
					if( pointNoShadow.Count > 0 ) {
						PointLightNoShadowShader shader = Mem.Shaders.Get<PointLightNoShadowShader>();

						shader.Bind();

						shader.Set( "viewPort", lightingBufferSize.AsFloat );
						shader.SetCamera( camera.TranformInterface.GlobalTranslation );
						shader.SetInverseMatrix( camera.IVPMatrix );

						foreach( GLLightPoint l in pointNoShadow ) {
							shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
							shader.SetLight( l.Light );
							mesh.RenderMesh();
						}
						shader.Unbind();
					}
				}
				{
					if( spotShadow.Count > 0 ) {
						SpotLightShader shader = Mem.Shaders.Get<SpotLightShader>();

						shader.Bind();

						shader.Set( "viewPort", lightingBufferSize.AsFloat );
						shader.SetCamera( camera.TranformInterface.GlobalTranslation );
						shader.SetInverseMatrix( camera.IVPMatrix );

						foreach( GLLightSpot l in spotShadow ) {
							Gl.ActiveTexture( TextureUnit.Texture10 );
							Gl.BindTexture( TextureTarget.TextureCubeMap, l.Buffer.CubeDepthMap );
							Gl.ActiveTexture( TextureUnit.Texture11 );
							Gl.BindTexture( TextureTarget.TextureCubeMap, l.Buffer.CubeDiffuseMap );

							shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
							shader.SetLight( l.Light );
							mesh.RenderMesh();
						}
						shader.Unbind();
					}
					if( spotNoShadow.Count > 0 ) {
						SpotLightNoShadowShader shader = Mem.Shaders.Get<SpotLightNoShadowShader>();

						shader.Bind();

						shader.Set( "viewPort", lightingBufferSize.AsFloat );
						shader.SetCamera( camera.TranformInterface.GlobalTranslation );
						shader.SetInverseMatrix( camera.IVPMatrix );

						foreach( GLLightSpot l in spotNoShadow ) {
							shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
							shader.SetLight( l.Light );
							mesh.RenderMesh();
						}
						shader.Unbind();
					}
				}
				mesh.Unbind();
				Gl.CullFace( CullFaceMode.Back );
				Gl.Disable( EnableCap.CullFace );
			}
		}

		public void RenderLightsParticles( Camera3 camera, Vector2i lightingBufferSize ) {
			{
				DirectionalLightParticleShader dirShader = Mem.Shaders.Get<DirectionalLightParticleShader>();
				Mesh mesh = Mem.Mesh2.Square;
				dirShader.Bind();
				mesh.Bind();

				dirShader.SetCamera( camera.TranformInterface.GlobalTranslation, camera.PerspectiveProjection.ZNear, camera.PerspectiveProjection.ZFar );
				dirShader.SetInverseMatrix( camera.IVPMatrix );

				foreach( ImmutableDuo<LightDirectional, GLLightDirectional> duo in directionals.Lights.Values ) {
					duo.ValueB.BindCascadeTextures();

					dirShader.SetLight( duo.ValueB );
					mesh.RenderMesh();
				}

				mesh.Unbind();
				dirShader.Unbind();
			}
			{
				Gl.Enable( EnableCap.CullFace );
				Gl.CullFace( CullFaceMode.Front );
				Mesh mesh = Mem.Mesh3.Icosphere;
				if( points.Lights.Count > 0 ) {
					PointLightParticleNoShadowShader shader = Mem.Shaders.Get<PointLightParticleNoShadowShader>();

					mesh.Bind();
					shader.Bind();

					shader.Set( "viewPort", lightingBufferSize.AsFloat );
					shader.SetCamera( camera.TranformInterface.GlobalTranslation );
					shader.SetInverseMatrix( camera.IVPMatrix );

					foreach( ImmutableDuo<LightPoint, GLLightPoint> d in points.Lights.Values ) {
						GLLightPoint l = d.ValueB;
						shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
						shader.SetLight( l.Light );
						mesh.RenderMesh();
					}
					shader.Unbind();
					mesh.Unbind();
				}
				if( spots.Lights.Count > 0 ) {
					SpotLightNoShadowParticleShader shader = Mem.Shaders.Get<SpotLightNoShadowParticleShader>();

					mesh.Bind();
					shader.Bind();

					shader.Set( "viewPort", lightingBufferSize.AsFloat );
					shader.SetCamera( camera.TranformInterface.GlobalTranslation );
					shader.SetInverseMatrix( camera.IVPMatrix );

					foreach( ImmutableDuo<LightSpot, GLLightSpot> d in spots.Lights.Values ) {
						GLLightSpot l = d.ValueB;
						shader.Set( "uMVP_mat", l.Matrix * camera.VPMatrix );
						shader.SetLight( l.Light );
						mesh.RenderMesh();
					}
					shader.Unbind();
					mesh.Unbind();
				}
				Gl.CullFace( CullFaceMode.Back );
				Gl.Disable( EnableCap.CullFace );
			}
		}

		private GLLightDirectional LightCreation( LightDirectional ld ) {
			return new GLLightDirectional( this, ld, scenes );
		}

		private void LightDestruction( GLLightDirectional glLight ) {
			glLight.Dispose();
		}

		private GLLightPoint LightCreation( LightPoint ld ) {
			return new GLLightPoint( this, ld, scenes );
		}

		private void LightDestruction( GLLightPoint glLight ) {
			glLight.Dispose();
		}

		private GLLightSpot LightCreation( LightSpot ld ) {
			return new GLLightSpot( this, ld, scenes );
		}

		private void LightDestruction( GLLightSpot glLight ) {
			glLight.Dispose();
		}

		internal class LightTypeHandler<T, T2> {
			private Queue<T> newLights, outLights;
			internal Dictionary<T, ImmutableDuo<T, T2>> Lights { get; private set; }
			internal delegate T2 LightCreationHandler( T newLight );
			internal delegate void LightDestructionHandler( T2 glLight );
			internal LightCreationHandler LightCreator { get; private set; }
			internal LightDestructionHandler LightDestroyer { get; private set; }

			internal LightTypeHandler( LightCreationHandler lightCreator, LightDestructionHandler lightDestroyer ) {
				LightCreator = lightCreator;
				LightDestroyer = lightDestroyer;
				newLights = new Queue<T>();
				outLights = new Queue<T>();
				Lights = new Dictionary<T, ImmutableDuo<T, T2>>();
			}

			internal void Add( T l ) {
				lock( newLights )
					newLights.Enqueue( l );
			}

			internal void Remove( T l ) {
				lock( outLights )
					outLights.Enqueue( l );
			}

			internal bool Update() {
				bool updated = false;
				lock( newLights )
					while( newLights.Count > 0 ) {
						T newLight = newLights.Dequeue();
						if( !Lights.ContainsKey( newLight ) ) {
							Lights.Add( newLight, new ImmutableDuo<T, T2>( newLight, LightCreator.Invoke( newLight ) ) );
							updated = true;
						}
					}

				lock( outLights )
					while( outLights.Count > 0 ) {
						T l = outLights.Dequeue();
						LightDestroyer.Invoke( Lights[ l ].ValueB );
						Lights.Remove( l );
						updated = true;
					}
				return updated;
			}
		}
	}
}