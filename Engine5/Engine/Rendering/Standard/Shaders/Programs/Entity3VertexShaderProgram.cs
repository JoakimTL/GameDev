﻿using Engine.Rendering.Objects;
using Engine.Rendering.Services;

namespace Engine.Rendering.Standard.Shaders.Programs;
public class Entity3VertexShaderProgram : ShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) {
		ShaderSource? source = shaderSourceService.Get( "assets/shaders/geometry3.vert" );
		if ( source is null )
			return;
		AttachShader( source );
	}
}