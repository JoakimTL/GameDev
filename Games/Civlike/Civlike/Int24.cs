//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike;

public readonly struct Int24 {
	private readonly byte _d0, _d1, _d2;

	public Int24( uint index ) {
		_d0 = (byte) (index & 0xFF);
		_d1 = (byte) (index >> 8 & 0xFF);
		_d2 = (byte) (index >> 16 & 0xFF);
	}

	public uint Index => _d0 | (uint) _d1 << 8 | (uint) _d2 << 16;

	public static implicit operator uint( Int24 index ) => index.Index;
	public static implicit operator Int24( uint index ) => new( index );

	public override bool Equals( object? obj ) => obj is Int24 vertex && vertex == this;
	public override int GetHashCode() => (int) Index;
	public static bool operator ==( Int24 left, Int24 right ) => left.Index == right.Index;
	public static bool operator !=( Int24 left, Int24 right ) => !(left == right);
}