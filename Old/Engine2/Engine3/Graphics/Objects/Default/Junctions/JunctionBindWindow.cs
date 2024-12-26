using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	class JunctionBindWindow : Engine.Pipelines.Junction {

		private readonly GLWindow window;

		public JunctionBindWindow( string name, GLWindow window ) : base( name, null ) {
			this.window = window;
			Effect = Execute;
		}

		protected void Execute() {
			window.Bind();
		}
	}
}
