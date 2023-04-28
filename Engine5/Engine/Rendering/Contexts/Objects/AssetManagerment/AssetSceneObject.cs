using Engine.Datatypes.Buffers;
using Engine.Rendering.Contexts.Objects.Meshes;
using Engine.Rendering.Contexts.Objects.Scenes;

namespace Engine.Rendering.Contexts.Objects.AssetManagerment;
internal class AssetSceneObject : SceneObjectBase
{
    public IRenderable Renderable { get; }
    public SceneInstanceData? InstanceData { get; private set; }
    public Type? InstanceDataType { get; private set; }
    public MaterialAsset? CurrentMaterial { get; private set; }
    public LoadedAssetMesh? CurrentMesh { get; private set; }

    public AssetSceneObject(IRenderable renderable)
    {
        Renderable = renderable;
    }

    internal void Set(MaterialAsset? material, LoadedAssetMesh? mesh, VertexArrayObjectBase? vao)
    {
        SetMaterial(material);
        SetMesh(mesh);
        SetVertexArrayObject(vao);
    }

    internal void SetMaterial(MaterialAsset? material)
    {
        CurrentMaterial = material;
        SetShaders(CurrentMaterial?.ShaderBundle);
    }

    internal void SetMesh(LoadedAssetMesh? mesh)
    {
        CurrentMesh = mesh;
        base.SetMesh(CurrentMesh);
    }

    internal void SetSceneInstanceData(SceneInstanceData instanceData, Type type)
    {
        InstanceDataType = type;
        InstanceData = instanceData;
        SetSceneData(instanceData);
    }

    internal void SetSceneInstanceDataSegment(ISegmentedBufferSegment segment, uint instanceSize, bool ownsData, Type type)
    {
        InstanceData?.SetSegment(segment, instanceSize, ownsData);
        InstanceDataType = type;
    }

    internal void UpdateInstanceData(ReadOnlySpan<byte> data, uint instanceSizeWithTexturesBytes, uint incompleteInstanceSizeBytes, uint instanceCount)
    {
        if (InstanceData is null)
            return;
        unsafe
        {
            byte* currentInstanceData = stackalloc byte[(int)instanceSizeWithTexturesBytes];
            Span<byte> span = new(currentInstanceData, (int)instanceSizeWithTexturesBytes);

            int numTextures = CurrentMaterial?.Textures.Count ?? 0;
            ushort* texIndicesSrc = stackalloc ushort[numTextures];
            for (int i = 0; i < numTextures; i++)
                texIndicesSrc[i] = CurrentMaterial!.Textures[i].Index;

            fixed (byte* srcPtr = data)
            {
                for (uint i = 0; i < instanceCount; i++)
                {
                    Buffer.MemoryCopy(srcPtr + i * incompleteInstanceSizeBytes, currentInstanceData + i * instanceSizeWithTexturesBytes, instanceSizeWithTexturesBytes, incompleteInstanceSizeBytes);
                    Buffer.MemoryCopy(texIndicesSrc, currentInstanceData + i * instanceSizeWithTexturesBytes + incompleteInstanceSizeBytes, instanceSizeWithTexturesBytes, numTextures * sizeof(ushort));
                }
            }
            InstanceData.SetInstances(0, span);
            InstanceData.SetActiveInstances(instanceCount);
        }
    }

    public override void Bind()
    {

    }

}
