using System.Numerics;
using Engine;
using Engine.Modularity.ECS;
using Engine.Modularity.ECS.Components;
using Engine.Modularity.ECS.Networking;

namespace VampireSurvivorTogether;

[Identification( "4b6d67fb-1041-4a9a-89a9-177d296e31da" )]
//[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
internal class ClientUnitController : SerializableComponent, IUpdateable {
	public bool Active => true;

	public ClientUnitController() {
	}

	public void Update( float time, float deltaTime ) {
		if ( !this.Parent.TryGetComponent( out ClientInputComponent? input ) )
			return;
		if ( !this.Parent.TryGetComponent( out Transform2Component? transform ) )
			return;

		Vector2 moveVec = new();

		if ( input.IsKeyPressed( GLFW.Keys.W ) )
			moveVec += new Vector2( 0, 1 );
		if ( input.IsKeyPressed( GLFW.Keys.A ) )
			moveVec += new Vector2( -1, 0 );
		if ( input.IsKeyPressed( GLFW.Keys.S ) )
			moveVec += new Vector2( 0, -1 );
		if ( input.IsKeyPressed( GLFW.Keys.D ) )
			moveVec += new Vector2( 1, 0 );

		if (moveVec.X != 0 || moveVec.Y != 0)
			transform.Transform.Translation += Vector2.Normalize( moveVec ) * deltaTime;

		if ( input.IsButtonPressed( GLFW.MouseButton.Left ) ) {
			Random rnd = new Random();
			var t = new Transform2Component();
			t.Transform.Translation = transform.Transform.GlobalTranslation;
			t.Transform.Rotation = rnd.NextSingle() * MathF.PI * 2;
			t.Transform.Scale = new System.Numerics.Vector2( rnd.NextSingle() * 0.025f + 0.025f );
			//var m = new MoveComponent();
			//m.SetMovement( Vector2.Normalize( new Vector2( rnd.NextSingle() * 2 - 1, rnd.NextSingle() * 2 - 1 ) ) * 0.2f );
			Entity e1 = new( "p" + time, 0 );
			e1.AddComponent( t );
			e1.AddComponent( new Render2Component() );
			//e1.AddComponent( m );
			e1.AddComponent( new DeathComponent() );
			e1.AddComponent( new SpinnerComponent() );
			this.Parent.Manager!.AddEntity( e1 );
		}
	}
}

[Identification( "2b6ae12e-ed89-4907-aaa8-5e405e1a1a29" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class DeathComponent : SerializableComponent, IUpdateable {
	public bool Active => true;

	public void Update( float time, float deltaTime ) {
		if ( this.Parent.CreationTime + 5 < time )
			this.Parent.Kill();
	}
}

[Identification( "2b6ae12e-ed89-4907-aaa7-5e405e1a1a29" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class SpinnerComponent : SerializableComponent, IUpdateable {
	public bool Active => true;

	public void Update( float time, float deltaTime ) {
		if ( this.Parent.TryGetComponent( out Transform2Component? transform ) )
			transform.Transform.Rotation += deltaTime;
	}
}