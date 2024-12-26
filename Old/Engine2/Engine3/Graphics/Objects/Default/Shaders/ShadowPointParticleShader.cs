namespace Engine.Graphics.Objects.Default.Shaders {
	public class ShadowPointParticleShader : Shader {

		public ShadowPointParticleShader() : base( "ShadowCubeParticle3", "light/shadows/pointParticle3", "light/shadows/pointParticle3" ) { }

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
		}

		protected override void PreCompile() {
		}
	}
}
