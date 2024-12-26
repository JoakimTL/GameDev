using CollisionDetDev3.Collision;
using Engine.Entities;
using Engine.Entities.D3;
using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Materials;
using Engine.Graphics.Objects.Default.Meshes.VertexMeshing;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Physics.D3.Shapes;
using Engine.QuickstartKit;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Tools;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3 {
	public class Entry : QuickstartClientRender {

		private static List<string> debug = new List<string>();
		public QSCameraHandler debugViewManipulator;
		public CollisionModKeyHandler keyHandler;

		private Mesh shape1Mesh;
		private Mesh shape2Mesh;
		private Entity shape1Render;
		private Entity shape2Render;

		private Shape3Polygon shape1;
		private Shape3Polygon shape2;

		private PointMesh3 simplexPointMesh;
		private Entity simplexPointRender;
		private LineMesh3 simplexLineMesh;
		private Entity simplexLineRender;
		private Mesh3 faceMesh;
		private Entity faceRender;
		private PointMesh3 minkPointMesh;
		private Entity minkLineRender;
		private Simplex3 simplex;
		private Simplex3Faces simplexFaces;

		private Entity originRender;

		private bool lockOnCollision;
		private bool locked;
		private bool log1;

		private int viewedSimplex;
		private bool viewLast;

		private Vector4i[] colors = new Vector4i[] {
			(255, 0, 0, 255),
			(0, 255, 0, 255),
			(0, 0, 255, 255),
			(255, 255, 0, 255),
			(255, 0, 255, 255),
			(0, 255, 255, 255)
		};

		public Entry() : base( new QSWinData(), new QS3Data(), new QS2Data() { frameTimer = true } ) {
		}

		protected override void Initialization() {
			debugViewManipulator = new QSCameraHandler( this );
			keyHandler = new CollisionModKeyHandler( this );

			Material defaultMat = new PBRMaterial( "def", Mem.Textures.BlankWhite, Mem.Textures.BlankBlack, Mem.Textures.BlankWhite, Mem.Textures.BlankWhite );

			//mold1 = new Shape3Polygon.Mold();

			shape1Mesh = Mem.Mesh3.Cube;
			shape1Render = EntityFactory.CreateBlank3Rendered( "shape1", shape1Mesh, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( shape1Render.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
				Collision3Module cm;
				shape1Render.Add( cm = new Collision3Module() );
				shape1 = Mem.CollisionMolds.CubeUniform.MoldNew( cm.Model );
				if( shape1Render.Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = (3.5f, 3.5f, 3.5f);
					tm.Transform.Scale = 0.5f;
				}
			}

			MeshGenerator.GenerateIcosphereVectors( 2, out List<Vector3> vec, out _ );
			Shape3Polygon.Mold mold2 = new Shape3Polygon.Mold( vec );
			shape2Mesh = MeshGenerator.GenerateIcosphere( 2, false );
			shape2Render = EntityFactory.CreateBlank3Rendered( "shape2", shape1Mesh, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( shape2Render.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
				Collision3Module cm;
				shape2Render.Add( cm = new Collision3Module() );
				shape2 = Mem.CollisionMolds.CubeUniform.MoldNew( cm.Model );
				if( shape2Render.Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = (3.5f, 4.9f, 3.5f);
					tm.Transform.Scale = (0.3f, 0.9f, 0.3f);
				}
			}

			simplexPointMesh = new PointMesh3( "simplexPointMesh" );
			simplexPointMesh.SetRadius( 20 );
			simplexPointRender = EntityFactory.CreateBlank3Rendered( "simplexPoint", simplexPointMesh, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( simplexPointRender.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
			}

			simplexLineMesh = new LineMesh3( "simplexLineMesh" );
			simplexLineMesh.SetWidth( 5 );
			simplexLineRender = EntityFactory.CreateBlank3Rendered( "simplexLine", simplexLineMesh, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( simplexLineRender.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
			}

			minkPointMesh = new PointMesh3( "minkPointMesh" );
			minkPointMesh.SetRadius( 5 );
			minkLineRender = EntityFactory.CreateBlank3Rendered( "minkLine", minkPointMesh, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( minkLineRender.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
			}

			PointMesh3 origin = new PointMesh3( "originMesh", new List<PointVertex3>() { new PointVertex3( 0 ) } );
			origin.SetRadius( 25 );
			originRender = EntityFactory.CreateBlank3Rendered( "origin", origin, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( originRender.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
			}

		}

		public void Unlock() {
			locked = !locked;
		}

		public void LockToggle() {
			lockOnCollision = !lockOnCollision;
		}

		public static void AddDebug( string s ) {
			debug.Add( s );
		}

		public override void EventsPolled() {

		}

		public void DecreaseView() {
			viewedSimplex--;
		}

		public void IncreaseView() {
			viewedSimplex++;
		}

		public void ToggleViewLast() {
			viewLast = !viewLast;
		}

		public void LogOne() {
			log1 = true;
		}

		private void CreateMinkowskiDisplay() {
			PointVertex3[] s = new PointVertex3[ shape1.Size * shape2.Size ];

			Console.WriteLine( s.Length );

			int ind = 0;
			for( int i = 0; i < shape1.Size; i++ ) {
				for( int j = 0; j < shape2.Size; j++ ) {
					s[ ind++ ] = new PointVertex3( shape1.GetTransformedPoint( i ) - shape2.GetTransformedPoint( j ), (0, 0, 0, 255) );
				}
			}

			minkPointMesh.Create( new List<PointVertex3>( s ) );
		}

		float ii = 0;
		protected override void InLoop() {
			if( shape1Render.Get( out Render3Module rm1 ) )
				rm1.RenderObject.Active = !Window.EventHandler.Keyboard[ Keys.X ];
			if( shape2Render.Get( out Render3Module rm2 ) )
				rm2.RenderObject.Active = !Window.EventHandler.Keyboard[ Keys.X ];
			/*if( !locked || Window.EventHandler.Keyboard[ Engine.GLFrameWork.Keys.N ] ) {
				if( shape1Render.Get( out Transform3Module tm ) ) {
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitY, 0.01f / 5 ) * tm.Transform.Rotation ).Normalized;
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitX, 0.03f / 5 ) * tm.Transform.Rotation ).Normalized;
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( tm.Transform.Rotation.Forward, 0.02f / 5 ) * tm.Transform.Rotation ).Normalized;
				}
			}*/

			if( !locked || Window.EventHandler.Keyboard[ Engine.GLFrameWork.Keys.B ] ) {
				if( shape2Render.Get( out Transform3Module tm ) ) {
					ii += 0.0001f;
					tm.Transform.Translation = (3.5f + (float) Math.Cos( ii * 20 ), 4.89f, 3.5f + (float) Math.Sin( ii ));
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitY, 0.01f ) * tm.Transform.Rotation ).Normalized;
					//tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitX, 0.03f ) * tm.Transform.Rotation ).Normalized;
					//tm.Transform.Rotation = ( Quaternion.FromAxisAngle( tm.Transform.Rotation.Forward, 0.02f ) * tm.Transform.Rotation ).Normalized;
				}
			}

			{
				if( shape1Render.Get( out Collision3Module cm ) )
					cm.Model.UpdateShapes();
			}
			{
				if( shape2Render.Get( out Collision3Module cm ) )
					cm.Model.UpdateShapes();
			}

			CreateMinkowskiDisplay();

			simplex = null;
			simplexFaces = null;

			Vector3 colA = 0, colB = 0;

			Engine.Physics.Collision.CollisionResult<Vector3> res2 = new Engine.Physics.Collision.CollisionResult<Vector3>();
			Engine.Physics.Collision.CollisionChecker.CheckCollision( shape1, shape2, res2, true );
			Collision3.IsColliding( ref simplex, ref simplexFaces, shape1, shape2, out Collision3Result res, out List<Simplex3> history, out List<Simplex3Faces> history2, log1 );
			if( res2.Colliding ) {// Collision3.IsColliding( ref simplex, ref simplexFaces, shape1, shape2, out Collision3Result res, out List<Simplex3> history, out List<Simplex3Faces> history2, log1 ) ) {
				if( shape1Render.Get( out Render3Module rm ) )
					rm.RenderObject.Data.Color = (1, 0, 0, 1);
				if( lockOnCollision )
					locked = true;
				var d = res2.GetDeepest();
				colA = d.A;
				colB = d.B;
				debug.Add( "Colliding: " + d.Normal + ", " + d.Depth + " -> " + ( colA - colB ).Length );
			} else {
				if( shape1Render.Get( out Render3Module rm ) )
					rm.RenderObject.Data.Color = (1, 1, 1, 1);
			}
			log1 = false;
			if( viewedSimplex >= history.Count )
				viewedSimplex = history.Count - 1;
			if( viewLast )
				viewedSimplex = history.Count - 1;
			if( viewedSimplex < 0 )
				viewedSimplex = 0;
			debug.Add( viewedSimplex.ToString() + "/" + ( history.Count - 1 ).ToString() );
			if( history.Count > 0 ) {
				Simplex3 simplex = history[ viewedSimplex ];
				Simplex3Faces faces = history2[ viewedSimplex ];
				debug.Add( this.simplex.Count.ToString() );
				debug.Add( simplex.Count.ToString() );
				List<PointVertex3> points = new List<PointVertex3>();
				List<LineVertex3> linePoints = new List<LineVertex3>();
				List<int> lineIndices = new List<int>();
				for( int i = 0; i < simplex.Count; i++ ) {
					points.Add( new PointVertex3( simplex[ i ].Sum, colors[ i % colors.Length ] ) );
					linePoints.Add( new LineVertex3( simplex[ i ].Sum, colors[ i % colors.Length ] ) );
					if( faces is null ) {
						for( int j = i + 1; j < simplex.Count; j++ ) {
							lineIndices.Add( i );
							lineIndices.Add( j );
						}
					}
				}

				if( !( faces is null ) ) {
					for( int i = 0; i < faces.Count; i++ ) {
						lineIndices.Add( faces[ i ].A );
						lineIndices.Add( faces[ i ].B );
						lineIndices.Add( faces[ i ].B );
						lineIndices.Add( faces[ i ].C );
						lineIndices.Add( faces[ i ].C );
						lineIndices.Add( faces[ i ].A );
						Vector3 o = ( simplex[ faces[ i ].A ].Sum + simplex[ faces[ i ].B ].Sum + simplex[ faces[ i ].C ].Sum ) / 3;
						Vector3 c = Vector3.Cross( ( simplex[ faces[ i ].B ].Sum - simplex[ faces[ i ].A ].Sum ).Normalized, ( simplex[ faces[ i ].C ].Sum - simplex[ faces[ i ].A ].Sum ).Normalized );
						points.Add( new PointVertex3( o, colors[ i % colors.Length ] ) );
						linePoints.Add( new LineVertex3( o, (0, 0, 0, 255) ) );
						linePoints.Add( new LineVertex3( o + c, colors[ i % colors.Length ] ) );
						lineIndices.Add( linePoints.Count - 2 );
						lineIndices.Add( linePoints.Count - 1 );
					}
					linePoints.Add( new LineVertex3( faces.SearchOrigin, (0, 0, 0, 255) ) );
					linePoints.Add( new LineVertex3( faces.SearchOrigin + faces.SearchDirection.Normalized, (122, 56, 255, 255) ) );
				} else {
					linePoints.Add( new LineVertex3( simplex.SearchOrigin, (0, 0, 0, 255) ) );
					linePoints.Add( new LineVertex3( simplex.SearchOrigin + simplex.SearchDirection.Normalized, (255, 56, 122, 255) ) );
				}

				lineIndices.Add( linePoints.Count - 2 );
				lineIndices.Add( linePoints.Count - 1 );

				linePoints.Add( new LineVertex3( colA, (255, 0, 0, 255) ) );
				linePoints.Add( new LineVertex3( colB, (0, 255, 0, 255) ) );
				lineIndices.Add( linePoints.Count - 2 );
				lineIndices.Add( linePoints.Count - 1 );

				simplexPointMesh.Create( points );
				simplexLineMesh.Create( linePoints, lineIndices );
			}

			debug.Add( lockOnCollision.ToString() );
			debug.Add( Render3.Camera.TranformInterface.Rotation.Forward.ToString() );
			debug.Add( "\n" );
			debug.Add( colA.ToString() );
			debug.Add( colB.ToString() );
			string s = "";
			for( int i = 0; i < debug.Count; i++ ) {
				s += debug[ i ] + ", ";
			}
			debug.Clear();
			Render2.SetDebugString( s );

			StandardPipeline.Exectute();
		}

		protected override void Exit() {

		}
	}
}
