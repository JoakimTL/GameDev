using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Graphics.Disposables {
	public class OGLFramebufferDisposer : OGLDisposable {

		private uint BufferID;
		private uint depthBuffer;
		private List<uint> textures;

		public OGLFramebufferDisposer( string name, uint BufferID, uint depthBuffer, List<uint> textures ) : base( name ) {
			this.BufferID = BufferID;
			this.depthBuffer = depthBuffer;
			this.textures = textures;
		}

		public override void Dispose() {
			if( BufferID != 0 ) {
				Gl.DeleteFramebuffers( BufferID );
				BufferID = 0;
			}
			if( depthBuffer != 0 ) {
				Gl.DeleteRenderbuffers( depthBuffer );
				depthBuffer = 0;
			}

			foreach( uint t in textures )
				Gl.DeleteTextures( t );
			textures.Clear();

			Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed!" );
		}
	}
}
