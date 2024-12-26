using System;
using Engine.Utilities.Graphics.Utilities;
using Engine.LinearAlgebra;
using Engine.GLFrameWork;
using OpenGL;
using System.Security.Cryptography.X509Certificates;

namespace Engine.Graphics.Objects {
	public class GLWindow {

		public Window GLFWWindow { get; private set; }

		public InputEventHandler EventHandler { get; private set; }

		private Vector2i size;
		private float aspectRatio;
		private Vector2 aspectRatioVector;
		private Matrix4 orthoAspectMatrix;

		public Vector2i Size => size;
		public float AspectRatio => aspectRatio;
		public Vector2 AspectRatioVector => aspectRatioVector;
		public Matrix4 OrthoAspectMatrix => orthoAspectMatrix;

		public bool ShouldClose => GLFW.WindowShouldClose( GLFWWindow );
		public bool IsFocused { get; private set; }

		public event Action SwapBufferEvent;

		public GLWindow( Window window ) {
			GLFWWindow = window;
			IsFocused = true;

			EventHandler = new InputEventHandler();
			EventHandler.Bind( GLFWWindow );
			EventHandler.Window.Resize += WindowResizeHandler;
			EventHandler.Window.Focused += WindowFocusHandler;

			GLFW.GetWindowSize( GLFWWindow, out int w, out int h );
			SizeChanged( w, h );

			GLUtil.ContextInitialization( window, 0 );
		}

		private void SizeChanged( int width, int height ) {
			this.size = new Vector2i( width, height );
			aspectRatio = (float) width / height;
			float aspectRatioX = size.X > size.Y ? ( (float) size.X / size.Y ) : 1;
			float aspectRatioY = size.Y > size.X ? ( (float) size.Y / size.X ) : 1;
			aspectRatioVector = new Vector2( aspectRatioX, aspectRatioY );
			Matrix4Factory.CreateOrthographic( aspectRatioX * 2, aspectRatioY * 2, -1, 1, out orthoAspectMatrix );
		}

		public void Bind() {
			Gl.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
			Gl.Viewport( 0, 0, Size.X, Size.Y );
		}

		public Vector2i GetPixelCoord( Vector2 ndc ) {
			Vector2 v = new Vector2( ( ndc + 1 ) / 2 );
			v.X *= Size.X;
			v.Y *= Size.Y;
			return v.IntFloored;
		}

		public void SwapBuffers() {
			SwapBufferEvent?.Invoke();
			GLFW.SwapBuffers( GLFWWindow );
		}

		public void Dispose() {
			GLFW.DestroyWindow( GLFWWindow );
		}

		public void SetIcons() {
		}

		private void WindowResizeHandler( IntPtr window, int width, int height ) {
			SizeChanged( width, height );
		}

		private void WindowFocusHandler( IntPtr window, bool focused ) {
			IsFocused = focused;
		}
	}
}
