using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Attributes;

namespace StandardPackage.Rendering.Scenes;

[Identity(nameof(Default3Scene))]
public sealed class Default3Scene : LayerlessScene
{
	public Default3Scene() : base(new[] { "default", "shadow_dir", "shadow_point" }) { }
}
