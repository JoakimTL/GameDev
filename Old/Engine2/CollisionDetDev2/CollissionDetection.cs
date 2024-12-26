using CollisionDetDev3.Collision;
using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using Engine.Physics.D2;
using Engine.Physics.D2.Shapes;
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

namespace CollisionDetDev3 {
	public partial class CollisionDetection : Form {

		private Timer timer;
		private DBPanel panel;

		private Vector2 scale = ( 160 );
		private Transform2 camTransform;

		private Vector2 mouse;

		private Shape2Polygon.Mold mold1;
		private Transform2 transform1;
		private Physics2Model model1;
		private Shape2Polygon shape1;
		private Shape2Polygon.Mold mold2;
		private Transform2 transform2;
		private Physics2Model model2;
		private Shape2Polygon shape2;

		private Shape2Polygon.Mold mink;
		private Physics2Model minkModel;
		private Shape2Polygon minkShape;

		private Hull2 simplex;
		private Shape2Polygon.Mold simplexMold;
		private Physics2Model simplexModel;
		private Shape2Polygon simplexShape;
		private Collision2Result res;

		private List<Physics2Model> models;
		private List<ImmutableDuo<Color, Shape2Polygon>> drawnShapes;

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

			mold1 = new Shape2Polygon.Mold();
			//mold1.Add( (-1, -1) );
			//mold1.Add( (1, -1) );
			//mold1.Add( (1, 1) );
			//mold1.Add( (-1.2f, 0) );
			//mold1.Add( (-1, 1) );
			mold2 = new Shape2Polygon.Mold();
			mold2.Add( (0, 0), 1 );
			mold2.Add( (0, 1), 1 );
			//mold2.Add( (-.4f, -.4f), 1 );
			//mold2.Add( (0.4f, 0.4f), 1 );


			for( int i = 0; i < 10; i++ ) {
				double a = i / 10F * Math.PI * 2;
				mold1.Add( ((float) Math.Cos( a ), (float) Math.Sin( a )), 1 );
			}
			/*for( int i = 0; i < 30; i++ ) {
				double a = i / 30f * Math.PI * 2;
				mold2.Add( ((float) Math.Cos( a ), (float) Math.Sin( a )) );
			}*/

			mink = new Shape2Polygon.Mold();

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

			transform1 = new Transform2();
			model1 = new Physics2Model( "m1", transform1 );
			shape1 = mold1.MoldNew( model1 );
			transform2 = new Transform2();
			model2 = new Physics2Model( "m2", transform2 );
			shape2 = mold2.MoldNew( model2 );

			minkModel = new Physics2Model( "mink", new Transform2() );
			minkShape = mink.MoldNew( minkModel );

			simplexMold = new Shape2Polygon.Mold();
			simplexModel = new Physics2Model( "simplex", new Transform2() );
			simplexShape = simplexMold.MoldNew( simplexModel );

			camTransform = new Transform2();
			camTransform.Translation = (0, 0);

			models = new List<Physics2Model>();
			models.Add( model1 );
			models.Add( model2 );

			drawnShapes = new List<ImmutableDuo<Color, Shape2Polygon>>();
			drawnShapes.Add( new ImmutableDuo<Color, Shape2Polygon>( Color.Red, shape1 ) );
			drawnShapes.Add( new ImmutableDuo<Color, Shape2Polygon>( Color.Green, shape2 ) );
			drawnShapes.Add( new ImmutableDuo<Color, Shape2Polygon>( Color.Blue, minkShape ) );
			//drawnShapes.Add( new ImmutableDuo<Color, Shape2Polygon>( Color.DimGray, simplexShape ) );

			this.Resize += Resized;
			Resized( null, null );

			panel.MouseMove += MouseMoved;
			this.KeyDown += KeyDownhandle;
		}

		private void KeyDownhandle( object sender, KeyEventArgs e ) {
			if( e.KeyCode == Keys.Q ) {
				displayMinkowski = !displayMinkowski;
			}
			if( e.KeyCode == Keys.W ) {
				lockedPosition = !lockedPosition;
			}
			if( e.KeyCode == Keys.E ) {
				if( Collision2.PerformStep( ref simplex, shape1, shape2, 0, out Collision2Result res, true ) == 1 ) {
					Console.WriteLine( res.Depth + ", " + res.PenetrationDirection );
				}
			}
			if( e.KeyCode == Keys.R ) {
				simplex = null;
			}
			if( e.KeyCode == Keys.T )
				updateConstant = !updateConstant;
		}

		private void MouseMoved( object sender, MouseEventArgs e ) {
			mouse = ( new Vector2( e.X, e.Y ) - GetCenter() ) / scale;
			if( !lockedPosition )
				transform2.Translation = mouse;
		}

		private void Resized( object sender, EventArgs e ) {
			panel.Size = this.ClientSize;
		}

		private float GetAspect() {
			return (float) panel.Width / panel.Height;
		}
		private Vector2 GetCenter() {
			return (panel.Width / 2f, panel.Height / 2f);
		}

		private void REEE( object sender, EventArgs e ) {
			panel.Refresh();
		}

		private void UpdateSimplexDisplay() {
			if( simplex is null ) {
				simplexMold.Set( new PointMass<Vector2>[ 0 ] );
				return;
			}
			PointMass<Vector2>[] s = new PointMass<Vector2>[ simplex.Count ];

			for( int i = 0; i < s.Length; i++ ) {
				s[ i ] = new PointMass<Vector2>( simplex[ i ].Sum, 1 );
			}

			simplexMold.Set( s );
		}

