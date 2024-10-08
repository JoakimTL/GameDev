﻿namespace Engine.Rendering.Shaders;

public class Entity3Shader : ShaderPipeline {
	public Entity3Shader() : base( typeof( Entity3ProgramVertex ), typeof( Entity3ProgramFragment ) ) { }
}
public class Entity3DirectionalShader : ShaderPipeline {
	public Entity3DirectionalShader() : base( typeof( Entity3ProgramVertex ), typeof( Entity3DirectionalProgramFragment ) ) { }
}
public class Entity2Shader : ShaderPipeline {
	public Entity2Shader() : base( typeof( Entity2ProgramVertex ), typeof( Entity2ProgramFragment ) ) { }
}