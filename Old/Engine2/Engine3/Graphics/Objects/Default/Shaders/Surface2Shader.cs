namespace Engine.Graphics.Objects.Default.Shaders {
	public class Surface2Shader : Shader {
		public Surface2Shader() : base( "Surface2", "surface2", "surface2" ) {
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