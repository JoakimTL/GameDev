namespace Engine.Graphics.Objects.Default.Shaders {
	public class UIShader : Shader {

		public UIShader() : base( "GUI", "gui/ui", "gui/ui" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vUV" );
			SetAttribLocation( 3, "vCol" );
		}

		protected override void PostCompile() {
			Set( "uTexture", 0 );
			Set( "uStencil", 4 );
		}

		protected override void PreCompile() {

		}
	}
}
