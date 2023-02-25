using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public class VertexBufferObjectService : Identifiable, IContextService, IInitializable, IDisposable
{
    private readonly Dictionary<Type, VertexBufferObject> _vbos;

    public VertexBufferObject ElementBuffer { get; private set; } = null!;
    public SegmentedVertexBufferObject UniformBuffer { get; private set; } = null!;
    public SegmentedVertexBufferObject ShaderStorage { get; private set; } = null!;

    public VertexBufferObjectService()
    {
        _vbos = new();
    }

    public void Initialize()
    {
        Gl.GetInteger(GetPName.UniformBufferOffsetAlignment, out uint alignment);
        ElementBuffer = new VertexBufferObject("Elements", 65_536u, BufferUsage.DynamicDraw);
        UniformBuffer = new SegmentedVertexBufferObject("Uniforms", 65_536u, BufferUsage.DynamicDraw, alignment);
        ShaderStorage = new SegmentedVertexBufferObject("ShaderStorage", 65_536u, BufferUsage.DynamicDraw, 1);
        this.LogLine($"Uniform offset alignment: {alignment}", Log.Level.NORMAL);
    }

    public VertexBufferObject Get(Type t)
    {
        if (_vbos.TryGetValue(t, out VertexBufferObject? vbo))
            return vbo;
        vbo = new VertexBufferObject(t.Name, 65536, BufferUsage.DynamicDraw);
        _vbos[t] = vbo;
        return vbo;
    }

    public void Dispose()
    {
        ElementBuffer.Dispose();
        UniformBuffer.Dispose();
        ShaderStorage.Dispose();
        foreach (var vbo in _vbos.Values)
            vbo.Dispose();
    }
}

public class DataBlockService : Identifiable, IContextService, IDisposable
{

}
