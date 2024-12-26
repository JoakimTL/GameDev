using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Graphics.Disposables {
	public class OGLTextureDisposer : OGLDisposable {

		private uint texId;

		public OGLTextureDisposer( string name, uint texId ) : base( name ) {
			this.texId = texId;
		}

		public override void Dispose() {
			Gl.DeleteTextures( new uint[] { texId } );
			MemLib.Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed [{texId}]!" );
		}
	}
}
