using Engine;
using Engine.Modularity.ECS;
using Engine.Modularity.ECS.Components;
using Engine.Modularity.ECS.Networking;
using GLFW;

namespace TestPlatform.Systems;

[Identification( "29a74f87-0ae4-40c9-9c0d-88a6f0d4dfb9" )]
[Network(ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp)]
public class PlayerInputSystem : Component, IUpdateable {
	public bool Active => true;

	public PlayerInputSystem() {

	}

	public void Update( float time, float deltaTime ) {
		if ( !this.Parent.TryGetComponent( out Transform3Component? t3 ) )
			return;
		if ( !this.Parent.TryGetComponent( out ClientInputComponent? cli ) )
			return;

		if ( cli.IsKeyPressed( Keys.U ) )
			t3.Transform.Translation += t3.Transform.Rotation.Up() * deltaTime;
		if ( cli.IsKeyPressed( Keys.J ) )
			t3.Transform.Translation += t3.Transform.Rotation.Down() * deltaTime;
	}
}
