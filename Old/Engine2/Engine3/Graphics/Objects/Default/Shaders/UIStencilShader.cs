namespace Engine.Graphics.Objects.Default.Shaders {
	public class UIStencilShader : Shader {

		public UIStencilShader() : base( "UI Stencil", "gui/ui", "gui/stencil" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vCol" );
			SetAttribLocation( 3, "vUV" );
		}

		protected override void PostCompile() {
		}

		protected override void PreCompile() {
		}
	}
}
