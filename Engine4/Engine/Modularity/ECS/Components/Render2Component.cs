using Engine.Modularity.ECS.Networking;
using Engine.Rendering.Standard.SceneObjects;

namespace Engine.Modularity.ECS.Components;

[Identification( "3640524d-1a81-4f83-b72b-81b1899e83fb" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class Render2Component : RenderComponentBase {
	public Render2Component() : base( SceneObjectTemplate.Square ) { }
}
