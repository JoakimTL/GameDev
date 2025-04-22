using Civs.World;

namespace Civs.Logic;

public sealed class WorldProviderService {

	public Globe? CurrentGlobe { get; private set; }

	public void GenerateWorld( uint subdivisionCount, double surfaceArea ) {
		this.CurrentGlobe = new( subdivisionCount, surfaceArea, 25000, 15000, 6_378_000 );
	}

	public void DeserializeWorld() {
		//TODO
	}
}