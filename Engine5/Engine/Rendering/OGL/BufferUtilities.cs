using Engine.Rendering.Contexts.Objects;
using OpenGL;

namespace Engine.Rendering.OGL;

public static class BufferUtilities
{

    private static VertexBufferObject? _tempBuffer;

    public static void Resize(this VertexBufferObject buffer, uint newSizeBytes)
    {
        _tempBuffer = new VertexBufferObject("temp", buffer.SizeBytes, BufferUsage.DynamicDraw);
        Gl.CopyNamedBufferSubData(buffer.BufferId, _tempBuffer.BufferId, nint.Zero, nint.Zero, _tempBuffer.SizeBytes);
        buffer.SetSize(newSizeBytes);
        Gl.CopyNamedBufferSubData(_tempBuffer.BufferId, buffer.BufferId, nint.Zero, nint.Zero, _tempBuffer.SizeBytes);
        _tempBuffer.Dispose();
        _tempBuffer = null;
    }
}