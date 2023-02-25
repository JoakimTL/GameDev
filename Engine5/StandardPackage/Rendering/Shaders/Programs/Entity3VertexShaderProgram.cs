using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Services;

namespace StandardPackage.Rendering.Shaders.Programs;
public sealed class Entity3VertexShaderProgram : ShaderProgramBase
{
    protected override void AttachShaders(ShaderSourceService shaderSourceService)
    {
        ShaderSource? source = shaderSourceService.Get("assets/shaders/geometry3.vert");
        if (source is null)
            return;
        AttachShader(source);
    }
}
