using Engine.Entities;
using Engine.Entities.D3;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.Graphics.Objects.Default.Materials;
using Engine.Graphics.Objects.Default.Meshes.VertexMeshing;
using Engine.Graphics.Objects.Default.Particles;
using Engine.Graphics.Objects.Default.Particles.D3;
using Engine.Graphics.Objects.Default.Particles.D3.Behaviours;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Networking;
using Engine.Networking.Client;
using Engine.QuickstartKit;
using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Graphics.Utilities;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VoxDev;
using VoxDev.UIScenes;
using VoxDev.VoxelRendering;
using VoxDev.Voxels;
using VoxDevClient.Rendering.UI;

namespace VoxDevClient {
	class Entry : QuickstartClientRender {

		private VoxelMouseHandler voxelMouseHandler;
		private VoxelWorld world;
		private VoxelWorldRenderer worldRender;
		private LightDirectional light;
		private NoisePattern noise;
		private NoisePattern noise2;
		private NoisePattern noise3;
		private PlayerEntity player;
		private static List<string> debug = new List<string>();

		private UIController menuController;

		private EntityManagerUpdater emu;
		private Entity3Manager eM;
		private Entity e;

		private ClientConnection client;


		public Entry() : base( new QSWinData(), new QS3Data(), new QS2Data() { frameTimer = true } ) {
		}

		protected override void Initialization() {
			noise = new NoisePattern( 8643, 0, 8, 8, InterpolationMethods.CosineInterpolation );
			noise3 = new NoisePattern( 1223, 0, 1, 2, InterpolationMethods.LinearInterpolation );
			noise2 = new NoisePattern( 2311, 2, 2, 2, InterpolationMethods.LinearInterpolation );
			world = new VoxelWorld( "test", new VoxelWorldGenerator( GenFunc, 4 ),
				new TestBlockSet( "test",
					Mem.Textures.Get( "voxMap", TextureMagFilter.Nearest ),
					Mem.Textures.Get( "voxMapNormal", TextureMagFilter.Nearest ),
					Mem.Textures.BlankWhite,
					Mem.Textures.BlankBlack ),
				0.25f, false
			);
			world.SetBounds( 0, (1, -1, 1) );
			world.BlockSet.Material.Properties.SetNormalMapped( true );
			worldRender = new VoxelWorldRenderer( world, Render3.Scenes.Entity, Render3.Camera );

			light = new LightDirectional( (0.9922f, 0.9843f, 0.8275f, 1f), 23, -25 );
			Render3.Lights.Add( light );

			Material m = new PBRMaterial( "blank", Mem.Textures.BlankWhite, Mem.Textures.BlankBlack, Mem.Textures.BlankWhite, Mem.Textures.BlankWhite );

			player = new PlayerEntity( "player", Render3.Camera, Window.EventHandler, Render3.Scenes.Entity, world );

			voxelMouseHandler = new VoxelMouseHandler( Render3.Camera, world );
			Window.EventHandler.Mouse.Add( voxelMouseHandler );

			emu = new EntityManagerUpdater( Clock32.Standard );
			eM = new Entity3Manager( Render3.Scenes.Entity );
			eM.Add( player );
			{
				player.Get( out Transform3Module tm );
				tm.Transform.Translation = (0, 100, 0);
			}
			eM.Add( e = EntityFactory.CreateBlank3Rendered( "minkLine", Mem.Mesh3.Cube, m, Mem.ShaderBundles.Entity3 ) );
			{
				{
					if( e.Get( out Transform3Module tm ) ) {
						tm.Transform.Scale = (1, 1, 1);
						tm.Transform.Translation = (0, 70, 0);
					}
				}
				if( e.Get( out Render3Module rm ) ) {
					e.Add( new Collision3Module( rm.RenderObject.Data.CollisionModel ) );
					Mem.CollisionMolds.CubeUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
					Mass3Module mm = new Mass3Module( 1f );
					e.Add( mm );
					e.Add( new CustomRigidbodyModule( world, rm.RenderObject.Data.CollisionModel ) );
					//mm.Mass.ApplyForce( (30.3f, 0, 0.3f), (0, 1, 0) );
					//mm.Mass.ApplyForce( (0, 0, 0), (0, -1, 0) );
				}
				{
					player.Get( out Transform3Module tm );
					//e.Add( new StiggiModule( tm.Transform ) );
				}
			}
			emu.Add( eM );

			menuController = new UIController( Render2.UIManager );
			menuController.ConnectionRequest += OnConnectionRequest;
			menuController.HostRequest += OnHostRequest;
			menuController.ExitClicked += OnExitRequest;

			client = new ClientConnection();

		}

