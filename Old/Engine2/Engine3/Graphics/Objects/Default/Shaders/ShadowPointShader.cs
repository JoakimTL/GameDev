namespace Engine.Graphics.Objects.Default.Shaders {
	public class ShadowPointShader : Shader {

		public ShadowPointShader() : base( "ShadowPoint", "light/shadows/point", "light/shadows/point" ) { }

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
