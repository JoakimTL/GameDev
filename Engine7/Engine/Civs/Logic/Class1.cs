using Civs.World;
using Engine.Modularity;

namespace Civs.Logic;
internal class Class1 {
}

public sealed class CivsGameLogicModule : ModuleBase {
	private readonly WorldProviderService _worldProviderService;

	public Globe? CurrentGlobe { get; private set; }

	public CivsGameLogicModule( WorldProviderService worldProviderService ) : base( false, 40 ) {
		OnInitialize += Init;
		OnUpdate += Update;
		this._worldProviderService = worldProviderService;
	}

	private void Init() {

	}

	private void Update( double time, double deltaTime ) {
		throw new NotImplementedException();
	}
}

public sealed class WorldProviderService {

	public Globe? CurrentGlobe { get; private set; }

	public void GenerateWorld() {
		this.CurrentGlobe = new( 9, 509600000 );
	}

	public void DeserializeWorld() {
		//TODO
	}
}