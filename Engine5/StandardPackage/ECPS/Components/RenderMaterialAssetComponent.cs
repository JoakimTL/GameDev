using Engine;
using Engine.GameLogic.ECPS;
using Engine.Structure.Interfaces;

namespace StandardPackage.ECPS.Components;

public sealed class RenderMaterialAssetComponent : ComponentBase, ICustomizedSerializable
{

	public static Guid SerializationIdentity { get; } = new("d72fd30f-701c-4d99-95fd-99310f35f1d8");
	public bool ShouldSerialize => true;

	public string? AssetName { get; private set; }

	public void SetMaterial(string assetName)
	{
		if (assetName == AssetName)
			return;
		AssetName = assetName;
		AlertComponentChanged();
	}

	public bool DeserializeData(byte[] data)
	{
		if (data.Length % sizeof(char) != 0)
			return Log.WarningThenReturn($"Data for name must be a multiple of {sizeof(char)}.", false);
		string? deserialized = data.CreateString();
		if (deserialized is null)
			return false;
		AssetName = deserialized;
		AlertComponentChanged();
		return true;
	}

	public byte[] SerializeData() => AssetName?.ToBytes() ?? Array.Empty<byte>();
}
