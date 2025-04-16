namespace Sandbox.Logic.Nations.People;

public readonly struct PhysicalTrait {
	private readonly byte _traits;

	public PhysicalTrait( Sex sex, PhenotypicCategory phenotypicCategory ) {
		_traits = (byte) ((byte) sex << 7 | (byte) phenotypicCategory & 0b0111_1111);
	}

	public Sex Sex => (Sex) (_traits >> 7);
	public PhenotypicCategory PhenotypicCategory => (PhenotypicCategory) (_traits & 0b0111_1111);
}