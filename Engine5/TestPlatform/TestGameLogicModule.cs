using Engine.GameLogic;
using Engine.GameLogic.ECS;
using Engine.GameLogic.ECS.Components;
using Engine.Structure.Interfaces;
using StandardPackage.ECS.Components;

namespace TestPlatform;

internal class TestGameLogicModule : GameLogicModuleBase, ITimedSystem {

	private Entity? _e;
	private float lastTime;

	public int SystemTickInterval => 10;

	protected override void OnInitialize() {
		_e = new();
		Get<EntityContainerService>().Add( _e );
		_e.AddOrGet<LinearMovement3Component>().Impulse(new (1, 0, 0));
		_e.AddOrGet<Mass3Component>();
		_e.AddOrGet<RotationalMovement3Component>().Twirl(new(5, 0, 0));
		_e.AddOrGet<Transform3Component>();
	}

	protected override void OnUpdate( float time, float deltaTime ) {
		if ( time - lastTime > 1 ) {
			Console.WriteLine(Get<EntityContainerService>());
			Console.WriteLine(_e);
			lastTime = time;
		}
	}
}
