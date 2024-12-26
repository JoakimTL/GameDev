namespace Engine.Graphics.Objects.Default.Shaders {
	public class UITextShader : Shader {

		public UITextShader() : base( "UI Text", "gui/text", "gui/text" ) { }

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
			Set( "uStencil", 4 );
		}

		protected override void PreCompile() {

		}
	}
}
