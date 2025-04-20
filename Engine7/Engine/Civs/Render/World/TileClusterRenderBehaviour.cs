using Civs.Logic.World;
using Engine.Module.Render.Entities;

namespace Civs.Render.World;
public sealed class TileClusterRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype> {
	public override void Update( double time, double deltaTime ) {
	}

	protected override bool InternalDispose() {
		return true;
	}
}
