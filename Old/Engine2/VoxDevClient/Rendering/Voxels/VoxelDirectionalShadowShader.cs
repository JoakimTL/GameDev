using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevClient.Rendering.Voxels {
	class VoxelDirectionalShadowShader : Shader {
		public VoxelDirectionalShadowShader() : base( "Cube Voxel Direcitonal Shadow", "light/shadows/cubeVoxelShadowDirectional", "light/shadows/cubeVoxelShadowDirectional" ) { }

		protected override void InitAttribs() {
			SetAttribLocation( 0, "vPos" );
			SetAttribLocation( 1, "vNormal" );
			SetAttribLocation( 2, "vUV" );
			SetAttribLocation( 3, "iTranslation" );
			SetAttribLocation( 4, "iScale" );
			SetAttribLocation( 5, "iUV" );
		}

		protected override void PostCompile() {
			Set( "uTexDiffuse", 0 );
		}

		protected override void PreCompile() {

		}
	}
}
