using Engine.Entities.D3;
using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {
	public static class EntityFactory {

		public static Entity CreateBlank3Rendered( string name, Mesh mesh, Material material, ShaderBundle shaders ) {
			Entity nE = new Entity( name );
			nE.Add( new Transform3Module() );
			nE.Add( new Render3Module( mesh, material, shaders ) );
			return nE;
		}

	}
}
