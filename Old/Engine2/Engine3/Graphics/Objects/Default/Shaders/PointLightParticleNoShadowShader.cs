﻿using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class PointLightParticleNoShadowShader : Shader {

		public PointLightParticleNoShadowShader() : base( $"Pointlight Particle No Shadow", "surface3", "light/pointnParticle" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vUV" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
			Set( "uTexNormal", 1 );
			Set( "uTexDepth", 2 );
			Set( "uTexLightingData", 3 );
			Set( "uParticleDiffuse", 5 );
			Set( "uParticleDepth", 7 );
		}

		protected override void PreCompile() {
		}

		public void SetLight( LightPoint light ) {
			Set( "lCol", light.Color );
			Set( "lPos", light.Translation );
			Set( "lRange", light.Range );
		}

		public void SetInverseMatrix( Matrix4 matIVP ) {
			Set( "ipvMat", matIVP );
		}

		public void SetCamera( Vector3 pos ) {
			Set( "eyePos", pos );
		}
	}
}