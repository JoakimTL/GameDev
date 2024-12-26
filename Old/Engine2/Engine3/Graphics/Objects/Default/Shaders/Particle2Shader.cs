namespace Engine.Graphics.Objects.Default.Shaders {
	public class Particle2Shader : Shader {

		public Particle2Shader() : base( "Particles 2D", "geometry/particles2", "geometry/particles2" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "iTransform" );
			SetAttribLocation( 2, "iTextureData" );
			SetAttribLocation( 3, "iColor" );
			SetAttribLocation( 4, "iBlend" );
			SetAttribLocation( 5, "iRotation" );
		}

		protected override void PostCompile() {
			Set( "uTexture", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
