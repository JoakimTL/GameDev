using Engine.Module.Entities.Container;

namespace Civlike.World.Components;

public sealed class GlobeComponent : ComponentBase {

	private Globe? _globe;

	public Globe Globe => this._globe ?? throw new InvalidOperationException( "Globe is not set." );

	public void SetGlobe( Globe globe ) {
		if (this._globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this._globe = globe;
	}

}
