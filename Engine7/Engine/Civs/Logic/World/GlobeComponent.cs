using Civs.World;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class GlobeComponent : ComponentBase {

	public Globe? Globe { get; private set; }

	public void SetGlobe( Globe globe ) {
		if (this.Globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this.Globe = globe;
	}

}
