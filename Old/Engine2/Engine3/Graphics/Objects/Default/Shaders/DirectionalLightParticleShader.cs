using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class DirectionalLightParticleShader : Shader {

		public DirectionalLightParticleShader() : base( $"DirectionalLight", "surface2", "light/directionalParticle" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
			Set( "uTexNormal", 1 );
			Set( "uTexDepth", 2 );
			Set( "uTexLightingData", 3 );
			Set( "uParticleDiffuse", 5 );
			Set( "uParticleDepth", 7 );
			for( int i = 0; i < GLLightDirectional.MAX_CASCADES; i++ ) {
				Set( $"uTexShadowDepth[{i}]", 10 + ( i * 2 ) );
				Set( $"uTexShadowDiffuse[{i}]", 10 + ( i * 2 + 1 ) );
			}
		}

		protected override void PreCompile() {
		}

		public void SetLight( GLLightDirectional lightR ) {
			Set( "lCol", lightR.Light.Color );
			Set( "lDir", lightR.Light.Rotation.Forward );
			lightR.BindShader( this );
		}

		public void SetInverseMatrix( Matrix4 matIVP ) {
			Set( "ipvMat", matIVP );
		}

		public void SetCamera( Vector3 pos, float zNear, float zFar ) {
			Set( "eyePos", pos );
			Set( "zNear", zNear );
			Set( "zFar", zFar );
		}
	}
}
