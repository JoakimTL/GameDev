using Engine;
using Engine.Modules.ECS;
using Engine.Standard.ECS.Components;

namespace UserTest.AITest;

public sealed class AgentKnowledge2 : AgentKnowledge<Vector2<double>> {
	protected override Vector2<double>? GetLocation( Entity e ) => !e.TryGetComponent( out Transform2Component? t2c ) ? null : t2c.Transform.GlobalTranslation;
}
