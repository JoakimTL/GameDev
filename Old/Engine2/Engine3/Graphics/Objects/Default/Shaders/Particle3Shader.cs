namespace Engine.Graphics.Objects.Default.Shaders {
	public class Particle3Shader : Shader {

		public Particle3Shader() : base( "Particles 3D", "geometry/particles3", "geometry/particles3" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "iTransform" );
			SetAttribLocation( 2, "iTextureData" );
			SetAttribLocation( 3, "iDiffuse" );
			SetAttribLocation( 4, "iGlow" );
			SetAttribLocation( 5, "iBlend" );
			SetAttribLocation( 6, "iRotation" );
		}

		protected override void PostCompile() {
			Set( "uDiffTexture", 0 );
			Set( "uDepTex", 2 );
		}

		protected override void PreCompile() {

		}
	}
}
