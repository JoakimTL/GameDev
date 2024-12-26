using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.VoxelRendering {
	class VoxelShader : Shader {
		public VoxelShader() : base( "Cube Voxel", "geometry/cubeVoxel", "geometry/cubeVoxel" ) { }

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
			Set( "uTexNormal", 1 );
			Set( "uTexLighting", 2 );
			Set( "uTexGlow", 3 );
		}

		protected override void PreCompile() {

		}
	}
}
