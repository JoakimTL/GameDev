using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class Line3Shader : Shader {
		public Line3Shader() : base( "Entity", "geometry/line3", "geometry/line3" ) {

		}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 3, "vCol" );
		}

		protected override void PostCompile() {
		}

		protected override void PreCompile() {

		}
	}
}
