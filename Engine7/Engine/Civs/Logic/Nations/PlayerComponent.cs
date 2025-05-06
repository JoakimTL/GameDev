using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;

public sealed class PlayerComponent : ComponentBase {

	public string Name { get; private set; } = string.Empty;
	public Vector4<float> MapColor { get; private set; }

	public PlayerComponent() {
		Name = $"Player {Random.Shared.Next():X8}";
		MapColor = (1, 1, 1, 1);
	}

	public void SetName( string name ) {
		if (Name == name)
			return;
		Name = name;
		this.InvokeComponentChanged();
	}

	public void SetColor( Vector4<float> color ) {
		if (MapColor == color)
			return;
		MapColor = color;
		this.InvokeComponentChanged();
	}
}