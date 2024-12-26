using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class GaussianBlurParticleShadowShader : Shader {
		public GaussianBlurParticleShadowShader() : base( "Gaussian Shadow", "surface2", "pfx/blurParticleShadow" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
