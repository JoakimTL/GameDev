namespace Engine.Rendering.Shaders;

public class Entity3TransparencyShader : ShaderPipeline {
	public Entity3TransparencyShader() : base( typeof( Entity3ProgramVertex ), typeof( Entity3TransparencyProgramFragment ) ) { }
}
public class Entity3DirectionalTransparencyShader : ShaderPipeline {
	public Entity3DirectionalTransparencyShader() : base( typeof( Entity3ProgramVertex ), typeof( Entity3DirectionalTransparencyProgramFragment ) ) { }
}