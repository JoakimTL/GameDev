using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class BloomShader : Shader {

		public BloomShader() : base( "Bloom Shader", "surface2", "pfx/bloom" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
		}

		protected override void PreCompile() {

		}

		public void Set( float threshold, float intensity, Vector3 colorWeights ) {
			Set( "threshold", threshold );
			Set( "intensity", intensity );
			Set( "colorWeight", colorWeights );
		}
	}
}
