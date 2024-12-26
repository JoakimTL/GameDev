using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Materials {
	public class SingleTextureMaterial : Material {
		public SingleTextureMaterial( string name, TextureUnit tUnit, Texture t ) : base( name ) {
			AddTextures( new Utilities.Data.Boxing.ImmutableDuo<TextureUnit, Texture>( tUnit, t ) );
		}
	}
}
