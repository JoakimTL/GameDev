namespace Civlike.World.GenerationState;

public abstract class GlobeGeneratorParameterBase {
	protected GlobeGeneratorParameterBase( uint subdivisions, double globeRadius, int seed, int playerCount ) {
		if (subdivisions < 6)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivisions must be at least 6." );
		if (subdivisions > 10)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivisions must be at most 10." );
		if (globeRadius <= 1500000)
			throw new ArgumentOutOfRangeException( nameof( globeRadius ), "Radius must be greater than 1500000 meters." );
		this.Subdivisions = subdivisions;
		this.GlobeRadius = globeRadius;
		this.GenerationSeed = seed;
		this.PlayerCount = playerCount;
	}

	public uint Subdivisions { get; }
	public double GlobeRadius { get; }
	public int GenerationSeed { get; }
	public int PlayerCount { get; }
}