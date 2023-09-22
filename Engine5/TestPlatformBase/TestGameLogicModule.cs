using Engine;
using Engine.GameLogic;
using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using Engine.GlobalServices;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using StandardPackage.ECPS.Components;
using StandardPackage.ECPS.Systems;
using StandardPackage.Rendering.Scenes;
using System.Numerics;

namespace TestPlatformBase;

public class TestGameLogicModule : GameLogicModuleBase, ITimedSystem {

	private Entity? _e;
	private float lastTime;

	public int SystemTickInterval => 50;

	protected override void OnInitialize() {
		//TODO: find other ways of creating entities!
		this._e = Get<EntityContainerService>()._container.Create();
		this._e.AddOrGet<LinearMovement3Component>().Impulse( new( 0, 0, 0 ) );
		this._e.AddOrGet<Mass3Component>();
		this._e.AddOrGet<RotationalMovement3Component>().Twirl( new( 0, 0, 0.5f ) );
		this._e.AddOrGet<Transform3Component>();
		this._e.AddOrGet<RenderInstance3DataComponent>();
		this._e.AddOrGet<RenderMaterialAssetComponent>().SetMaterial( "testMaterial" );
		this._e.AddOrGet<RenderMeshAssetComponent>().SetMesh( "box" );
		this._e.AddOrGet<RenderSceneComponent>().SetScene<Default3Scene>();
	}

	//TODO: interpolation between these ticks.

	protected override void OnUpdate( float time, float deltaTime ) {
        Global.Get<LoggedInputServiceTesterService>().LogEverything();
        if ( this._e is not null ) {
			var transform = this._e.Get<Transform3Component>();
			if ( transform is not null ) {
				transform.Transform.Translation = new Vector3( MathF.Sin( time ) * 2, MathF.Cos( time * 5 ) * 2, MathF.Cos( time ) * 2 );

				/*if ( !Global.Get<LoggedInputService>()[ GlfwBinding.Enums.Keys.L ] )
					this._e.Get<RenderInstance3DataComponent>()?.Set( transform.Transform.GlobalData, Vector4.One );*/
			}
			/*if ( Global.Get<LoggedInputService>()[ GlfwBinding.Enums.Keys.K ] )
				this._e.Get<RotationalMovement3Component>()?.Twirl( new( 1, 0, 0 ) );*/
		}
		if ( time - this.lastTime > 1 ) {
			//Console.WriteLine(Get<EntityContainerService>());
			//Console.WriteLine(Get<EntitySystemContainerService>());
			//Console.WriteLine( this._e );
			this.lastTime = time;
		}
	}
}


[ProcessBefore<Gravity3System, SystemBase>]
public class Gravity3ValueProvider : SystemBase, IGravity3ValueProvider {
	public bool IsAcceleration => true;

	private Vector3 _centerOfUniverse;

	public Vector3 GetGravity( Vector3 globalTranslation ) {
		var v = globalTranslation - this._centerOfUniverse;
		var l = MathF.Max( v.LengthSquared(), 1 );
		return v / l;
	}

	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) => this._centerOfUniverse = new Vector3( MathF.Cos( time ) * 100, 0, MathF.Sin( time ) * 100 );
}
