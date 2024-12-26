using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class GaussianBlurBloomShader : Shader {
		public GaussianBlurBloomShader() : base( "Gaussian Bloom", "surface2", "pfx/blurBloom" ) { }

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
