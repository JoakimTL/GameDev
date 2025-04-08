using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations;

//4096 * 2 * 16 * 128 * 256 * 4
//tiles * sexes * education levels * age * professions * 4 bytes = 16 384 MiB

public readonly struct PartOfPopulationKey {
	private readonly uint _professionMemoryId;
	private readonly PoPData _popData;

	/// <param name="profession">Which profession this pop has. Null means no profession.</param>
	/// <param name="sex">Which sex this pop has.</param>
	/// <param name="educationLevel">The education level of this pop. 0 means base level, 10 grants a 100% efficiency boost from their work.</param>
	/// <param name="ageYears">Age of the population in the PoP.</param>
	public PartOfPopulationKey( ProfessionTypeBase? profession, Sex sex, byte educationLevel, byte ageYears ) {
		this._professionMemoryId = profession?.MemoryId ?? 0;
		_popData = new( sex, educationLevel, ageYears );
	}

	//2 * 10 * 2 * 24 = 960

	public ProfessionTypeBase? Profession => Definitions.Professions.Get( _professionMemoryId );
	public Sex Sex => _popData.Sex;
	public byte EducationLevel => _popData.EducationLevel;
	public byte AgeYears => _popData.AgeYears;

	public override bool Equals( object? obj ) => obj is PartOfPopulationKey other && PoPData.Equals( other._popData, _popData ) && _professionMemoryId == other._professionMemoryId;

	public override int GetHashCode() => HashCode.Combine( Profession, _popData );

	public static bool operator ==( PartOfPopulationKey left, PartOfPopulationKey right ) => left.Equals( right );

	public static bool operator !=( PartOfPopulationKey left, PartOfPopulationKey right ) => !(left == right);

	private readonly struct PoPData {
		private readonly uint _data;

		// bbbb_bbbi_iiii_iiis

		public PoPData( Sex sex, byte educationLevel, byte ageYears ) {
			_data = 0;
			_data |= (byte) sex & 0b1u;
			_data |= (uint) educationLevel & 0b1111 << 1;
			_data |= (uint) ageYears << 5;
		}

		public Sex Sex => (Sex) (_data & 0b1u);
		public byte EducationLevel => (byte) ((_data >> 1) & 0b1111u);
		public byte AgeYears => (byte) ((_data >> 9) & 0b111_1111u);

		public static bool Equals( PoPData a, PoPData b ) => a._data == b._data;
	}
}
