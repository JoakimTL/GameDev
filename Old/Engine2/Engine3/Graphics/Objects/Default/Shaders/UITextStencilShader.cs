namespace Engine.Graphics.Objects.Default.Shaders {
	public class UITextStencilShader : Shader {

		public UITextStencilShader() : base( "UI Text Stencil", "gui/text", "gui/stencil" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "iTranslation" );
			SetAttribLocation( 2, "iSize" );
			SetAttribLocation( 3, "iUVOffset" );
			SetAttribLocation( 4, "iUVSize" );
			SetAttribLocation( 5, "iCol" );
		}

		protected override void PostCompile() {
			Set( "uDiffuse", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
