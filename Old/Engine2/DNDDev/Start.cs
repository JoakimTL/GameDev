using Engine.Entities;
using Engine.Entities.D3;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.Graphics.Objects.Default.Materials;
using Engine.Graphics.Objects.Default.Particles;
using Engine.Graphics.Objects.Default.Particles.D3;
using Engine.Graphics.Objects.Default.Particles.D3.Behaviours;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.QuickstartKit;
using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Channels;

namespace DNDDev {
	class Start : QuickstartClientRender {

		List<TextureDisplay> displays;
		List<Entity> cascades;
		List<Entity> frustumPoints;

		TextLabel debugLabel;

		LightDirectional light;
		GLLightDirectional glLight;

		LightPoint lightP;
		LightSpot lightS;

		Camera3 frustum;

		Entity test;

		CameraHandler movement;

		Material mat;

		Particle3System particles;
		Random r;

		Particle3 p;

		MutableSinglet<Vector3> wind;

		string nextDebug;

		public Start() : base( new QSWinData() { vsyncLevel = 0 }, new QS3Data() { zFar = 1024 }, new QS2Data() { frameTimer = true } ) {//, new QS2Data() { frameTimer = false } ) {

		}

		public override void EventsPolled() {
		}

		protected override void Exit() {
		}

		protected override void Initialization() {

			/*int[][] values = new int[][] { new int[] { 1, 0, int.MaxValue }, new int[] { 0, int.MinValue, 128 }, new int[] { 255, 65536 } };
			byte[] array = new byte[ 8 * 4 ];
			unsafe {
				int index = 0;
				for( int i = 0; i < values.Length; i++ ) {
					fixed( int* p = values[ i ] ) {
						byte* bb = (byte*) p;
						for( int ii = 0; ii < values[ i ].Length; ii++ )
							array[ index++ ] = bb[ ii ];
					}
				}
			}
			for( int i = 0; i < array.Length / 4; i++ ) {
				Console.WriteLine( BitConverter.ToString( array, i * 4, 4 ) );
			}
			int[] values2 = new int[ array.Length / 4 ];
			unsafe {
				fixed( byte* p = array ) {
					int* bb = (int*) p;
					for( int i = 0; i < values2.Length; i++ )
						values2[ i ] = bb[ i ];
				}
			}
			for( int i = 0; i < values2.Length; i++ ) {
				Console.WriteLine( values2[ i ] );
			}*/

			//debugLabel = new TextLabel();
			//debugLabel.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.1f ), new ModTranslationSetAligmentHorizontal( HorizontalAlignment.LEFT, true ), new ModTranslationSetAligmentVertical( VerticalAlignment.TOP, true ) ) );
			//debugLabel.Attributes.MaxLength = 50;
			//Render2.UIManager.Add( debugLabel, true );

			light = new LightDirectional( (0.9922f, 0.9843f, 0.8275f, 0.3f), 23, -70 );
			Render3.Lights.Add( light );

			{
				LightDirectional light = new LightDirectional( (0.9922f, 0.9843f, 0.8275f, 0.75f), 45, -30 );
				Render3.Lights.Add( light );
			}

			{
				LightDirectional light = new LightDirectional( (0.9922f, 0.9843f, 0.8275f, 0.75f), 90, -30 );
				Render3.Lights.Add( light );
			}

			{
				LightDirectional light = new LightDirectional( (0.9922f, 0.9843f, 0.8275f, 0.75f), 135, -30 );
				Render3.Lights.Add( light );
			}

			lightP = new LightPoint( (0.9922f, 0.9843f, 0.8275f, 3), (0, 15, 0), 80, true );
			Render3.Lights.Add( lightP );

			lightS = new LightSpot( (0.9922f, 0.9843f, 0.8275f, 0.75f), (0, 2, 0), 10, 60, -80, 0.7f, false );
			//Render3.Lights.Add( lightS );

			//frustum = new Camera3( new Perspective.Dynamic( "test", Window, 90, zFar: 64 ), new Transform3() );
			//frustum.TranformInterface.Rotation = Quaternion.Normalize( Quaternion.FromAxisAngle( Vector3.UnitX, 3.1415F/4 ) * frustum.TranformInterface.Rotation );

			//glLight = new GLLightDirectional( light, Render3.Scene );

			wind = new MutableSinglet<Vector3>( 0 );
			particles = new Particle3System( Mem.ShaderBundles.Particle3, new ParticleMaterial( "Particle Material", Mem.Textures.Get( "particles" ), 16 ), new MutableSinglet<Camera3>( Render3.Camera ), 2048 );
			Render3.InstanceSystemManager.Add( particles );
			//attract = new BehaviourAttractionSquared( 0, 0.025f );
			r = new Random();
			/*for( int i = 0; i < 0; i++ ) {
				Particle3 p = particles.CreateParticle();
				p.TerminateFromQueue = false;
				p.Data.Rotation = (float) r.NextDouble() * 2 - 1 * 3.1415f;
				p.Data.Scale = (float) r.NextDouble() * 0.1f + 2.05f;
				p.Data.Translation = 0;//				new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 1.25f;
				p.Behaviours.Add( new Behaviour3Acceleration( new Vector3( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 0.1f ) );
				//p.Behaviours.Add( new BehaviourDrag( 0.125f ) );
				//p.Behaviours.Add( new BehaviourCircularAcceleration( -0.25f, 6, (float) r.NextDouble() * 2 - 1 * 3.1415f ) );
				//p.Behaviours.Add( new BehaviourAttractionLinear( 0, -0.05f ) );
				//p.Behaviours.Add( new BehaviourTorque( -0.05f ) );
				//p.Behaviours.Add( attract );
				//p.Behaviours.Add( new BehaviourInverseAttraction( 0, 0.2f ) );
				p.Animations.Set( new AnimationTarget( 0, (170, 170, 170, 255), 0, 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, (120, 120, 120, 255), 0, 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, (70, 70, 70, 255), 0, 4, 2 ) );
				p.Animations.RepeatQueue = true;
			}*/
			Render3.Scenes.Particle.Add( particles );

			mat = new PBRMaterial( "blank" )
				.AddTexture( TextureUnit.Texture0, Mem.Textures.BlankWhite )
				.AddTexture( TextureUnit.Texture1, Mem.Textures.BlankBlack )
				.AddTexture( TextureUnit.Texture2, Mem.Textures.BlankWhite )
				.AddTexture( TextureUnit.Texture3, Mem.Textures.BlankBlack );

			nextDebug = "";
			/*{
				test = new Entity<Module3>( "test" );
				test.Add( new Transform3Module() );
				Render3Module rm = new Render3Module();
				rm.RenderObject.Mesh = Mem.Mesh3.Cube;
				rm.RenderObject.Material = new PBRMaterial( "blank" )
					.AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite )
					.AddTexture( OpenGL.TextureUnit.Texture1, Mem.Textures.BlankBlack )
					.AddTexture( OpenGL.TextureUnit.Texture2, Mem.Textures.BlankWhite )
					.AddTexture( OpenGL.TextureUnit.Texture3, Mem.Textures.BlankWhite );
				rm.RenderObject.Shader = Mem.Shaders.Get<EntityShader>();
				rm.RenderObject.SetRenderFunctionComplete( Render3.Camera );
				Mem.CollisionMolds.Cube.MoldNew( rm.RenderObject.CollisionModel );
				Render3.Scene.Add( rm.RenderObject );
				rm.RenderObject.Active = true;
				test.Add( rm );
			}*/

			displays = new List<TextureDisplay>();

			{
				Render3Module rm;
				Transform3Module tm;
				test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (0, 0, -5);
				tm.Transform.Scale = 20;
				test.Add( rm = new Render3Module( Mem.Mesh3.Get( "bunny" ), mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			{
				Render3Module rm;
				Transform3Module tm;
				Entity test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (0, -5, 0);
				tm.Transform.Scale = (512, 1, 512);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			/*{
				Render3Module rm;
				Transform3Module tm;
				Entity<Module3> test = new Entity<Module3>( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (0, 32, 0);
				tm.Transform.Scale = (512, 1, 512);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.Shaders.Get<EntityShader>() ) );
				rm.RenderObject.RenderFunction = ( SceneObject<SceneObjectData3> so, Shader s ) => {
					s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * Render3.Camera.VPMatrix );
					s.Set( "uM_mat", so.Data.TransformObject.Matrix );
					s.Set( "uColor", rm.RenderObject.Data.Color );
					so.Mesh.RenderMesh();
				};
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.Cube.MoldNew( rm.RenderObject.Data.CollisionModel );
			}*/
			{
				Render3Module rm;
				Transform3Module tm;
				Entity test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (0, 16, 64);
				tm.Transform.Scale = (64, 32, 12);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			{
				Render3Module rm;
				Transform3Module tm;
				Entity test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (0, 16, -64);
				tm.Transform.Scale = (64, 32, 12);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			{
				Render3Module rm;
				Transform3Module tm;
				Entity test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (64, 16, 0);
				tm.Transform.Scale = (12, 32, 64);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			{
				Render3Module rm;
				Transform3Module tm;
				Entity test = new Entity( "simFrustum" );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (-64, 16, 0);
				tm.Transform.Scale = (12, 32, 64);
				test.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.ShaderBundles.Entity3 ) );
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			}
			//add alpha channel to shadow mapping

			/*for( int i = 0; i < 50; i++ ) {
				Render3Module rm;
				Transform3Module tm;
				Entity<Module3> test = new Entity<Module3>( "test" + i );
				test.Add( tm = new Transform3Module() );
				tm.Transform.Translation = (
					( (float) r.NextDouble() * 2 - 1 ) * 100,
					( (float) r.NextDouble() * 2 - 1 ) * 50 + 50,
					( (float) r.NextDouble() * 2 - 1 ) * 100
				);
				tm.Transform.Scale = (1 + (float) r.NextDouble(), 1 + (float) r.NextDouble(), 1 + (float) r.NextDouble());
				tm.Transform.Scale *= 10;
				test.Add( rm = new Render3Module( Mem.Mesh3.Get( "bunny" ), mat, Mem.Shaders.Get<EntityShader>() ) );
				rm.RenderObject.RenderFunction = ( SceneObject<SceneObjectData3> so, Shader s ) => {
					s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * Render3.Camera.VPMatrix );
					s.Set( "uM_mat", so.Data.TransformObject.Matrix );
					s.Set( "uColor", rm.RenderObject.Data.Color );
					so.Mesh.RenderMesh();
				};
				Render3.Scenes.Entity.Add( rm.RenderObject );
				Mem.CollisionMolds.Cube.MoldNew( rm.RenderObject.Data.CollisionModel );
			}*/

			movement = new CameraHandler( this );
			Window.EventHandler.Mouse.Add( movement );

			//frustumPoints = new List<Entity<Module3>>();

			/*for( int i = 0; i < glLight.cascadePoints.Length; i++ ) {
				Render3Module rm;
				Transform3Module tm;
				Entity<Module3> casc = new Entity<Module3>( "cascade" );
				frustumPoints.Add( casc );
				casc.Add( tm = new Transform3Module() );
				casc.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.Shaders.Get<EntityShader>() ) );
				rm.RenderObject.RenderFunction = ( SceneObject so, Shader s ) => {
					s.Set( "uMVP_mat", so.ModelMatrix * Render3.Camera.VPMatrix );
					s.Set( "uM_mat", so.ModelMatrix );
					s.Set( "uColor", rm.RenderObject.Color );
					so.Mesh.RenderMesh();
				};
				rm.RenderObject.Color = new Vector4( 0, 0, 1, 1f );
				Render3.Scene.Add( rm.RenderObject );
			}

			for( int i = 0; i < glLight.cascades.Length; i++ ) {
				TextureDisplay texDisp = new TextureDisplay( new Texture( "test", glLight.cascades[ i ].Buffer.DepthTexture, TextureTarget.Texture2d, glLight.cascades[ i ].Buffer.Size ) );
				texDisp.Constraints.Set( new ConstraintBundle(
					new ModScalingSet( 0.2f ),
					new ModTranslationSetAligmentHorizontal( HorizontalAlignment.RIGHT, true ),
					new ModTranslationSetAligmentVertical( VerticalAlignment.TOP, true ),
					new ModTranslationAdd( (-0.21f, -0.21f - 0.41f * i) ) ) );
				displays.Add( texDisp );
				Render2.UIManager.Add( texDisp, true );

				{
					IView view = glLight.cascades[ i ].View;
					Render3Module rm;
					Transform3Module tm;
					Entity<Module3> casc = new Entity<Module3>( "cascade" );
					casc.Add( tm = new Transform3Module() );
					tm.Transform.Scale = 1;
					casc.Add( rm = new Render3Module( Mem.Mesh3.Cube, mat, Mem.Shaders.Get<EntityShader>() ) );
					rm.RenderObject.RenderFunction = ( SceneObject so, Shader s ) => {
						s.Set( "uMVP_mat", so.ModelMatrix * view.IVPMatrix * Render3.Camera.VPMatrix );
						s.Set( "uM_mat", so.ModelMatrix );
						s.Set( "uColor", rm.RenderObject.Color );
						so.Mesh.RenderMesh();
					};
					rm.RenderObject.Color = new Vector4( 1, 1, 0, 1f );
					Render3.Scene.Add( rm.RenderObject );

				}
			}*/

			/*Console.WriteLine( Quaternion.FromEulerAngles( 90, 0, 0 ).Forward );//check which part affects which axis
			Console.WriteLine( Render3.Camera.TranformInterface.Rotation.Forward );//check which part affects which axis*/
		}

		protected override void InLoop() {

			/*if (mapDisplay is null ) {
				var v = Render3.Lights.Get( light );
				if (!(v is null ) ) {
					mapDisplay = new TextureDisplay( new Texture( "t", v.ValueB.cascades[ 0 ].Buffer.DepthTexture, OpenGL.TextureTarget.Texture2d, v.ValueB.cascades[ 0 ].Buffer.Size ) );
					mapDisplay.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.2f ), new ModTranslationSetAligmentVertical( VerticalAlignment.TOP, true ), new ModTranslationAdd((0, -.2f)) ) );
					Render2.UIManager.Add( mapDisplay, true );
				}
			}*/

			//light.Rotation = Quaternion.Normalize( Quaternion.FromAxisAngle( Vector3.UnitY, 0.0001f ) * light.Rotation );

			/*if( glLight is null ) {
				var duo = Render3.Lights.Get( light );
				if( !( duo is null ) ) {
					glLight = duo.ValueB;
					for( int i = 0; i < glLight.cascades.Length; i++ ) {
						TextureDisplay texDisp = new TextureDisplay( new Texture( "test", glLight.cascades[ i ].Buffer.DiffuseTexture, TextureTarget.Texture2d, glLight.cascades[ i ].Buffer.Size ) );
						texDisp.Constraints.Set( new ConstraintBundle(
							new ModScalingSet( 0.2f ),
							new ModTranslationSetAligmentHorizontal( HorizontalAlignment.RIGHT, true ),
							new ModTranslationSetAligmentVertical( VerticalAlignment.TOP, true ),
							new ModTranslationAdd( (-0.21f, -0.21f - 0.41f * i) ) ) );
						displays.Add( texDisp );
						Render2.UIManager.Add( texDisp, true );
					}
				}
			}*/

			/*frustum.TranformInterface.Rotation = Quaternion.Normalize( Quaternion.FromAxisAngle( Vector3.UnitY, -0.0005f ) * frustum.TranformInterface.Rotation );

			frustum.UpdateMatrices();
			glLight.UpdateMatrices( frustum );
			glLight.FillBuffers();
			Window.Bind();

			for( int i = 0; i < glLight.cascadePoints.Length; i++ ) {
				if( frustumPoints[ i ].Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = glLight.cascadePoints[ i ];
				}
			}*/

			int it = 0;
			while( particles.ActiveParticles < 2048 && it < 1 ) {
				it++;
				p = particles.CreateParticle();
				if( !( p is null ) ) {
					p.Data.Rotation = 0;
					p.Data.Scale = 1.5f;
					p.Data.Translation = (8, 3, 8) + new Vector3( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ).Normalized;//				new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 1.25f;

					p.Behaviours.Add( new Behaviour3Acceleration( new Vector3( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ).Normalized * .32f ) );
					p.Behaviours.Add( new Behaviour3Acceleration( 3 * new Vector3( 0, 0.75f, 0 ) ) );
					//p.Behaviours.Add( new Behaviour3Wind( wind, 0 ) );
					//p.Behaviours.Add( new Behaviour3Timed( 3, 18, CrazyLoop ) );
					//p.Behaviours.Add( new BehaviourCircularAcceleration( -0.25f, 6, (float) r.NextDouble() * 2 - 1 * 3.1415f ) );
					//p.Behaviours.Add( new BehaviourAttractionLinear( 0, -0.05f ) );
					//p.Behaviours.Add( new BehaviourTorque( -0.05f ) );
					//p.Behaviours.Add( attract );
					//p.Behaviours.Add( new BehaviourInverseAttraction( 0, 0.2f ) );

					p.Animations.Set( new AnimationTarget( 0, (0, 255, 0, 255), (0, 255, 0, 255), 1, 0 ) );
					p.Animations.Enqueue( new AnimationTarget( 0, (255, 255, 0, 255), (255, 255, 0, 255), 1, 0 ) );
					p.Animations.Enqueue( new AnimationTarget( 0, (255, 0, 0, 255), (255, 0, 0, 255), 1, 0 ) );
					p.Animations.Enqueue( new AnimationTarget( 0, 0, (175, 175, 175, 0), 1, 0 ) );
				}
			}

			wind.Value = new Vector3( (float) Math.Cos( Clock32.Standard.Time * 0.06f ), 0, (float) Math.Sin( Clock32.Standard.Time * 0.06f ) ) * 0.02f;

			{
				if( test.Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = (0, 0, (float) Math.Sin( Clock32.Standard.Time / 5 ) * 128);
				}
			}

			if( !( p is null ) ) {
				//debugLabel.Content = p.Lifetime.ToString();
			}

			movement.CameraUpdate();
			StandardPipeline.Exectute();
		}
	}
}
