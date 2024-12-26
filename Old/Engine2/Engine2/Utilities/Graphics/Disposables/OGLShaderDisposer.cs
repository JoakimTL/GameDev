using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Graphics.Disposables {
	public class OGLShaderDisposer : OGLDisposable {

		private uint ID;
		private uint vShader, gShader, fShader;

		public OGLShaderDisposer( string name, uint id, uint vs, uint gs, uint fs ) : base( name ) {
			this.ID = id;
			this.vShader = vs;
			this.gShader = gs;
			this.fShader = fs;
		}

		private void DisposeFragmentShader() {
			Gl.DetachShader( ID, fShader );
			Gl.DeleteShader( fShader );
			fShader = 0;
		}

		private void DisposeGeometryShader() {
			Gl.DetachShader( ID, gShader );
			Gl.DeleteShader( gShader );
			gShader = 0;
		}

		private void DisposeVertexShader() {
			Gl.DetachShader( ID, vShader );
			Gl.DeleteShader( vShader );
			vShader = 0;
		}

		public override void Dispose() {
			DisposeVertexShader();
			DisposeGeometryShader();
			DisposeFragmentShader();
			Gl.DeleteProgram( ID );
			Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed!" );
		}
	}
}
