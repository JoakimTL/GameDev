namespace Engine.Graphics.Objects.Default.Shaders {
	public class DepthCopyShader : Shader {
		public DepthCopyShader() : base( "Surface2", "surface2", "pfx/depthCopy" ) {
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