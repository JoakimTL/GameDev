using Engine.LMath;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionDev {
	public partial class Form1 : Form {

		private Shape shape1;
		private Shape shape2;
		private Shape minkowski;
		private Simplex simplex;
		private Simplex poly;
		bool show = false;
		bool locked = false;
		Result closest;

		private Vector2 mousePointer;

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load( object sender, EventArgs e ) {
			simplex = new Simplex();
			shape1 = new Shape( Color.Blue );
			shape1.points.Add( new Engine.LMath.Vector2( 1, 1 ) );
			shape1.points.Add( new Engine.LMath.Vector2( -1, 1 ) );
			shape1.points.Add( new Engine.LMath.Vector2( -1, -1 ) );
			shape1.points.Add( new Engine.LMath.Vector2( 0, -1.3f ) );
			shape1.points.Add( new Engine.LMath.Vector2( 1, -1 ) );
			shape2 = new Shape( Color.Red );
			shape2.points.Add( new Engine.LMath.Vector2( 0, 0 ) );
			shape2.points.Add( new Engine.LMath.Vector2( 0.4f, 0 ) );
			minkowski = new Shape( Color.Green );
			KeyDown += KeyPressEvent;
			Resize += OnResizaaaaae;
			dbPanel1.MouseMove += MouseMoveEvent;
			dbPanel1.Location = new Point( 0, 0 );
			dbPanel1.Size = Size;
			dbPanel1.Paint += Repaint;
			timer1.Interval = 25;
			timer1.Tick += TimerElapsed;
			timer1.Enabled = true;
			poly = null;
		}

		private void KeyPressEvent( object sender, KeyEventArgs e ) {
			if( e.KeyCode == Keys.E ) {
				closest = GJK.DoSingleStep( ref poly, simplex, shape1, shape2, true );
			}
			if( e.KeyCode == Keys.R ) {
				simplex.comps.Clear();
				closest = null;
				poly = null;
			}
			if( e.KeyCode == Keys.W ) {
				locked = !locked;
			}
			if( e.KeyCode == Keys.Q ) {
				show = !show;
			}
		}

		private void MouseMoveEvent( object sender, MouseEventArgs e ) {
			if( !locked )
				shape2.transform = Matrix4Factory.CreateTranslation( new Vector3( ( e.X - Size.Width / 2 ) / 100f, ( e.Y - Size.Height / 2 ) / 100f, 0 ) );
			mousePointer = new Vector2( e.X, e.Y );
		}

		private void OnResizaaaaae( object sender, EventArgs e ) {
			dbPanel1.Size = Size;
		}

		private void TimerElapsed( object sender, EventArgs e ) {
			dbPanel1.Refresh();
		}

		private void Repaint( object sender, PaintEventArgs e ) {
			float size = 100;
			float radius = 8;
			e.Graphics.Clear( Color.Black );

			//shape1.transform = Matrix4Factory.CreateFromAxisAngle( new Vector3( 0, 0, 1 ), (float) ( (double) Stopwatch.GetTimestamp() / TimeSpan.TicksPerSecond ) );
			/*
			simplex.comps.Clear();
			simplex.failed = false;
			closest = null;*/
			//while( closest is null ) {
			//closest = GJK.DoSingleStep( simplex, shape1, shape2, false );
			//}

			SolidBrush m1 = new SolidBrush( Color.Orange );

			Vector2 mousePointerReal = new Vector2( ( mousePointer.X - Size.Width / 2f ) / 100f, ( mousePointer.Y - Size.Height / 2f ) / 100f );
			e.Graphics.FillEllipse( m1, mousePointerReal.X * size - 3 + e.ClipRectangle.Width / 2, mousePointerReal.Y * size - 3 + e.ClipRectangle.Height / 2, 3 * 2, 3 * 2 );
			Shape s2 = new Shape( Color.White );
			s2.points.Add( mousePointerReal );
			Simplex s = new Simplex();
			Result c = null;
			Simplex p = null;
			int it = 0;
			while( c is null && it < 50 ) {
				c = GJK.DoSingleStep( ref p, s, shape1, s2, false );
				it++;
			}
			SolidBrush b0;
			if( c is null ) {
				b0 = new SolidBrush( Color.FromArgb( 255, 255, 255, 255 ) );
			} else {
				if( c.inside ) {
					b0 = new SolidBrush( Color.FromArgb( 255, Math.Min( Math.Max( (int) ( c.sum.Length * 100 ), 0 ), 255), 0, 255 ) );
				} else {
					b0 = new SolidBrush( Color.FromArgb( 255, Math.Min( Math.Max( (int) ( c.sum.Length * 100 ), 0 ), 255 ), 255, 255 ) );
				}
			}
			e.Graphics.FillEllipse( b0, -radius + e.ClipRectangle.Width / 2, -radius + e.ClipRectangle.Height / 2, radius * 2, radius * 2 );
			if( !( c is null ) ) {
				e.Graphics.DrawString( c.sum.Length.ToString(), new Font( "Arial", 12 ), new SolidBrush( Color.White ), mousePointerReal.X * size + 3 + e.ClipRectangle.Width / 2, mousePointerReal.Y * size + 3 + e.ClipRectangle.Height / 2 );

				Pen pc = new Pen( Color.DarkGreen );
				e.Graphics.DrawLine( pc,
					c.a.X * size + e.ClipRectangle.Width / 2,
					c.a.Y * size + e.ClipRectangle.Height / 2,
					c.b.X * size + e.ClipRectangle.Width / 2,
					c.b.Y * size + e.ClipRectangle.Height / 2 );
			}
			SolidBrush b1 = new SolidBrush( shape1.color );
			for( int i = 0; i < shape1.points.Count; i++ ) {
				e.Graphics.FillEllipse( b1, shape1.GetTransformed( i ).X * size - radius + e.ClipRectangle.Width / 2, shape1.GetTransformed( i ).Y * size - radius + e.ClipRectangle.Height / 2, radius * 2, radius * 2 );
			}
			Pen p1 = new Pen( shape1.color );
			for( int i = 0; i < shape1.points.Count; i++ ) {
				e.Graphics.DrawLine( p1,
					shape1.GetTransformed( i ).X * size + e.ClipRectangle.Width / 2,
					shape1.GetTransformed( i ).Y * size + e.ClipRectangle.Height / 2,
					shape1.GetTransformed( ( i + 1 ) % shape1.points.Count ).X * size + e.ClipRectangle.Width / 2,
					shape1.GetTransformed( ( i + 1 ) % shape1.points.Count ).Y * size + e.ClipRectangle.Height / 2 );
			}
			SolidBrush b2 = new SolidBrush( shape2.color );
			for( int i = 0; i < shape2.points.Count; i++ ) {
				e.Graphics.FillEllipse( b2, shape2.GetTransformed( i ).X * size - radius + e.ClipRectangle.Width / 2, shape2.GetTransformed( i ).Y * size - radius + e.ClipRectangle.Height / 2, radius * 2, radius * 2 );
			}
			Pen p2 = new Pen( shape2.color );
			for( int i = 0; i < shape2.points.Count; i++ ) {
				e.Graphics.DrawLine( p2,
					shape2.GetTransformed( i ).X * size + e.ClipRectangle.Width / 2,
					shape2.GetTransformed( i ).Y * size + e.ClipRectangle.Height / 2,
					shape2.GetTransformed( ( i + 1 ) % shape2.points.Count ).X * size + e.ClipRectangle.Width / 2,
					shape2.GetTransformed( ( i + 1 ) % shape2.points.Count ).Y * size + e.ClipRectangle.Height / 2 );
			}

			minkowski.points.Clear();
			for( int i = 0; i < shape1.points.Count; i++ ) {
				for( int j = 0; j < shape2.points.Count; j++ ) {
					minkowski.points.Add( new Vector2( shape1.GetTransformed( i ).X - shape2.GetTransformed( j ).X, shape1.GetTransformed( i ).Y - shape2.GetTransformed( j ).Y ) );
				}
			}

			if( show ) {
				SolidBrush b3 = new SolidBrush( minkowski.color );
				for( int i = 0; i < minkowski.points.Count; i++ ) {
					e.Graphics.FillEllipse( b3, minkowski.points[ i ].X * size - radius + e.ClipRectangle.Width / 2, minkowski.points[ i ].Y * size - radius + e.ClipRectangle.Height / 2, radius * 2, radius * 2 );
				}
			}

			SolidBrush b4 = new SolidBrush( Color.White );
			Pen p4 = new Pen( Color.White );
			for( int i = 0; i < simplex.comps.Count; i++ ) {
				e.Graphics.FillEllipse( b4, simplex.comps[ i ].sum.X * size - radius + e.ClipRectangle.Width / 2, simplex.comps[ i ].sum.Y * size - radius + e.ClipRectangle.Height / 2, radius * 2, radius * 2 );
				e.Graphics.DrawString( i.ToString(), new Font( "Arial", 12 ), b4, simplex.comps[ i ].sum.X * size + radius + e.ClipRectangle.Width / 2, simplex.comps[ i ].sum.Y * size + radius + e.ClipRectangle.Height / 2 );
			}

			for( int i = 0; i < simplex.comps.Count; i++ ) {
				p4.Color = Color.FromArgb( 255 << 24 | 255 << ( i * 8 ) );
				e.Graphics.DrawLine( p4,
					simplex.comps[ i ].sum.X * size + e.ClipRectangle.Width / 2,
					simplex.comps[ i ].sum.Y * size + e.ClipRectangle.Height / 2,
					simplex.comps[ ( i + 1 ) % simplex.comps.Count ].sum.X * size + e.ClipRectangle.Width / 2,
					simplex.comps[ ( i + 1 ) % simplex.comps.Count ].sum.Y * size + e.ClipRectangle.Height / 2 );
			}

			for( int i = 0; i < simplex.comps.Count; i++ ) {
				p4.Color = Color.FromArgb( 255 << 24 | 255 << ( i * 8 ) );
				e.Graphics.DrawLine( p4,
					simplex.comps[ i ].a.X * size + e.ClipRectangle.Width / 2,
					simplex.comps[ i ].a.Y * size + e.ClipRectangle.Height / 2,
					simplex.comps[ i ].b.X * size + e.ClipRectangle.Width / 2,
					simplex.comps[ i ].b.Y * size + e.ClipRectangle.Height / 2 );
			}

			if( !( closest is null ) ) {
				p4.Color = Color.DarkRed;
				p4.Width = 10;
				e.Graphics.DrawLine( p4,
					closest.a.X * size + e.ClipRectangle.Width / 2,
					closest.a.Y * size + e.ClipRectangle.Height / 2,
					closest.b.X * size + e.ClipRectangle.Width / 2,
					closest.b.Y * size + e.ClipRectangle.Height / 2 );
			}
		}
	}
}
