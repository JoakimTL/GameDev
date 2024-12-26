using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Junctions {
	class JunctionClearSceneBuffer : Engine.Pipelines.Junction {

		private ClearBufferMask mask;
		public Vector4 Color;

		public JunctionClearSceneBuffer( string name, ClearBufferMask mask, Vector4 color ) : base( name, null ) {
			this.mask = mask;
			Color = color;
			Effect = Execute;
		}

		public JunctionClearSceneBuffer( string name, ClearBufferMask mask ) : this( name, mask, Vector4.Zero ) { }

		private void Execute() {
			Gl.ClearColor( Color.X, Color.Y, Color.Z, Color.W );
			Gl.Clear( mask );
		}
	}
}
