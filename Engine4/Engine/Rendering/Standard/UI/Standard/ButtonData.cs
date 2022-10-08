using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.UI.Standard;

public class ButtonData {

	public Color16x4 Color { get; set; }

	public ButtonData() {
		this.Color = Color16x4.White;
	}
}
