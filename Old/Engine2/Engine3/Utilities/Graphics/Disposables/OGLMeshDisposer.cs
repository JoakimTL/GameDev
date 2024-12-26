using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Graphics.Disposables {
	public class OGLMeshDisposer : OGLDisposable {

		private uint vao;
		private List<uint> VBOs;

		public OGLMeshDisposer( string name, uint vao, List<uint> VBOs ) : base( name ) {
			this.vao = vao;
			this.VBOs = VBOs;
		}

		public override void Dispose() {
			Gl.DeleteVertexArrays( vao );
			string disp = "";
			for( int i = 0; i < VBOs.Count; i++ ) {
				Gl.DeleteBuffers( VBOs[ i ] );
				disp += $"({VBOs[ i ]})";
			}
			VBOs.Clear();
			MemLib.Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed [{disp}]!" );
		}
	}
}
