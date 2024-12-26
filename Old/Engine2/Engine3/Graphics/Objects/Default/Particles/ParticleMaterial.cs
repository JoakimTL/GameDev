using Engine.LinearAlgebra;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles {
	public class ParticleMaterial : Material {

		public int Rows { get; private set; }

		public ParticleMaterial( string name, Texture diffuse, int rows ) : base( name ) {
			AddTexture( OpenGL.TextureUnit.Texture0, diffuse );
			Rows = rows;
			shaderBind += Bind;
		}

		private void Bind( Shader s ) {
			s.Set( "uRows", 1f / Rows );
		}

		public Vector2 GetPointFromIndex( int index ) {
			float s = 1f / Rows;
			return new Vector2( index % Rows * s, index / Rows * s );
		}
	}
}
