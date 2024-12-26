using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class EntityShader : Shader {
		public EntityShader() : base( "Entity", "geometry/geometry3", "geometry/geometry3" ) {

		}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vUV" );
			SetAttribLocation( 2, "vNor" );
			SetAttribLocation( 3, "vCol" );
		}

		protected override void PostCompile() {
			Set( "cTexture", 0 );
			Set( "nTexture", 1 );
			Set( "sTexture", 2 );
			Set( "gTexture", 3 );
		}

		protected override void PreCompile() {

		}
	}
}
