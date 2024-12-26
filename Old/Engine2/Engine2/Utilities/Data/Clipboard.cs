using Engine.GLFrameWork;

namespace Engine.Utilities.Data {
	public static class Clipboard {
		public static string GetClipboardText( Window window ) {
			return GLFW.GetClipboardString( window );
		}

		public static void SetClipboardText( Window window, string s ) {
			GLFW.SetClipboardString( window, s );
		}
	}
}
