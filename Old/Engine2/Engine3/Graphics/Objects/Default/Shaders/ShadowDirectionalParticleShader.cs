namespace Engine.Graphics.Objects.Default.Shaders {
	public class ShadowDirectionalParticleShader : Shader {

		public ShadowDirectionalParticleShader() : base( "ShadowFlatParticle3", "light/shadows/directionalParticle3", "light/shadows/directionalParticle3" ) {		}

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
			Set( "uTexture", 0 );
			Set( "uDepTex", 10 );
		}

		protected override void PreCompile() {
		}
	}
}
