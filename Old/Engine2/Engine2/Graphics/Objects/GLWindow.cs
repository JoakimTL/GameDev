using System;
using Engine.Utilities.Graphics.Utilities;
using Engine.LMath;
using Engine.GLFrameWork;

namespace Engine.Graphics.Objects {
	public class GLWindow {

		public Window Pointer {
			get; private set;
		}

		public InputEventHandler EventHandler {
			get; private set;
		}


		private Vector2i size;
		private float aspectRatio;
		private Vector2 aspectRatioVector;
		private Matrix4 orthoAspectMatrix;

		public Vector2i Size => size;
		public float AspectRatio => aspectRatio;
		public Vector2 AspectRatioVector => aspectRatioVector;
		public Matrix4 OrthoAspectMatrix => orthoAspectMatrix;

		public bool ShouldClose => GLFW.WindowShouldClose( Pointer );

		public event Action SwapBufferEvent;

		public GLWindow( Window window ) {
			Pointer = window;

			EventHandler = new InputEventHandler();
			EventHandler.Bind( Pointer );

			EventHandler.Window.Resize += WindowResizeHandler;
			GLFW.GetWindowSize( Pointer, out int w, out int h );
			SizeChanged( w, h );

			GLUtil.ContextInitialization( window, 0 );
		}

		private void WindowResizeHandler( IntPtr window, int width, int height ) {
			SizeChanged( width, height );
		}

		private void SizeChanged( int width, int height ) {
			this.size = new Vector2i( width, height );
			aspectRatio = (float) width / height;
			float aspectRatioX = size.X > size.Y ? ( (float) size.X / size.Y ) : 1;
			float aspectRatioY = size.Y > size.X ? ( (float) size.Y / size.X ) : 1;
			aspectRatioVector = new Vector2( aspectRatioX, aspectRatioY );
			Matrix4Factory.CreateOrthographic( aspectRatioX * 2, aspectRatioY * 2, -1, 1, out orthoAspectMatrix );
		}

		public Vector2i GetPixelCoord( Vector2 ndc ) {
			Vector2 v = new Vector2( ( ndc + 1 ) / 2 );
			v.X *= Size.X;
			v.Y *= Size.Y;
			return v.IntFloored;
		}

		public void SwapBuffers() {
			SwapBufferEvent?.Invoke();
			GLFW.SwapBuffers( Pointer );
		}

		public void Dispose() {
			GLFW.DestroyWindow( Pointer );
		}

	}
}
