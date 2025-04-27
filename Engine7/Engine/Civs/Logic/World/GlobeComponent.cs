using Civs.World.NewWorld;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class GlobeComponent : ComponentBase {

	private GlobeModel? _globe;

	public GlobeModel Globe => _globe ?? throw new InvalidOperationException( "Globe is not set." );

	public void SetGlobe( GlobeModel globe ) {
		if (this._globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this._globe = globe;
	}

}
