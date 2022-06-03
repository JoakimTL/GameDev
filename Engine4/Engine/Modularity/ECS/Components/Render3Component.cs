using Engine.Modularity.ECS.Networking;
using Engine.Rendering.Shaders;

namespace Engine.Modularity.ECS.Components;

[Identification( "ed3a1d07-92f6-4475-b23d-92a59ce0809c" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class Render3Component : RenderComponentBase {
	public Render3Component() : base( "cube", typeof( Entity3ShaderBundle ) ) { }

}
