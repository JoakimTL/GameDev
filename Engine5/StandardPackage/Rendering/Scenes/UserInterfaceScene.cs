using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Attributes;

namespace StandardPackage.Rendering.Scenes;

[Identity(nameof(UserInterfaceScene))]
public sealed class UserInterfaceScene : LayeredScene
{
	public UserInterfaceScene() : base(new[] { "default", "occlusion" }) { }
}
