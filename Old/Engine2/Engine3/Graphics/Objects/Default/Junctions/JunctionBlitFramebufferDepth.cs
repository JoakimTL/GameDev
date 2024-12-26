using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	public class JunctionBlitFramebufferDepth : Engine.Pipelines.Junction {

		private Framebuffer dst, src;
		private bool unbind;

		public JunctionBlitFramebufferDepth( string name, Framebuffer dst, Framebuffer src, bool unbind ) : base( name, null ) {
			this.dst = dst;
			this.src = src;
			this.unbind = unbind;
			Effect = Execute;
		}


		private void Execute() {
			Framebuffer.BlitDepth( src, dst, unbind );
		}
	}
}
