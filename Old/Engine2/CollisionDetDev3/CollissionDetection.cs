using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CollisionDetDev3.Collision;
using System.Runtime.InteropServices;
using Engine.Physics.D3;
using Engine.Physics.D3.Shapes;
using Engine.Physics;

namespace CollisionDetDev3 {
	public partial class CollisionDetection : Form {

		private Timer timer;
		private DBPanel panel;

		private Vector3 scale = (160, 160, 1);
		private Perspective projection;
		private Transform3 camTransform;
		private View3 camera;

		private Vector3 mouse;

		private Shape3Polygon.Mold mold1;
		private Transform3 transform1;
		private Physics3Model model1;
		private Shape3Polygon shape1;
		private Shape3Polygon.Mold mold2;
		private Transform3 transform2;
		private Physics3Model model2;
		private Shape3Polygon shape2;

		private Shape3Polygon.Mold mink;
		private Physics3Model minkModel;
		private Shape3Polygon minkShape;

		private Simplex3 simplex;
		private Simplex3Faces simplexFaces;
		private Shape3Polygon.Mold simplexMold;
		private Physics3Model simplexModel;
		private Shape3Polygon simplexShape;
		private Collision3Result res;

		private List<Physics3Model> models;
		private List<ImmutableDuo<Color, Shape3Polygon>> drawnShapes;

		private Pen[] pens;
		private Brush[] brushes;

		private bool displayMinkowski, lockedPosition, updateConstant;

		public CollisionDetection() {
			InitializeComponent();
		}

		private void LoadForm( object sender, EventArgs e ) {
			timer = new Timer();
			timer.Interval = 30;
			timer.Tick += REEE;
			timer.Enabled = true;

			panel = new DBPanel();
			panel.Paint += RefreshPanel;

			Controls.Add( panel );

			mold1 = new Shape3Polygon.Mold();
			mold1.Add( (-1, -1, -1) );
			mold1.Add( (1, -1, -1) );
			mold1.Add( (1, 1, -1) );
			mold1.Add( (-1, 1, -1) );
			mold1.Add( (-1, -1, 1) );
			mold1.Add( (1, -1, 1) );
			mold1.Add( (1, 1, 1) );
			mold1.Add( (-1, 1, 1) );
			mold2 = new Shape3Polygon.Mold();
			mold2.Add( (-1, -1, -1) );
			mold2.Add( (1, -1, -1) );
			mold2.Add( (1, 1, -1) );
			mold2.Add( (-1, 1, -1) );
			mold2.Add( (-1, -1, 1) );
			mold2.Add( (1, -1, 1) );
			mold2.Add( (1, 1, 1) );
			mold2.Add( (-1, 1, 1) );

			/*
			for( int i = 0; i < 30; i++ ) {
				double a = i / 30f * Math.PI * 2;
				mold1.Add( ((float) Math.Cos( a ), (float) Math.Sin( a ), 0) );
			}*/
			/*for( int i = 0; i < 30; i++ ) {
				double a = i / 30f * Math.PI * 2;
				mold2.Add( ((float) Math.Cos( a ), (float) Math.Sin( a )) );
			}*/

			mink = new Shape3Polygon.Mold();

			brushes = new Brush[] {
				new SolidBrush(Color.Red),
				new SolidBrush(Color.Green),
				new SolidBrush(Color.Blue),
				new SolidBrush(Color.Yellow),
				new SolidBrush(Color.Cyan),
				new SolidBrush(Color.Purple),
				new SolidBrush(Color.Gray)
			};
			pens = new Pen[] {
				new Pen(Color.Red, 3),
				new Pen(Color.Green, 3),
				new Pen(Color.Blue, 3),
				new Pen(Color.Yellow, 3),
				new Pen(Color.Cyan, 3),
				new Pen(Color.Purple, 3),
				new Pen(Color.Gray, 3)
			};

			transform1 = new Transform3();
			shape1 = new Shape3Polygon( 1, mold1 );
			model1 = new Physics3Model( "m1", transform1 );
			model1.Add( shape1 );
			transform2 = new Transform3();
			shape2 = new Shape3Polygon( 1, mold2 );
			model2 = new Physics3Model( "m2", transform2 );
			model2.Add( shape2 );
			transform2.Translation = (0, -1.6f, -1.6f);
			transform2.Scale = 0.5f;

			minkShape = new Shape3Polygon( 1, mink );
			minkModel = new Physics3Model( "mink", new Transform3() );
			minkModel.Add( minkShape );

			simplexMold = new Shape3Polygon.Mold();
			simplexShape = new Shape3Polygon( 1, simplexMold );
			simplexModel = new Physics3Model( "simplex", new Transform3() );
			simplexModel.Add( simplexShape );

			camTransform = new Transform3();
			camTransform.Translation = (0, 0, 2);
			projection = new Perspective( "lmao", 90, GetAspect() );
			camera = new View3( projection, camTransform );

			models = new List<Physics3Model>();
			models.Add( model1 );
			models.Add( model2 );

			drawnShapes = new List<ImmutableDuo<Color, Shape3Polygon>>();
			drawnShapes.Add( new ImmutableDuo<Color, Shape3Polygon>( Color.Red, shape1 ) );
			drawnShapes.Add( new ImmutableDuo<Color, Shape3Polygon>( Color.Green, shape2 ) );
			drawnShapes.Add( new ImmutableDuo<Color, Shape3Polygon>( Color.Blue, minkShape ) );
			//drawnShapes.Add( new ImmutableDuo<Color, Shape2Polygon>( Color.DimGray, simplexShape ) );

			this.Resize += Resized;
			Resized( null, null );

			lockedPosition = true;

			panel.MouseMove += MouseMoved;
			panel.MouseDown += MouseButtonDown;
			panel.MouseUp += MouseButtonUp;
			this.KeyDown += KeyDownhandle;
			this.KeyUp += KeyUpHandle;
		}

