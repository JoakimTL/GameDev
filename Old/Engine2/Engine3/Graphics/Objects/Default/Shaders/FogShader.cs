using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class FogShader : Shader {

		public FogShader() : base( "Fog Shader", "surface2", "pfx/fog" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDepth", 0 );
		}

		protected override void PreCompile() {

		}
}
}
