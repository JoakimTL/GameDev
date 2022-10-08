using Engine.Rendering.InputHandling;
using Engine.Rendering.Standard.UI.Standard.Text;
using GLFW;

namespace Engine.Rendering.Standard.UI.Standard;

public class Button : ColoredMeshDisplay {

	private readonly Label _label;

	public Button( Font font, string text ) : base( Resources.Render.Mesh2.Square ) {
		this._label = new( font, text, 1 );
		this._label.SetParent( this );

		this.ButtonPressed += OnButtonPress;
	}

	private void OnButtonPress( UIElement e, MouseButton b, ModifierKeys mod, MouseState state ) {

	}
}