		private Vector2 lastMouse = 0;
		public Dictionary<MouseButtons, bool> mouseB = new Dictionary<MouseButtons, bool>();
		public Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();

		private void MouseButtonUp( object sender, MouseEventArgs e ) {
			mouseB[ e.Button ] = false;
		}

		private void MouseButtonDown( object sender, MouseEventArgs e ) {
			mouseB[ e.Button ] = true;
		}

		private void KeyDownhandle( object sender, KeyEventArgs e ) {
			keys[ e.KeyCode ] = true;
			if( e.KeyCode == Keys.M ) {
				displayMinkowski = !displayMinkowski;
			}
			if( e.KeyCode == Keys.Q ) {
				lockedPosition = !lockedPosition;
			}
			if( e.KeyCode == Keys.E ) {
				int val = Collision3.PerformStep( ref simplex, ref simplexFaces, shape1, shape2, out Collision3Result res, true, 0, out _, out _ );
				if( val == 1 ) {
					Console.WriteLine( "collision!" );
					//Console.WriteLine( res.Depth + ", " + res.PenetrationDirection );
				} else if( val == -1 ) {
					Console.WriteLine( "No collision!" );
				}
			}
			if( e.KeyCode == Keys.F ) {
				//if( !( simplex is null ) )
				//simplex.MaintainConvexity( true );
			}
			if( e.KeyCode == Keys.R ) {
				simplex = null;
				simplexFaces = null;
			}
			if( e.KeyCode == Keys.T )
				updateConstant = !updateConstant;
		}

		private void KeyUpHandle( object sender, KeyEventArgs e ) {
			keys[ e.KeyCode ] = false;
		}

		private void MouseMoved( object sender, MouseEventArgs e ) {
			Vector2 deltaMouse = lastMouse - new Vector2( e.X, e.Y );
			lastMouse = new Vector2( e.X, e.Y );
			mouse = Engine.Utilities.Data.Unprojector.GetMouseUnprojected( camera, ( new Vector2( e.X, e.Y ) - GetCenter().XY ) / GetCenter().XY ) * 2;

			if( mouseB.TryGetValue( MouseButtons.Right, out bool v ) && v ) {
				Vector2 mov = deltaMouse / GetCenter().XY;
				camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, mov.X / 4 * (float) Math.PI ) * camera.TranformInterface.Rotation;
				camera.TranformInterface.Rotation = camera.TranformInterface.Rotation.Normalized;
				camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( camera.TranformInterface.Rotation.Right, -mov.Y / 4 * (float) Math.PI ) * camera.TranformInterface.Rotation;
				camera.TranformInterface.Rotation = camera.TranformInterface.Rotation.Normalized;
			}
		}