		private void CreateMinkowski( Shape2Polygon shapeA, Shape2Polygon shapeB ) {
			if( !displayMinkowski ) {
				mink.Set( new PointMass<Vector2>[ 0 ] );
				return;
			}
			PointMass<Vector2>[] s = new PointMass<Vector2>[ shapeA.Size * shapeB.Size ];

			int ind = 0;
			for( int i = 0; i < shapeA.Size; i++ ) {
				for( int j = 0; j < shapeB.Size; j++ ) {
					s[ ind++ ] = new PointMass<Vector2>( shapeA.GetTransformedPoint( i ) - shapeB.GetTransformedPoint( j ), 1 );
				}
			}

			mink.Set( s );
		}

		Font f = new Font( "arial", 12 );

		private void RefreshPanel( object sender, PaintEventArgs e ) {
			e.Graphics.Clear( Color.Black );

			/*
			camTransform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, 0.03f ) * camTransform.Rotation;
			camTransform.Rotation = camTransform.Rotation.Normalized;
			*/

			for( int i = 0; i < models.Count; i++ ) {
				models[ i ].UpdateShapes();
			}

			CreateMinkowski( shape1, shape2 );
			minkModel.UpdateShapes();

			UpdateSimplexDisplay();
			simplexModel.UpdateShapes();

			for( int i = 0; i < drawnShapes.Count; i++ ) {
				Shape2Polygon shape = drawnShapes[ i ].ValueB;
				Brush b = new SolidBrush( drawnShapes[ i ].ValueA );
				Pen pen = new Pen( drawnShapes[ i ].ValueA, 4 );
				for( int j = 0; j < shape.Size; j++ ) {
					Vector2 p = shape.GetTransformedPoint( j );
					Vector2 p2 = shape.GetTransformedPoint( ( j + 1 ) % shape.Size );
					p *= scale.XY;
					p += GetCenter();
					p2 *= scale.XY;
					p2 += GetCenter();
					float s = 10;
					e.Graphics.FillEllipse( b, p.X - s / 2, p.Y - s / 2, s, s );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( p.X + s / 2 ), (int) ( p.Y + s / 2 ) );
					e.Graphics.DrawLine( pen, (int) p.X, (int) p.Y, (int) p2.X, (int) p2.Y );
				}
			}

			if( !( simplex is null ) ) {
				for( int j = 0; j < simplex.Count; j++ ) {
					int k = j + 1;
					k = k == simplex.Count ? 0 : k;
					Vector2 p = simplex[ j ].Sum;
					Vector2 p2 = simplex[ k ].Sum;
					p *= scale.XY;
					p += GetCenter();
					p2 *= scale.XY;
					p2 += GetCenter();
					e.Graphics.DrawLine( pens[ j % pens.Length ], (int) p.X, (int) p.Y, (int) p2.X, (int) p2.Y );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( ( p.X + p2.X ) / 2 ), (int) ( ( p.Y + p2.Y ) / 2 ) );
				}
				for( int j = 0; j < simplex.Count; j++ ) {
					Vector2 p = simplex[ j ].Sum;
					p *= scale.XY;
					p += GetCenter();
					float s = 10;
					e.Graphics.FillEllipse( brushes[ j % brushes.Length ], p.X - s / 2, p.Y - s / 2, s, s );
					e.Graphics.DrawString( "" + (char) ( 'a' + j ), f, brushes[ j % brushes.Length ], (int) ( p.X + s / 2 ), (int) ( p.Y + s / 2 ) );
				}
			}

			if( updateConstant )
				if( Collision2.CheckCollision( shape1, shape2, out res, out bool collision ) ) {
					if( res is null ) {
						e.Graphics.DrawString( "Collision:" + collision + "\nNO EPA", f, origoBrush, 0, 0 );
					} else {
						{
							Vector2 p = transform2.Translation.XY + res.PenetrationDirection * res.Depth;
							Vector2 p2 = transform2.Translation.XY;
							p *= scale.XY;
							p += GetCenter();
							p2 *= scale.XY;
							p2 += GetCenter();
							e.Graphics.DrawLine( new Pen( Color.White, 5 ), (int) p.X, (int) p.Y, (int) p2.X, (int) p2.Y );
						}
						{
							Vector2 p = res.PointA;
							Vector2 p2 = res.PointB;
							p *= scale.XY;
							p += GetCenter();
							p2 *= scale.XY;
							p2 += GetCenter();
							e.Graphics.FillRectangle( brushes[ 0 % brushes.Length ], (int) p.X - 5, (int) p.Y - 5, 10, 10 );
							e.Graphics.FillRectangle( brushes[ 1 % brushes.Length ], (int) p2.X - 5, (int) p2.Y - 5, 10, 10 );
						}
					}
				} else {
					e.Graphics.DrawString( "Collision:" + collision + "\nNO EPA", f, origoBrush, 0, 0 );
				}

			e.Graphics.DrawString( "Mouse: " + mouse, f, origoBrush, 0, 60 );
			e.Graphics.FillEllipse( origoBrush, GetCenter().X - 1, GetCenter().Y - 1, 2, 2 );

		}

		private SolidBrush origoBrush = new SolidBrush( Color.White );

		private void Keypress( object sender, KeyPressEventArgs e ) {

		}
	}
}
