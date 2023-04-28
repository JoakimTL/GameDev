using Engine;
using Engine.Datatypes.Colors;
using Engine.Datatypes.Transforms;
using Engine.Structure.Interfaces;
using StandardPackage.Rendering.VertexArrayLayouts;
using System.Numerics;

namespace StandardPackage.ECPS.Components;

public sealed class RenderInstance3DataComponent : RenderInstanceDataComponentBase<Entity3SceneData>, ICustomizedSerializable
{
    public static Guid SerializationIdentity { get; } = new("fc52578f-949c-464b-86ec-80bbb2a14805");
    public bool ShouldSerialize => true;

    private float _previousTime;
    private TransformData<Vector3, Quaternion, Vector3> _previousTransform;
    private float _currentTime;
    private TransformData<Vector3, Quaternion, Vector3> _currentTransform;

    private Color16x4 _color;
    private bool _normalMapped;

    public override ReadOnlySpan<byte> GetInstanceData(float time, out bool extrapolating)
    {
        extrapolating = time - _currentTime < _currentTime - _previousTime;
        float extrapotaionFactor = (time - _currentTime) / (_currentTime - _previousTime);
        Console.WriteLine(time + ", " + _currentTime + ", " + _previousTime);
        Console.WriteLine(extrapotaionFactor);
        if (extrapolating)
            new Entity3SceneData() { 
                ModelMatrix = _currentTransform.GetExtrapolation(_previousTransform, extrapotaionFactor).GetMatrix(), 
                Color = _color, 
                NormalMapped = _normalMapped ? byte.MaxValue : byte.MinValue }.CopyInto(InstanceData);
        return InstanceData;
    }

    public void Set(float time, TransformData<Vector3, Quaternion, Vector3> transform, Color16x4 color, bool normalMapped = true)
    {
        _previousTransform = _currentTransform;
        _previousTime = _currentTime;
        _currentTransform = transform;
        _currentTime = time;
        _color = color;
        _normalMapped = normalMapped;
        SetData(new Entity3SceneData() { ModelMatrix = _currentTransform.GetMatrix(), Color = _color, NormalMapped = _normalMapped ? byte.MaxValue : byte.MinValue });
    }

    public bool DeserializeData(byte[] data)
    {
        if (data.Length != InstanceData.Length)
            return false;
        InstanceData = data;
        AlertComponentChanged();
        return true;
    }

    public byte[] SerializeData() => InstanceData;
}