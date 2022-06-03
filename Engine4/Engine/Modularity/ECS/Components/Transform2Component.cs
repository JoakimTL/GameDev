using System.Numerics;
using Engine.Data.Datatypes.Transforms;
using Engine.Modularity.ECS.Networking;

namespace Engine.Modularity.ECS.Components;

[Identification( "085bb5cd-9269-4bc7-887b-a6eeff7a17be" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class Transform2Component : TransformComponentBase<Vector2, float, Vector2> {
	public Transform2Component() : base( new Transform2() ) { }
}
