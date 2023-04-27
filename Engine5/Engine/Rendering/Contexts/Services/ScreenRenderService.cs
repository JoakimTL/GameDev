using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Objects.Meshes;
using Engine.Rendering.Contexts.Objects.Meshes.Vertices;
using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using OpenGL;
using System.Numerics;

namespace Engine.Rendering.Contexts.Services;

[ProcessAfter<VertexArrayLayoutService, IInitializable>]
public sealed class ScreenRenderService : IContextService, IInitializable
{

    private readonly VertexMeshService _vertexMeshService;
    private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService;
    private VertexMesh<SimpleVertex2> _wholeScreenRectangle = null!;
    private VertexArrayObjectBase _vao = null!;

    public ScreenRenderService(VertexMeshService vertexMeshService, CompositeVertexArrayObjectService compositeVertexArrayObjectService)
    {
        _vertexMeshService = vertexMeshService;
        _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
    }

    public void RenderToScreen(ShaderPipelineBase shader, DataBlockCollection uniforms)
    {
        shader.Bind();
        _vao.Bind();
        uniforms.BindShader(shader);

        //Make sure the elements are an actual pointer, not some pointer address. OpenGL.Net has an overload allowing for arrays of data to be passed, but uses "object" which might allow code that shouldn't work. The parenthesis around (nint)("_wholeScreenRectangle.ElementOffset * IMesh.ElementSizeBytes") is very important, allowing this to be represented as a pointer.
        Gl.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)_wholeScreenRectangle.ElementCount, DrawElementsType.UnsignedInt, (nint)(_wholeScreenRectangle.ElementOffset * IMesh.ElementSizeBytes), (int)_wholeScreenRectangle.VertexOffset);
    }

    public void Render(ShaderPipelineBase shader, VertexArrayObjectBase vao, IndirectCommand command, DataBlockCollection uniforms)
    {
        if (shader is null)
        {
            Log.Line($"Shader invalid, rendering skipped.", Log.Level.CRITICAL);
            return;
        }
        if (vao is null)
        {
            Log.Line($"VAO invalid, rendering skipped.", Log.Level.CRITICAL);
            return;
        }

        shader.Bind();
        vao.Bind();
        uniforms.BindShader(shader);
        Gl.DrawElementsInstancedBaseVertex(
            PrimitiveType.Triangles,
            (int)command.Count,
            DrawElementsType.UnsignedInt,
            (IntPtr)(command.FirstIndex * sizeof(uint)),
            (int)command.InstanceCount,
            (int)command.BaseVertex
        );
    }

    public void Initialize()
    {
        _wholeScreenRectangle = _vertexMeshService.Create("UnitRectangle", stackalloc SimpleVertex2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) }, stackalloc uint[] { 0, 2, 1, 0, 3, 2 }) ?? throw new NullReferenceException("Unable to create screen render mesh.");
        _vao = _compositeVertexArrayObjectService.Get(new[] { typeof(SimpleVertex2) }) ?? throw new NullReferenceException("Couldn't find the vao for screen render");
    }
}
