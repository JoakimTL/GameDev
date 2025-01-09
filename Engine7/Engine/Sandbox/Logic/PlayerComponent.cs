using Engine.Module.Entities.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic;
public sealed class PlayerComponent : ComponentBase {

	public Guid PlayerId { get; }

	public PlayerComponent() {
		this.PlayerId = Guid.NewGuid();
	}

}
