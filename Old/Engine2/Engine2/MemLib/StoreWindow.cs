using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class StoreWindow {

		private Dictionary<IntPtr, GLWindow> windows;

		public StoreWindow() {
			windows = new Dictionary<IntPtr, GLWindow>();
		}

#if DEBUG
		~StoreWindow() {
			if( windows.Count > 0 )
				System.Diagnostics.Debug.Fail( "Window/eventhandler cache was not disposed!" );
		}
#endif

		public GLWindow this[ IntPtr id ] {
			get {
				if( windows.TryGetValue( id, out GLWindow v ) )
					return v;
				return null;
			}
		}

		public GLWindow CreateWindow( string title, int width, int height ) {
			Window w = GLFW.CreateWindow( width, height, title, Monitor.None, Window.None );
			GLWindow dw = new GLWindow( w );
			lock( windows )
				windows.Add( w, dw );
			return dw;
		}

		public GLWindow CreateFullscreen( string title, Monitor m ) {
			VideoMode vm = GLFW.GetVideoMode( m );
			Window w = GLFW.CreateWindow( vm.Width, vm.Height, title, m, Window.None );
			GLWindow dw = new GLWindow( w );
			lock( windows )
				windows.Add( w, dw );
			return dw;
		}

		public GLWindow CreateWindowedFullscreen( string title, Monitor m ) {
			VideoMode vm = GLFW.GetVideoMode( m );
			Window w = GLFW.CreateWindow( vm.Width, vm.Height, title, Monitor.None, Window.None );
			GLFW.SetWindowAttribute( w, WindowAttribute.Floating, true );
			GLFW.SetWindowAttribute( w, WindowAttribute.Decorated, false );
			GLFW.GetMonitorPosition( m, out int mx, out int my );
			GLFW.SetWindowPosition( w, mx, my );
			GLWindow dw = new GLWindow( w );
			lock( windows )
				windows.Add( w, dw );
			return dw;
		}

		public void Dispose() {
			lock( windows ) {
				foreach( var p in windows.Values )
					GLFW.DestroyWindow( p.Pointer );
				windows.Clear();
			}
		}

	}
}
