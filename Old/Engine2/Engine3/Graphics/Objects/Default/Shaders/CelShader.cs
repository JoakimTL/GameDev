namespace Engine.Graphics.Objects.Default.Shaders {
	public class CelShader : Shader {
		public CelShader() : base( "Cel", "surface2", "pfx/cel" ) {
		}

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexture", 0 );
		}

		protected override void PreCompile() {

		}
	}
}