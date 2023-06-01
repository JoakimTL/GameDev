using Engine;
using Engine.GameLogic.ECPS;
using Engine.Structure.Interfaces;

namespace StandardPackage.ECPS.Components;

public sealed class NameComponent : ComponentBase, ICustomizedSerializable
{
	public string EntityName { get; private set; }

	protected override string UniqueNameTag => $"{this.EntityName}";

	public static Guid SerializationIdentity { get; } = new("21d318d8-f61c-4dee-99aa-9513628b8976");
	public bool ShouldSerialize => true;

	public NameComponent()
	{
		this.EntityName = "Unnamed";
	}

	public void SetName(string newName)
	{
		this.EntityName = newName;
		AlertComponentChanged();
	}

	public bool DeserializeData(byte[] data)
	{
		if (data.Length % sizeof(char) != 0)
			return Log.WarningThenReturn($"Data for name must be a multiple of {sizeof(char)}.", false);
		string? deserialized = data.CreateString();
		if (deserialized is null)
			return false;
		this.EntityName = deserialized;
		return true;
	}

	public byte[] SerializeData() => this.EntityName.ToBytes();
}
