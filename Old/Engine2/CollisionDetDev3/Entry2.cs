using CollisionDetDev3.Collision;
using Engine.Entities;
using Engine.Entities.D3;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3 {
	public class Entry2 : QuickstartClientRender {

		private static List<string> debug = new List<string>();
		public QSCameraHandler debugViewManipulator;

		private Entity mainShapeRender;
		private Shape3Polygon shape1;
		private List<Entity> trash;
		private List<Shape3Polygon> trashShapes;
		private Shape3Polygon sphere;

		public Entry2() : base( new QSWinData() { vsyncLevel = 0 }, new QS3Data(), new QS2Data() { frameTimer = true } ) {
		}

		protected override void Initialization() {
			debugViewManipulator = new QSCameraHandler( this );

			Material defaultMat = new PBRMaterial( "def", Mem.Textures.BlankWhite, Mem.Textures.BlankBlack, Mem.Textures.BlankWhite, Mem.Textures.BlankWhite );

			//mold1 = new Shape3Polygon.Mold();

			trash = new List<Entity>();
			trashShapes = new List<Shape3Polygon>();

			mainShapeRender = EntityFactory.CreateBlank3Rendered( "shape1", Mem.Mesh3.Cube, defaultMat, Mem.ShaderBundles.Entity3 );
			{
				if( mainShapeRender.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
				Collision3Module cm;
				mainShapeRender.Add( cm = new Collision3Module() );
				shape1 = Mem.CollisionMolds.CubeUniform.MoldNew( cm.Model );
			}

			MeshGenerator.GenerateIcosphereVectors( 2, out List<Vector3> vec, out _ );
			Shape3Polygon.Mold mold2 = new Shape3Polygon.Mold( vec );
			Random r = new Random();
			for( int i = 0; i < 256; i++ ) {
				Entity n = EntityFactory.CreateBlank3Rendered( "shape#" + i, Mem.Mesh3.Cube, defaultMat, Mem.ShaderBundles.Entity3 );
				if( n.Get( out Render3Module rm ) ) {
					Render3.Scenes.Entity.Add( rm.RenderObject );
				}
				Collision3Module cm;
				n.Add( cm = new Collision3Module() );
				trashShapes.Add( Mem.CollisionMolds.CubeUniform.MoldNew( cm.Model ) );
				if( n.Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = ((float) r.NextDouble() * 20 - 10, (float) r.NextDouble() * 20 - 10, (float) r.NextDouble() * 20 - 10);
					tm.Transform.Scale = (0.25f, 0.25f, 0.25f);
				}
				trash.Add( n );
			}

		}

		public static void AddDebug( string s ) {
			debug.Add( s );
		}

		public override void EventsPolled() {

		}

		protected override void InLoop() {

			{
				if( mainShapeRender.Get( out Transform3Module tm ) ) {
					tm.Transform.Translation = Render3.Camera.TranformInterface.Translation + Render3.Camera.TranformInterface.Rotation.Forward * 5;
					tm.Transform.Rotation = Render3.Camera.TranformInterface.Rotation;
				}
				if( mainShapeRender.Get( out Collision3Module cm ) )
					cm.Model.UpdateShapes();
			}

			//Random r = new Random();
			for( int i = 0; i < trash.Count; i++ ) {
				/*if( trash[ i ].Get( out Transform3Module tm ) ) {
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitY, (float) r.NextDouble() * 0.05f ) * tm.Transform.Rotation ).Normalized;
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( Vector3.UnitX, (float) r.NextDouble() * 0.05f ) * tm.Transform.Rotation ).Normalized;
					tm.Transform.Rotation = ( Quaternion.FromAxisAngle( tm.Transform.Rotation.Forward, (float) r.NextDouble() * 0.05f ) * tm.Transform.Rotation ).Normalized;
				}*/
				if( trash[ i ].Get( out Collision3Module cm ) )
					cm.Model.UpdateShapes();
			}

			for( int i = 0; i < trash.Count; i++ ) {
				for( int j = i + 1; j < trash.Count; j++ ) {
					if( Collision3.IsColliding( trashShapes[ i ], trashShapes[ j ], out Collision3Result res ) ) {
						if( trash[ j ].Get( out Transform3Module tm ) )
							tm.Transform.Translation += res.PenetrationDirection * res.Depth;
					}
				}
			}

			for( int i = 0; i < trash.Count; i++ ) {
				if( Collision3.IsColliding( shape1, trashShapes[ i ], out Collision3Result res ) ) {
					if( trash[ i ].Get( out Transform3Module tm ) )
						tm.Transform.Translation += res.PenetrationDirection * res.Depth;
				}
			}

			debug.Add( Render3.Camera.TranformInterface.Rotation.Forward.ToString() );
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
