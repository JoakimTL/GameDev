using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class CubeFlatteningShader : Shader {

		public CubeFlatteningShader() : base( "Shadow Cube Flattening Shader", "surface2", "pfx/cubeflat" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexShadowDepth", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
