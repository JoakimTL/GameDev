namespace Engine.Graphics.Objects.Default.Shaders {
	public class ShadowDirectionalShader : Shader {

		public ShadowDirectionalShader() : base( "ShadowFlat", "light/shadows/directional", "light/shadows/directional" ) {		}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vUV" );
		}
		
		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
		}

		protected override void PreCompile() {
		}
	}
}
