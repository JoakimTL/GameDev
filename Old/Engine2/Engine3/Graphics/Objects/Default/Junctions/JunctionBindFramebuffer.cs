using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	class JunctionBindFramebuffer : Engine.Pipelines.Junction {

		private Framebuffer buffer;
		private FramebufferTarget target;

		public JunctionBindFramebuffer( string name, Framebuffer buffer, FramebufferTarget target ) : base( name, null ) {
			this.buffer = buffer;
			this.target = target;
			Effect = Execute;
		}


		private void Execute() {
			buffer.Bind( target );
		}
	}
}
