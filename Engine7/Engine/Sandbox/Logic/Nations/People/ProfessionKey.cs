using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations.People;

public readonly struct ProfessionKey {
	private readonly int _key;

	public ProfessionKey( ushort educationLevel, ProfessionTypeBase profession ) {
		_key = (int) ((uint) educationLevel << 16 | profession.MemoryId & 0xff_ff);
	}

	public ushort EducationLevel => (ushort) (_key >> 16);
	public ushort ProfessionMemoryId => (ushort) (_key & 0xff_ff);

	public ProfessionTypeBase Profession => Definitions.Professions.Get( ProfessionMemoryId ) ?? throw new ArgumentOutOfRangeException( $"Profession with memory id {ProfessionMemoryId} not found." );

	public override string ToString() => $"{Profession?.Name} ({EducationLevel})";

	public override bool Equals( object? obj ) => obj is ProfessionKey other && _key == other._key;

	public override int GetHashCode() => this._key;

	public static bool operator ==( ProfessionKey left, ProfessionKey right ) => left.Equals( right );
	public static bool operator !=( ProfessionKey left, ProfessionKey right ) => !(left == right);
}