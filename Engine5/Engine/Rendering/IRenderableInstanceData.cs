namespace Engine.Rendering;

public interface IRenderableInstanceData
{
    Type? InstanceDataType { get; }
    byte[] GetData();
}
