namespace Engine.Rendering;

public interface IRenderableInstanceData
{
    ReadOnlySpan<byte> GetInstanceData(float time, out bool extrapolating);
    Type? InstanceDataType { get; }
}