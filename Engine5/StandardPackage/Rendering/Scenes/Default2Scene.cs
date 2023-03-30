using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Attributes;

namespace StandardPackage.Rendering.Scenes;

[Identity(nameof(Default2Scene))]
public sealed class Default2Scene : LayeredScene
{
	public Default2Scene() : base(new[] { "default" }) { }
}
