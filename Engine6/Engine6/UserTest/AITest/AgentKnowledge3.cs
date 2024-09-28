using Engine;
using Engine.Modules.ECS;
using Engine.Standard.ECS.Components;

namespace UserTest.AITest;

public sealed class AgentKnowledge3 : AgentKnowledge<Vector3<double>> {
	protected override Vector3<double>? GetLocation( Entity e ) => !e.TryGetComponent( out Transform3Component? t3c ) ? null : t3c.Transform.GlobalTranslation;
}
