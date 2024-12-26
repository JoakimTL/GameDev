namespace Engine.Graphics.Objects.Default.Shaders {
	public class AmbientParticleShader : Shader {
		public AmbientParticleShader() : base( "Ambient Particles", "surface2", "light/ambientParticles" ) {
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
