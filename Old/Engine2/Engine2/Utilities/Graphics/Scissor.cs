using Engine.LMath;
using OpenGL;

namespace Engine.Utilities.Graphics.Utilities {
	public static class Scissor {

		private static bool active = false;
		private static Vector2i botlef, size;

		public static void Set( Vector4i dims ) {
			if( botlef != dims.XY || size != dims.ZW ) {
				botlef = dims.XY;
				size = dims.ZW;
				if( size.X <= 0 || size.Y <= 0 ) {
					MemLib.Mem.Logs.Error.WriteLine( $"Tried to enable scissoring with bad dimensions [{dims}]!" );
				} else {
					Gl.Scissor( botlef.X, botlef.Y, size.X, size.Y );
				}
			}
			if( !active ) {
				active = true;
				Gl.Enable( EnableCap.ScissorTest );
			}
		}

		public static void Disable() {
			if( active ) {
				active = false;
				Gl.Disable( EnableCap.ScissorTest );
			}
		}

	}
}
