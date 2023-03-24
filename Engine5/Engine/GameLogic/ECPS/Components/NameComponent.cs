using Engine.Structure.Interfaces;

namespace Engine.GameLogic.ECPS.Components;

public sealed class NameComponent : ComponentBase, ICustomizedSerializable {
	public string EntityName { get; private set; }

	protected override string UniqueNameTag => $"{EntityName}";

	public static Guid SerializationIdentity { get; } = new( "21d318d8-f61c-4dee-99aa-9513628b8976" );
    public bool ShouldSerialize => true;

    public NameComponent() {
		EntityName = "Unnamed";
	}

	public void SetName( string newName ) {
		EntityName = newName;
		AlertComponentChanged();
	}

	public bool DeserializeData( byte[] data ) {
		if ( data.Length % sizeof( char ) != 0 )
			return Log.WarningThenReturn( $"Data for name must be a multiple of {sizeof( char )}.", false );
		string? deserialized = data.CreateString();
		if ( deserialized is null )
			return false;
		EntityName = deserialized;
		return true;
	}

	public byte[] SerializeData() => EntityName.ToBytes();
}