		private void Resized( object sender, EventArgs e ) {
			panel.Size = this.ClientSize;
			projection.SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( 90f / 180 * (float) Math.PI, GetAspect(), 0.00390625f, 256 ) );
		}

		private float GetAspect() {
			return (float) panel.Width / panel.Height;
		}
		private Vector3 GetCenter() {
			return (panel.Width / 2f, panel.Height / 2f, 0);
		}

		private void REEE( object sender, EventArgs e ) {
			panel.Refresh();
		}

		private void UpdateSimplexDisplay() {
			if( simplex is null ) {
				simplexMold.Set( new PointMass<Vector3>[ 0 ] );
				return;
			}
			PointMass<Vector3>[] s = new PointMass<Vector3>[ simplex.Count ];

			for( int i = 0; i < s.Length; i++ ) {
				s[ i ] = new PointMass<Vector3>( simplex[ i ].Sum, 1 );
			}

			simplexMold.Set( s );
		}

		private void CreateMinkowski( Shape3Polygon shapeA, Shape3Polygon shapeB ) {
			if( !displayMinkowski ) {
				mink.Set( new PointMass<Vector3>[ 0 ] );
				return;
			}
			PointMass<Vector3>[] s = new PointMass<Vector3>[ shapeA.Size * shapeB.Size ];

			int ind = 0;
			for( int i = 0; i < shapeA.Size; i++ ) {
				for( int j = 0; j < shapeB.Size; j++ ) {
					s[ ind++ ] = new PointMass<Vector3>( shapeA.GetTransformedPoint( i ) - shapeB.GetTransformedPoint( j ), 1 );
				}
			}

			mink.Set( s );
		}

		Font f = new Font( "arial", 12 );

		private void RefreshPanel( object sender, PaintEventArgs e ) {
			e.Graphics.Clear( Color.Black );

			/*camTransform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, 0.03f ) * camTransform.Rotation;
			camTransform.Rotation = camTransform.Rotation.Normalized;
			*/
			float speed = 0.1f;
			{
				if( keys.TryGetValue( Keys.S, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Backward * speed;
				}
			}
			{
				if( keys.TryGetValue( Keys.A, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Left * speed;
				}
			}
			{
				if( keys.TryGetValue( Keys.W, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Forward * speed;
				}
			}
			{
				if( keys.TryGetValue( Keys.D, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Right * speed;
				}
			}
			{
				if( keys.TryGetValue( Keys.ControlKey, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Up * speed;
				}
			}
			{
				if( keys.TryGetValue( Keys.Space, out bool v ) && v ) {
					camera.TranformInterface.Translation += camera.TranformInterface.Rotation.Down * speed;
				}
			}

			camera.UpdateMatrices();

			if( !lockedPosition )
				transform2.Translation = camera.TranformInterface.Translation + camera.TranformInterface.Rotation.Forward * 3;

			for( int i = 0; i < models.Count; i++ ) {
				models[ i ].UpdateShapes();
			}

			CreateMinkowski( shape1, shape2 );
			minkModel.UpdateShapes();

			UpdateSimplexDisplay();
			simplexModel.UpdateShapes();

			for( int i = 0; i < drawnShapes.Count; i++ ) {
				Shape3Polygon shape = drawnShapes[ i ].ValueB;
				Brush b = new SolidBrush( drawnShapes[ i ].ValueA );
				Pen pen = new Pen( drawnShapes[ i ].ValueA, 4 );
				for( int j = 0; j < shape.Size; j++ ) {
					Vector3 p = shape.GetTransformedPoint( j );
					Vector3 p2 = shape.GetTransformedPoint( ( j + 1 ) % shape.Size );
					Vector3 tp = Vector3.TransformGraphics( p, camera.VPMatrix, out bool behind1 );
					Vector3 tp2 = Vector3.TransformGraphics( p2, camera.VPMatrix, out bool behind2 );
					if( behind1 || behind2 )
						continue;

					tp *= GetCenter();
					tp += GetCenter();
					tp2 *= GetCenter();
					tp2 += GetCenter();
					float s = 10;
					e.Graphics.FillEllipse( b, tp.X - s / 2, tp.Y - s / 2, s, s );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( tp.X + s / 2 ), (int) ( tp.Y + s / 2 ) );
					e.Graphics.DrawLine( pen, (int) tp.X, (int) tp.Y, (int) tp2.X, (int) tp2.Y );
				}
			}

			if( !( simplex is null ) ) {
				for( int j = 0; j < simplex.Count; j++ ) {
					int k = j + 1;
					k = k == simplex.Count ? 0 : k;
					Vector3 p = simplex[ j ].Sum;
					Vector3 p2 = simplex[ k ].Sum;
					Vector3 tp = Vector3.TransformGraphics( p, camera.VPMatrix, out bool behind1 );
					Vector3 tp2 = Vector3.TransformGraphics( p2, camera.VPMatrix, out bool behind2 );
					if( behind1 || behind2 )
						continue;

					tp *= GetCenter();
					tp += GetCenter();
					tp2 *= GetCenter();
					tp2 += GetCenter();
					e.Graphics.DrawLine( pens[ j % pens.Length ], (int) tp.X, (int) tp.Y, (int) tp2.X, (int) tp2.Y );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( ( tp.X + tp2.X ) / 2 ), (int) ( ( tp.Y + tp2.Y ) / 2 ) );
				}
				for( int j = 0; j < simplex.Count; j++ ) {
					Vector3 p = simplex[ j ].Sum;
					Vector3 tp = Vector3.TransformGraphics( p, camera.VPMatrix, out bool behind1 );
					if( behind1 )
						continue;

					tp *= GetCenter();
					tp += GetCenter();
					float s = 10;
					e.Graphics.FillEllipse( brushes[ j % brushes.Length ], tp.X - s / 2, tp.Y - s / 2, s, s );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( tp.X + s / 2 ), (int) ( tp.Y + s / 2 ) );
				}
			}

			Simplex3 simm = null;
			Simplex3Faces simmF = null;
			if( updateConstant )
				if( Collision3.IsColliding( ref simm, ref simmF, shape1, shape2, out _, out _, out _ ) ) {
					if( res is null ) {
						e.Graphics.DrawString( "Collision", f, origoBrush, 0, 0 );
					}/* else {
						Vector3 p = transform2.Translation.XY + res.PenetrationDirection * res.Depth;
						Vector3 p2 = transform2.Translation.XY * scale;
						Vector3 tp = Vector3.Transform( p * scale, camera.VPMatrix );
						Vector3 tp2 = Vector3.Transform( p2, camera.VPMatrix );
						tp *= new Vector3( GetAspect(), 1, 1 );
						tp *= GetCenter();
						tp += GetCenter();
						tp2 *= new Vector3( GetAspect(), 1, 1 );
						tp2 *= GetCenter();
						tp2 += GetCenter();
						e.Graphics.DrawLine( new Pen( Color.White, 5 ), (int) tp.X, (int) tp.Y, (int) tp2.X, (int) tp2.Y );
					}*/
				} else {
					e.Graphics.DrawString( "No collision", f, origoBrush, 0, 0 );
				}

			e.Graphics.DrawString( "Mouse: " + mouse, f, origoBrush, 0, 60 );
			e.Graphics.FillEllipse( origoBrush, GetCenter().X - 1, GetCenter().Y - 1, 2, 2 );

			{
				Vector3 to = Vector3.TransformGraphics( 0, camera.VPMatrix, out bool behind1 );
				if( !behind1 ) {
					to *= GetCenter();
					to += GetCenter();
					float s = 10;
					e.Graphics.FillEllipse( origoBrush, to.X - s / 2, to.Y - s / 2, s, s );
				}
			}

		}

		private SolidBrush origoBrush = new SolidBrush( Color.White );

		private void Keypress( object sender, KeyPressEventArgs e ) {

		}
	}
}
