using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Cameras.Projections {
	public class Orthographic : Projection {
		public Orthographic( string name, float x, float y, float zNear, float zFar ) : base( name, Matrix4Factory.CreateOrthographic( x, y, zNear, zFar ) ) { }

		public class Dynamic : Orthographic, IWindowEventListener {

			private float scale;
			private float width, height, zNear, zFar;
			private Vector2 lastAspectVector;

			public float Scale {
				get => scale; set {
					if( value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: Scale change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					if( value == scale )
						return;
					scale = value;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				}
			}

			public float WidthAspectScale {
				get => width; set {
					if( value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: WidthAspectScale change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					if( value == width )
						return;
					width = value;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				}
			}

			public float HeightAscpectScale {
				get => height; set {
					if( value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: HeightAscpectScale change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					if( value == height )
						return;
					height = value;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				}
			}

			public float ZNear {
				get => zNear; set {
					if( value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: Depth change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					if( value == zNear )
						return;
					zNear = value;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				}
			}

			public float ZFar {
				get => zFar; set {
					if( value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: Depth change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					if( value == zFar )
						return;
					zFar = value;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				}
			}

			public Dynamic( string name, GLWindow window, float widthScale, float heightScale, float zNear, float zFar, float scale ) : base( name, scale * widthScale * window.AspectRatioVector.X, scale * heightScale * window.AspectRatioVector.Y, zNear, zFar ) {
				this.width = widthScale;
				this.height = heightScale;
				this.zNear = zNear;
				this.zFar = zFar;
				this.scale = scale;
				lastAspectVector = window.AspectRatioVector;
				window.EventHandler.Window.Add( this );
			}

			public void WindowResizeHandler( IntPtr window, int width, int height ) {
				if( MemLib.Mem.Windows.Get( window, out GLWindow dw ) ) {
					lastAspectVector = dw.AspectRatioVector;
					SetMatrix( Matrix4Factory.CreateOrthographic( scale * this.width * lastAspectVector.X, scale * this.height * lastAspectVector.Y, zNear, zFar ) );
				} else
					Logging.Error( "Unable to locate window input!" );
			}

			public void WindowFocusHandler( IntPtr window, bool focused ) {
			}
		}

	}
}
