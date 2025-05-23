﻿using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.LinearAlgebra;

namespace Engine.Graphics.Objects.Default.Shaders {
	public class SpotLightShader : Shader {

		public SpotLightShader() : base( "Spotlight", "surface3", "light/spot" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
			Set( "uTexNormal", 1 );
			Set( "uTexDepth", 2 );
			Set( "uTexLightingData", 3 );
			Set( "uTexShadowDepth", 10 );
			Set( "uTexShadowDiffuse", 11 );
		}

		protected override void PreCompile() {
		}

		public void SetLight( LightSpot light ) {
			Set( "lCol", light.Color );
			Set( "lPos", light.Translation );
			Set( "lRange", light.Range );
			Set( "lDir", light.Rotation.Forward );
			Set( "lCut", light.Cutoff );
			//Set( "lSize", new Vector2( lightR.Resolution, lightR.Resolution ) );
		}

		public void SetInverseMatrix( Matrix4 matIVP ) {
			Set( "ipvMat", matIVP );
		}

		public void SetCamera( Vector3 pos ) {
			Set( "eyePos", pos );
		}
	}
}
