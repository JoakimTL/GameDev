using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class Particle2Shader : Shader {

		public Particle2Shader() : base( "Particles 2D", "geometry/particles", "geometry/particles2" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "iM_Mat" );
			SetAttribLocation( 5, "iTex" );
			SetAttribLocation( 6, "iColor" );
			SetAttribLocation( 7, "iBlend" );
		}

		protected override void PostCompile() {
			Set( "uTexture", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
