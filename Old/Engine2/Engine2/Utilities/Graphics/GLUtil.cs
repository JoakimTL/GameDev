using Engine.GLFrameWork;
using OpenGL;
using System;

namespace Engine.Utilities.Graphics.Utilities {
	public static class GLUtil {

		public static bool Initialized { get; private set; } = false;

		public static void InitializeGL() {

			if( Initialized )
				return;

			Gl.Initialize();
			GLFW.SetErrorCallback( ErrorCallback );

			if( !GLFW.Init() )
				Environment.Exit( -1 );

			Initialized = true;

		}

		public static void ContextInitialization( Window w, int vsync ) {

			GLFW.WindowHint( Hint.Samples, 4 );
			GLFW.WindowHint( Hint.ContextVersionMajor, 3 );
			GLFW.WindowHint( Hint.ContextVersionMinor, 3 );
			GLFW.WindowHint( Hint.OpenglForwardCompatible, Constants.True );
			GLFW.WindowHint( Hint.OpenglProfile, (int) Profile.Core );

			SetWindowCurrent( w );
			SetVSync( vsync );

			MemLib.Mem.Logs.Routine.WriteLine( "Using OpenGL version: " + Gl.GetString( StringName.Version ), ConsoleColor.Blue );

		}

		public static void SetWindowCurrent( Window w ) {
			GLFW.MakeContextCurrent( w );
		}

		public static void PollEvents() {
			GLFW.PollEvents();
		}

		public static void Terminate() {
			GLFW.Terminate();
		}

		public static void SetVSync( int vsync ) {
			GLFW.SwapInterval( vsync );
		}

		private static void ErrorCallback( GLFrameWork.ErrorCode code, IntPtr desc ) {
			MemLib.Mem.Logs.Error.WriteLine( "GLFW Error: " + code + ": " + desc );
			Console.ReadLine();
		}

		public static bool CheckError( string executor, bool warning = false ) {
			OpenGL.ErrorCode e = Gl.GetError();
			if( e != OpenGL.ErrorCode.NoError ) {
				if( warning ) {
					MemLib.Mem.Logs.Warning.WriteLine( $"GLError[{executor}]: {e}" );
				} else
					MemLib.Mem.Logs.Error.WriteLine( $"GLError[{executor}]: {e}" );
				return true;
			}
			return false;
		}

	}
}
