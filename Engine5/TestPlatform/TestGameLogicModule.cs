using Engine.GameLogic;
using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using StandardPackage.ECPS.Components;
using StandardPackage.ECPS.Systems;
using System.Numerics;

namespace TestPlatform;

internal class TestGameLogicModule : GameLogicModuleBase, ITimedSystem {

	private Entity? _e;
	private float lastTime;

	public int SystemTickInterval => 10;

	protected override void OnInitialize() {
		_e = Get<EntityContainerService>()._container.Create();
		_e.AddOrGet<LinearMovement3Component>().Impulse( new( 1, 0, 0 ) );
		_e.AddOrGet<Mass3Component>();
		_e.AddOrGet<RotationalMovement3Component>().Twirl( new( 5, 0, 0 ) );
		_e.AddOrGet<Transform3Component>();
	}

	protected override void OnUpdate( float time, float deltaTime ) {
		if ( time - lastTime > 1 ) {
			Console.WriteLine( Get<EntityContainerService>() );
			Console.WriteLine( Get<EntitySystemContainerService>() );
			Console.WriteLine( _e );
			lastTime = time;
		}
	}
}


[ProcessBefore<Gravity3System, SystemBase>]
public class Gravity3ValueProvider : SystemBase, IGravity3ValueProvider {
	public bool IsAcceleration => true;

	private Vector3 _centerOfUniverse;

	public Vector3 GetGravity( Vector3 globalTranslation ) {
		var v = globalTranslation - _centerOfUniverse;
		var l = MathF.Max( v.LengthSquared(), 1 );
		return v / l;
	}

	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		_centerOfUniverse = new Vector3( MathF.Cos( time ) * 100, 0, MathF.Sin( time ) * 100 );
	}
}
