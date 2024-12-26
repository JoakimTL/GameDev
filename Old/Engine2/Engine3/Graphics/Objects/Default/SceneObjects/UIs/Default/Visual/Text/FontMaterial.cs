using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class FontMaterial : Material {
		public FontMaterial( string name, TextureUnit tUnit, Texture tex ) : base( name ) {
			AddTextures( new ImmutableDuo<TextureUnit, Texture>( tUnit, tex ) );
		}
	}
}
