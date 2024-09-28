using Engine.Modules.ECS;
using Engine.Standard.ECS.Components;

namespace Engine.Standard.ECS.Systems;
public sealed class Motion3System() : SystemBase( NetworkAllowance.SERVER, typeof( Motion3Component ), typeof( Transform3Component ) ) {
	protected override void Update( IReadOnlyCollection<Entity> eligibleEntities, in double time, in double deltaTime ) {
		foreach (Entity e in eligibleEntities) {
			if (!e.TryGetComponent( out Motion3Component? mc ) || !e.TryGetComponent( out Transform3Component? tc ))
				continue;
			tc.Transform.Translation += mc.Velocity * deltaTime;
			tc.Transform.Rotation *= Rotor3.FromAxisAngle( mc.Momentum, deltaTime );
		}
	}
}
