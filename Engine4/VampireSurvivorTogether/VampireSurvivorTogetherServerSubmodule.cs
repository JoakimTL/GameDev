//using System.Numerics;
//using Engine;
//using Engine.Data;
//using Engine.Modularity.ECS;
//using Engine.Modularity.ECS.Components;
//using Engine.Modularity.ECS.Networking;
//using Engine.Networking;
//using Engine.Structure;

//namespace VampireSurvivorTogether;

//[ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
//public class VampireSurvivorTogetherServerSubmodule : Submodule {

//	public VampireSurvivorTogetherServerSubmodule() : base( true ) {
//		OnInitialization += Initialized;
//		OnUpdate += Updated;
//	}

//	private void Initialized() {
//		Singleton<NetworkManager>().NewNamedConnection += AddPlayerEntity;
//		Singleton<EntityNetworkManager>();
//	}

//	private void AddPlayerEntity( NetworkConnection n ) {
//		Entity e = new( n.Name, n.NetworkId );
//		var t = new Transform2Component();
//		t.Transform.Translation = new Vector2( 0 );
//		t.Transform.Rotation = 0;
//		t.Transform.Scale = new Vector2( 0.25f );
//		e.AddComponent( t );
//		e.AddComponent( new ClientInputComponent() );
//		e.AddComponent( new ClientUnitController() );
//		e.AddComponent( new SpinnerComponent() );
//		var rndr = new Render2Component();
//		rndr.SetTemplate( "Flumbo" );
//		e.AddComponent( rndr );
//		Random rand = new();
//		e.GetComponent<Render2Component>()!.Color = new Vector4(
//			rand.NextSingle() * 0.25f + 0.25f,
//			rand.NextSingle() * 0.25f + 0.25f,
//			rand.NextSingle() * 0.25f + 0.25f,
//			1 );
//		Singleton<EntityManager>().AddEntity( e );
//	}

//	private void Updated( float time, float deltaTime ) {

//	}

//	protected override bool OnDispose() {
//		return true;
//	}
//}

//[Identification( "d60608d8-5dfa-42de-afd5-4e9cd8a8eb75" )]
//[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
//public class MoveComponent : SerializableComponent, IUpdateable {
//	public Vector2 Movement { get; private set; }

//	public bool Active { get; private set; }

//	public MoveComponent() {
//		this.Active = true;
//		this.Movement = new Vector2();
//	}

//	public void SetActive( bool a ) {
//		this.Active = a;
//		TriggerChanged();
//	}

//	public void SetMovement( Vector2 m ) {
//		this.Movement = m;
//		TriggerChanged();
//	}

//	public void Update( float time, float deltaTime ) {
//		if ( this.Active )
//			if ( this.Parent.TryGetComponent( out Transform2Component? t ) )
//				t.Transform.Translation += this.Movement * deltaTime;
//	}

//	public override void SetFromSerializedData( byte[] data ) {
//		var dataSplit = Segmentation.Parse( data );
//		if ( dataSplit is null )
//			return;
//		SetActive( DataUtils.ToUnmanaged<bool>( dataSplit[ 0 ] ) ?? false );
//		SetMovement( DataUtils.ToUnmanaged<Vector2>( dataSplit[ 1 ] ) ?? Vector2.One );
//	}

//	protected override byte[]? GetSerializedData() => Segmentation.Segment( DataUtils.ToBytes( this.Active ), DataUtils.ToBytes( this.Movement ) );
//}
