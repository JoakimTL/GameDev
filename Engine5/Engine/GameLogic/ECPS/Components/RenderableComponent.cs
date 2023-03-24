using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Reflection;

namespace Engine.GameLogic.ECPS.Components;

public sealed class RenderableComponent : ComponentBase {
    public RenderShaderAssetComponent ShaderComponent { get; }
    public RenderMeshAssetComponent MeshComponent { get; }

    public event Action<RenderableComponent>? Changed;

    public RenderableComponent(RenderShaderAssetComponent shaderComponent, RenderMeshAssetComponent meshComponent)
    {
        ShaderComponent = shaderComponent;
        MeshComponent = meshComponent;
        //TODO        //ShaderComponent.ComponentChanged += OnChanged();
    }

}

public sealed class RenderShaderAssetComponent : ComponentBase, ICustomizedSerializable {

	private Type? _shaderType;
	private string? _shaderIdentity;

    public static Guid SerializationIdentity { get; } = new("d72fd30f-701c-4d99-95fd-99310f35f1d8");
    public bool ShouldSerialize => true;

    public void SetShader<T>() where T : ShaderBundleBase
	{
		Type type = typeof(T);
		if (type == _shaderType)
			return;
		_shaderType = typeof(T);
		_shaderIdentity = _shaderType.GetCustomAttribute<IdentityAttribute>()?.Identity;
		AlertComponentChanged();
	}

    public bool DeserializeData(byte[] data)
    {
        if (data.Length % sizeof(char) != 0)
            return Log.WarningThenReturn($"Data for name must be a multiple of {sizeof(char)}.", false);
        string? deserialized = data.CreateString();
        if (deserialized is null)
            return false;
        _shaderIdentity = deserialized;
		_shaderType = Global.Get<IdentityTypeService>().GetFromIdentity(_shaderIdentity) ?? null;
        return true;
    }

    public byte[] SerializeData()
    {
		return _shaderIdentity?.ToBytes() ?? Array.Empty<byte>();
    }
}

public sealed class RenderMeshAssetComponent : ComponentBase, ICustomizedSerializable {

	private string? _assetName;

    public static Guid SerializationIdentity { get; } = new("69711b6a-5570-4378-9a09-d15da724f4ce");
    public bool ShouldSerialize => true;

    public void SetMesh(string assetName)
    {
        if (assetName == _assetName)
            return;
        _assetName = assetName;
        AlertComponentChanged();
    }

    public bool DeserializeData(byte[] data)
    {
        if (data.Length % sizeof(char) != 0)
            return Log.WarningThenReturn($"Data for name must be a multiple of {sizeof(char)}.", false);
        string? deserialized = data.CreateString();
        if (deserialized is null)
            return false;
        _assetName = deserialized;
        return true;
    }

    public byte[] SerializeData()
    {
        return _assetName?.ToBytes() ?? Array.Empty<byte>();
    }

    //Remember: no OGL stuff here, that will be handled on the OGL thread
}