using Engine;
using Engine.GameLogic.ECPS;
using Engine.Structure.Interfaces;

namespace StandardPackage.ECPS.Components;

public sealed class RenderMeshAssetComponent : ComponentBase, ICustomizedSerializable
{
	public static Guid SerializationIdentity { get; } = new("69711b6a-5570-4378-9a09-d15da724f4ce");
	public bool ShouldSerialize => true;

	public string? AssetName { get; private set; }

	public void SetMesh(string assetName)
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

	//Remember: no OGL stuff here, that will be handled on the OGL thread
}
