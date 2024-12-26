using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class OutlineShader : Shader {

		public OutlineShader() : base( "Outline Shader", "surface2", "pfx/outline" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexNormal", 1 );
			Set( "uTexDepth", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