		private void OnExitRequest() {
			Running = false;
		}

		private void OnHostRequest( string username ) {
			//Start server and join
			client.Connection.ConnectAsClient( username, new IPEndPoint( IPAddress.Loopback, 12345 ) );
		}

		private void OnConnectionRequest( string username, string address ) {
			if( IPEndPoint.TryParse( address, out IPEndPoint endPoint ) )
				client.Connection.ConnectAsClient( username, endPoint );
		}

		private void UpdateWorld() {
			worldRender.BoundedUpdate( 0, (1, -1, 1) );
		}

		private void GenFunc( VoxelChunk chunk ) {
			for( int x = 0; x < VoxelChunk.SIZE; x++ ) {
				float xx = chunk.BlockPosition.X + x;
				for( int z = 0; z < VoxelChunk.SIZE; z++ ) {
					float zz = chunk.BlockPosition.Z + z;
					//float h = noise.GetValue( xx * world.TransformInterface.Scale.X, zz * world.TransformInterface.Scale.Z ) + noise3.GetValue( xx * world.TransformInterface.Scale.X, zz * world.TransformInterface.Scale.Z );
					int id = (int) Math.Round( noise2.GetValue( xx * world.TransformInterface.Scale.X, zz * world.TransformInterface.Scale.Z ) ) % 2 + 1;

					for( int y = 0; y < VoxelChunk.SIZE; y++ ) {
						float yy = chunk.BlockPosition.Y + y;
						chunk.SetIdUnchecked( x, y, z, ( ( yy * world.TransformInterface.Scale.Y < 0 ) ? (ushort) id : (ushort) 0 ) );
					}
				}
			}
		}

		public static void AddDebug( string s ) {
			debug.Add( s );
		}

		public override void EventsPolled() {
			//cameraHandler.CameraUpdate();
			voxelMouseHandler.CameraUpdate( Window.EventHandler );
		}

		private float lastTime = 0;
		protected override void InLoop() {
			UpdateWorld();
			emu.UpdateManagers();

			worldRender.wireframe = Window.EventHandler.Keyboard[ Engine.GLFrameWork.Keys.Q ];
			if( player.Get( out Transform3Module tm ) ) {
				debug.Add( tm.Transform.GlobalTranslation.ToString() );
				debug.Add( world.GetBlockPosition( tm.Transform.GlobalTranslation ).ToString() );
			}
			debug.Add( "\n" );
			if( e.Get( out Mass3Module mm ) ) {
				debug.Add( mm.Mass.GetInertia( (0, 1, 0) ).ToString() );
			}
			Entity3Region r = eM.GetRegion( e );
			if( !( r is null ) ) {
				debug.Add( r.Position.ToString() );
			} else {
				debug.Add( "null" );
			}

			string s = "";
			for( int i = 0; i < debug.Count; i++ ) {
				s += debug[ i ] + ", ";
			}
			debug.Clear();
			Render2.SetDebugString( s + Render3.Camera.TranformInterface.GlobalTranslation.ToString() );
			float time = Clock32.Standard.Time;
			lastTime = time;
			if( Window.EventHandler.Keyboard[ Engine.GLFrameWork.Keys.T ] )
				world.TransformInterface.Rotation = ( Quaternion.FromAxisVector( (0, 0.01f, 0) ) * world.TransformInterface.Rotation ).Normalized;

			StandardPipeline.Exectute();
		}

		protected override void Exit() {
			world.Unload( true );
		}
	}
}
