namespace Engine.Graphics.Objects.Default.Shaders {
	public class AmbientLightShader : Shader {
		public AmbientLightShader() : base( "Ambient Light", "surface2", "light/ambient" ) {
		}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
			Set( "uTexLightingData", 3 );
			Set( "uTexGlow", 4 );
			Set( "uParticleDiffuse", 5 );
			Set( "uParticleGlow", 6 );
		}

		protected override void PreCompile() {

		}
	}
}
