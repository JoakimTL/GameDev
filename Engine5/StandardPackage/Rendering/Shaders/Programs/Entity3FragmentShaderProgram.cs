using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Services;

namespace StandardPackage.Rendering.Shaders.Programs;

public sealed class Entity3FragmentShaderProgram : ShaderProgramBase
{
    protected override void AttachShaders(ShaderSourceService shaderSourceService)
    {
        ShaderSource? source = shaderSourceService.Get("assets/shaders/geometry3.frag");
        if (source is null)
            return;
        AttachShader(source);
    }
}
