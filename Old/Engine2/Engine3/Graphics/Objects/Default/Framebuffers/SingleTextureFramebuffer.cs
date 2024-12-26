using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Framebuffers {
	public abstract class SingleTextureFramebuffer : Framebuffer {
		private float scale;

		public uint Texture {
			get; protected set;
		}

		public SingleTextureFramebuffer( string name, Vector2i size ) : base( name, size ) { }

	}
}
