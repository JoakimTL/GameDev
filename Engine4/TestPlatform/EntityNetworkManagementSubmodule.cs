using System.Numerics;
using Engine;
using Engine.Modularity.ECS;
using Engine.Modularity.ECS.Components;
using Engine.Modularity.ECS.Networking;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Networking;
using TestPlatform.Systems;

namespace TestPlatform;

[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class EntityNetworkManagementSubmodule : Submodule {

	private Entity? _entity1;
	private Entity? _entity2;

	public EntityNetworkManagementSubmodule() : base( true ) {
		OnInitialization += Initialized;
		OnUpdate += Update;
	}

	private void Initialized() {
		Singleton<NetworkManager>();
		Singleton<EntityNetworkManager>();
		this._entity1 = new Entity( "Testboi", Guid.NewGuid(), 0 );
		this._entity1.AddComponent( new Transform3Component() );
		this._entity1.AddComponent( new Render3Component() );
		this._entity2 = new Entity( "Testclientinput", Guid.NewGuid(), Singleton<NetworkManager>().Connections.FirstOrDefault()?.NetworkId ?? 0 );
		this._entity2.AddComponent( new Transform3Component() );
		this._entity2.AddComponent( new Render3Component() );
		this._entity2.AddComponent( new ClientInputComponent() );
		this._entity2.AddComponent( new PlayerInputSystem() );
		Singleton<EntityManager>().AddEntity( this._entity1 );
		Singleton<EntityManager>().AddEntity( this._entity2 );
	}

	private void Update( float time, float deltaTime ) {
		Transform3Component? tc = this._entity1?.GetComponent<Transform3Component>();
		if ( tc is not null ) {
			tc.Transform.Translation = new( MathF.Sin( time * 4 ) * 3, 0, 0 );
			tc.Transform.Scale = new( (MathF.Sin( time * 3 ) * .5f) + 1, 1, 1 );
			tc.Transform.Rotation = Quaternion.CreateFromYawPitchRoll( MathF.Sin( time * 3 ) * 180 / MathF.PI, 90, 0);
		}
		Render3Component? rc = this._entity1?.GetComponent<Render3Component>();
		if ( rc is not null ) {
			if ( (int) time % 10 < 5 ) {
				rc.Color = new Vector4( 1, 0, 0, 1 );
				rc.Mesh = "cube";
			} else {
				rc.Color = new Vector4( 0, 0, 1, 1 );
				rc.Mesh = "icosphere#3";
			}
		}
	}

	protected override bool OnDispose() => true;
}
