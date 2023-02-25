using Engine.Rendering.Contexts.Services;
using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public abstract class ShaderPipelineBase : Identifiable, IDisposable
{
    private readonly Dictionary<ShaderType, ShaderProgramBase> _programs;
    public uint PipelineId { get; private set; }
    public IReadOnlyDictionary<ShaderType, ShaderProgramBase> Programs => _programs;
    public abstract bool UsesTransparency { get; }

    protected ShaderPipelineBase()
    {
        _programs = new Dictionary<ShaderType, ShaderProgramBase>();
        PipelineId = Gl.GenProgramPipeline();
    }

#if DEBUG
    ~ShaderPipelineBase()
    {
        System.Diagnostics.Debug.Fail("Shader pipeline was not disposed!");
    }
#endif

    protected abstract IEnumerable<ShaderProgramBase> GetShaderPrograms(ShaderProgramService shaderProgramService);

    internal void CreatePipeline(ShaderProgramService shaderProgramService)
    {
        var shaderPrograms = GetShaderPrograms(shaderProgramService);
        List<ShaderProgramBase> validPrograms = new(shaderPrograms);
        if (validPrograms.Count == 0)
            return;
        for (int i = 0; i < validPrograms.Count; i++)
        {
            ShaderProgramBase prg = validPrograms[i];
            bool valid = true;
            foreach (ShaderType type in prg.Sources.Keys)
            {
                if (_programs.ContainsKey(type))
                {
                    this.LogWarning("Cannot have multiple programs with the same mask bits active.");
                    valid = false;
                    break;
                }
            }
            if (!valid)
                continue;

            foreach (ShaderType type in prg.Sources.Keys)
                _programs.Add(type, prg);

            Gl.UseProgramStage(PipelineId, prg.Mask, prg.ProgramID);
        }
    }

    public void DirectBind() => Gl.BindProgramPipeline(PipelineId);
    public static void DirectUnbind() => Gl.BindProgramPipeline(0);

    public void Dispose()
    {
        Gl.DeleteProgramPipelines(PipelineId);
        GC.SuppressFinalize(this);
    }
}

