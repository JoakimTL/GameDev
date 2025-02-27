using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations;

public readonly struct PartOfPopulationKey {
	private readonly PoPData _popData;

	/// <param name="profession">Which profession this pop has.</param>
	/// <param name="sex">Which sex this pop has.</param>
	/// <param name="educationLevel">The education level of this pop. 0 means base level, 10 grants a 100% efficiency boost from their work.</param>
	/// <param name="ageYears">Age of the population in the PoP.</param>
	public PartOfPopulationKey( ProfessionTypeBase? profession, Sex sex, byte educationLevel, ushort ageYears ) {
		this.Profession = profession;
		_popData = new( sex, educationLevel, ageYears );
	}

	//2 * 10 * 2 * 24 = 960

	public ProfessionTypeBase? Profession { get; }
	public Sex Sex => _popData.Sex;
	public byte EducationLevel => _popData.EducationLevel;
	public uint AgeYears => _popData.AgeYears;

	public override bool Equals( object? obj ) => obj is PartOfPopulationKey other && PoPData.Equals( other._popData, _popData ) && ReferenceEquals( other.Profession, Profession );

	public override int GetHashCode() => HashCode.Combine( Profession, _popData );

	public static bool operator ==( PartOfPopulationKey left, PartOfPopulationKey right ) => left.Equals( right );

	public static bool operator !=( PartOfPopulationKey left, PartOfPopulationKey right ) => !(left == right);

	private readonly struct PoPData {
		private readonly uint _data;

		// s_iiii_iiii_bbbb_bbbb_bbbb_bbbb

		public PoPData( Sex sex, byte educationLevel, ushort ageYears ) {
			_data = 0;
			_data |= (byte) sex & 0b1u;
			_data |= (uint) educationLevel << 1;
			_data |= (uint) ageYears << 9;
		}

		public Sex Sex => (Sex) (_data & 0b1u);
		public byte EducationLevel => (byte) ((_data >> 1) & 0b1111_1111u);
		public ushort AgeYears => (ushort) ((_data >> 9) & 0b1111_1111_1111_1111u);

		public static bool Equals( PoPData a, PoPData b ) => a._data == b._data;
	}
}
