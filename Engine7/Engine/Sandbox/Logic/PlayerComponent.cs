using Engine.Module.Entities.Container;

namespace Sandbox.Logic;
public sealed class PlayerComponent : ComponentBase {

	public Guid PlayerId { get; }

	public PlayerComponent() {
		this.PlayerId = Guid.NewGuid();
	}

}
