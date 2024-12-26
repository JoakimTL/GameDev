using Engine.Graphics.Objects;
using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Cameras.Projections {
	public class Perspective : Projection {

		public const float DEFAULT_NEAR = 0.00390625f;  //2^-8
		public const float DEFAULT_FAR = 256;           //2^8

		public Perspective( string name, float fov, float aspectratio, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR ) : base( name, Matrix4Factory.CreatePerspectiveFieldOfView( fov / 180 * (float) Math.PI, aspectratio, zNear, zFar ) ) { }

		public class Dynamic : Perspective, IWindowResizeEventListener {

			private float fov, zNear, zFar, lastAspectRatio;
			public float FOV {
				get => fov; set {
					if( value >= 180 || value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: FOV change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					fov = value;
					SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( FOV / 180 * (float) Math.PI, lastAspectRatio, ZNear, ZFar ) );
				}
			}

			public float ZNear {
				get => zNear; set {
					if( value >= ZFar || value <= 0 ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: Near-Z change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					zNear = value;
					SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( FOV / 180 * (float) Math.PI, lastAspectRatio, ZNear, ZFar ) );
				}
			}

			public float ZFar {
				get => zFar; set {
					if( value <= zFar ) {
						MemLib.Mem.Logs.Error.WriteLine( $"[{Name}]: Far-Z change was attempted, but [{value}] was out of bounds!" );
						return;
					}
					zFar = value;
					SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( FOV / 180 * (float) Math.PI, lastAspectRatio, ZNear, ZFar ) );
				}
			}

			public Dynamic( string name, GLWindow window, float fov, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR ) : base( name, fov, window.AspectRatio, zNear, zFar ) {
				this.fov = fov;
				this.zNear = zNear;
				this.zFar = zFar;
				lastAspectRatio = window.AspectRatio;
				window.EventHandler.Window.Add( this );
			}

			public void WindowResizeHandler( IntPtr window, int width, int height ) {
				lastAspectRatio = MemLib.Mem.Windows[ window ].AspectRatio;
				SetMatrix( Matrix4Factory.CreatePerspectiveFieldOfView( FOV / 180 * (float) Math.PI, lastAspectRatio, ZNear, ZFar ) );
			}
		}
	}
}
