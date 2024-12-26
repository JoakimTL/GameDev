using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class WaterShader : Shader {
		public WaterShader() : base( "Water", "geometry/water", "geometry/water" ) {}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 3, "vUV" );
		}

		protected override void PostCompile() {
			Set( "uTexDepth", 2 );
			Set( "refractionTex", 8 );
			Set( "reflectionTex", 9 );
			Set( "dudvTex", 10 );
			Set( "foamTex", 11 );
		}

		protected override void PreCompile() {

		}
	}
}
