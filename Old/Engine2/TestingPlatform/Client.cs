using Engine.MemLib;
using Engine.Pipelines;
using Engine.QuickstartKit;
using Engine.Utilities.Graphics.Utilities;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using Engine.Utilities.Data;
using Engine.LinearAlgebra;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Transforms;
using Engine;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.Particles.D2;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Particles;
using Engine.Graphics.Objects.Default.Particles.D2.Behaviours;

namespace TestingPlatform {
	class Client : QuickstartClientRender {

		TestBox[] menuBoxes;
		/*Particle2System particles;
		Particle2 p;
		BehaviourAttractionSquared attract;*/
		private Particle2System particles;
		private BehaviourAttractionSquared attract;
		private Particle2 p;

		public Client() : base( new QSWinData() { vsyncLevel = 0 }, new QS3Data(), new QS2Data() { frameTimer = true } ) {

		}

		public override void EventsPolled() {
		}

		protected override void Exit() {

		}

		protected override void Initialization() {
			/*TextureDisplay disp = new TextureDisplay( Mem.Textures.BlankWhite );
			disp.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.5f ), new ModTranslationAdd( (0, 0.25f) ), new ModRotationAdd( 0.4f ) ) );
			Render2.UIManager.Add( disp, true );

			particles = new Particle2System( Mem.ShaderBundles.Particle2, new ParticleMaterial( "Particle Material", Mem.Textures.Get( "particles" ), 16 ), Render2.Camera, 2048 );
			Render2.InstanceSystemManager.Add( particles );
			attract = new BehaviourAttractionSquared( 0, 0.025f );
			Random r = new Random();
			for( int i = 0; i < 2048; i++ ) {
				p = particles.CreateParticle();
				p.TerminateFromQueue = false;
				p.Data.Rotation = (float) r.NextDouble() * 2 - 1 * 3.1415f;
				p.Data.Scale = (float) r.NextDouble() * 0.1f + 0.05f;
				p.Data.Translation = 0;//				new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 1.25f;
				p.Behaviours.Add( new BehaviourAcceleration( new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 0.1f ) );
				p.Behaviours.Add( new BehaviourDrag( 0.125f ) );
				p.Behaviours.Add( new BehaviourCircularAcceleration( -0.25f, 6, (float) r.NextDouble() * 2 - 1 * 3.1415f ) );
				p.Behaviours.Add( new BehaviourAttractionLinear( 0, -0.05f ) );
				p.Behaviours.Add( new BehaviourTorque( -0.05f ) );
				p.Behaviours.Add( attract );
				p.Behaviours.Add( new BehaviourInverseAttraction( 0, 0.2f ) );
				p.Animations.Set( new AnimationTarget( 1, (255, 0, 255, 255), 0, 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, (0, 255, 0, 255), 0, 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, (0, 0, 255, 255), 0, 4, 2 ) );
				p.Animations.RepeatQueue = true;
			}
			Render2.SceneEntity.Add( particles );
			*/
			/*inventory = new Inventory();
			Render2.UIManager.Add( inventory.display );
			inventory.display.Constraints.Set( new ConstraintBundle( new ModScalingSetPixel( (384 / 2, 64 / 2) ), new ModTranslationSetAligmentHorizontal( HorizontalAlignment.RIGHT, true ), new ModTranslationAdd( (0, .5f) ), new ModTranslationAddPixel( (-400 / 2, 0) ) ), 0 );
			inventory.display.Activate();

			inventory[ 0 ] = new Item( new Material( "jordbær" ).AddTexture( TextureUnit.Texture0, Mem.Textures.Get( "strawberry" ) ) );

			Render2.Camera.Projection.Scale = 10;
			world = new World( Render2.SceneEntity, Render2.Camera, Window );

			npc = new NPC( new Material( "jonas" ).AddTexture( TextureUnit.Texture0, Mem.Textures.Get( "jonas" ) ), Mem.Mesh2.Square );
			npc.SetRenderFunction( Render2.Camera );
			world.AddEntity( npc );*/
			#region trash
			menuBoxes = new TestBox[ 7 ];
			for( int i = 0; i < menuBoxes.Length; i++ ) {
				menuBoxes[ i ] = new TestBox( Mem.Textures.BlankWhite, new Vector2( 0, 0.5f - i * 0.2f ), new Vector2( 0.2f, 0.075f ) );
				Render2.UIManager.Add( menuBoxes[ i ] );
				Mem.CollisionMolds.SquareUniform.MoldNew( menuBoxes[ i ].Data.CollisionModel );
				menuBoxes[ i ].Activate();
			}
			/*

			particles = new Particle2System( Mem.Shaders.Get<Particle2Shader>(), new ParticleMaterial( "Particle Material", Mem.Textures.Get( "particles" ), 16 ), Render2.Camera, 2048 );
			Render2.InstanceSystemManager.Add( particles );
			attract = new BehaviourAttractionSquared( 0, 0.025f );
			Random r = new Random();
			for( int i = 0; i < 2048; i++ ) {
				p = particles.CreateParticle();
				p.TerminateFromQueue = false;
				p.Data.Rotation = (float) r.NextDouble() * 2 - 1 * 3.1415f;
				p.Data.Scale = (float) r.NextDouble() * 0.1f + 0.05f;
				p.Data.Translation = 0;//				new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 1.25f;
				p.Behaviours.Add( new BehaviourAcceleration( new Vector2( (float) r.NextDouble() * 2 - 1, (float) r.NextDouble() * 2 - 1 ) * 0.1f ) );
				p.Behaviours.Add( new BehaviourDrag( 0.125f ) );
				p.Behaviours.Add( new BehaviourCircularAcceleration( -0.25f, 6, (float) r.NextDouble() * 2 - 1 * 3.1415f ) );
				p.Behaviours.Add( new BehaviourAttractionLinear( 0, -0.05f ) );
				p.Behaviours.Add( attract );
				p.Behaviours.Add( new BehaviourInverseAttraction( 0, 0.2f ) );
				p.Animations.Set( new AnimationTarget( 0, ( 1, 0, 1, 1 ), 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, (0, 1, 0, 1), 4, 2 ) );
				p.Animations.Enqueue( new AnimationTarget( 0, ( 0, 0, 1, 1 ), 4, 2 ) );
				p.Animations.RepeatQueue = true;
			}
			Render2.SceneEntity.Add( particles );

			/*texDisp = new TextureDisplay( Mem.Textures.BlankWhite );
			texDisp.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.15f ), new ModTranslationAdd( new Vector2( 0.5f, 0 ) ) ), 0 );
			texDisp.Constraints.Enqueue( new ConstraintBundle( new ModScalingSet( 0.35f ), new ModColorSet( new Vector4( 1, 0.5f, 0, 1 ) ), new ModRotationAdd( 2.0f ), new ModTranslationAdd( new Vector2( 0.5f, 0 ) ) ), 2, InterpolationMethods.LinearInterpolation, 0 );
			Render2.UIManager.Add( texDisp );

			texDisp.Activate();

			texDisp2 = new TextureDisplay( Mem.Textures.BlankWhite );
			pos = new ModTranslationAdd( new Vector2( 0.25f, 0 ) );
			texDisp2.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.15f ), new ModTranslationAdd( new Vector2( 0.5f, 0 ) ) ), 0 );
			texDisp2.Constraints.Enqueue( new ConstraintBundle( new ModScalingSet( 0.35f ), new ModColorSet( new Vector4( 1, 0.5f, 0.5f, 1 ) ), new ModTranslationAdd( new Vector2( 0.5f, 0 ) ) ), 2, InterpolationMethods.LinearInterpolation, 0 );
			texDisp2.Constraints.Enqueue( new ConstraintBundle( new ModScalingSet( 0.55f ), new ModColorSet( new Vector4( 1, 0.5f, 0.5f, 1 ) ), pos ), 2, InterpolationMethods.LinearInterpolation, 0 );
			//texDisp2.Constraints.RepeatQueue = true;
			Mem.CollisionMolds.Square.MoldNew( texDisp2.CollisionModel );
			Render2.UIManager.Add( texDisp2 );
			texDisp2.ActiveUpdated += UpdatedLMAO;

			posA = new ModTranslationAdd( 0 );
			posB = new ModTranslationAdd( 0 );

			texDispA = new TextureDisplay( Mem.Textures.BlankWhite );
			texDispA.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.05f ), posA ), 0 );
			Render2.UIManager.Add( texDispA );

			texDispB = new TextureDisplay( Mem.Textures.BlankWhite );
			texDispB.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.05f ), posB ), 0 );
			Render2.UIManager.Add( texDispB );

			label = new TextLabel();
			label.SetParent( texDisp );
			label.SetStencil( Render2.UIManager.Stencils.CreateNew( Window ) );
			label.Stencil.AddElement( texDisp );
			label.Stencil.AddElement( texDisp2 );
			label.Content = "testing, lol";
			label.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.35f ), new ModTranslationAdd( new Vector2( -0.5f, 0 ) ) ), 0 );

			label2 = new TextLabel();
			label2.Content = "testing, lol";
			Render2.UIManager.Add( label2 );
			label2.Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.05f ), new ModTranslationAdd( new Vector2( -1, .5f ) ) ), 0 );
			label2.Attributes.SetMaxLength( 32.0f );*/
			#endregion
		}

		protected override void InLoop() {
			//attract.Point = Window.EventHandler.Mouse.Data.PositionNDCA;
			//world.Update();
			StandardPipeline.Exectute();
		}
	}
}
