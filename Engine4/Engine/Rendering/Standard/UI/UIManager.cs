using Engine.Rendering.InputHandling;
using Engine.Rendering.Standard.Scenes;
using GLFW;

namespace Engine.Rendering.Standard.UI;
public class UIManager : Identifiable, IUpdateable, IKeyboardEventListener, IMouseEventListener {

	private readonly HashSet<UIElement> _elements;
	private readonly LayeredScene _scene;

	public Scene Scene => this._scene;
	public bool Active => true;

	public UIManager() {
		this._elements = new HashSet<UIElement>();
		this._scene = new LayeredScene();
		Resources.Render.Window.KeyboardEvents.AddListener( this );
		Resources.Render.Window.MouseEvents.AddListener( this );
	}

	public void Add( UIElement e ) {
		lock ( this._elements )
			this._elements.Add( e );
	}

	public void Remove( UIElement e ) {
		lock ( this._elements )
			this._elements.Remove( e );
	}

	public void Update( float time, float deltaTime ) {
		lock ( this._elements )
			foreach ( UIElement e in this._elements )
				e.Update( time, deltaTime );
	}

	public void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) {
		foreach ( UIElement e in this._elements )
			e.OnKeyReleased( key, mods, scanCode );
	}

	public void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) {
		foreach ( UIElement e in this._elements )
			e.OnKeyPressed( key, mods, scanCode );
	}

	public void OnKeyRepeated( Keys key, ModifierKeys mods, int scanCode ) {
		foreach ( UIElement e in this._elements )
			e.OnKeyRepeated( key, mods, scanCode );
	}

	public void OnCharacterWritten( uint charCode, string character, ModifierKeys mods ) {
		foreach ( UIElement e in this._elements )
			e.OnCharacterWritten( charCode, character, mods );
	}

	public void OnButtonReleased( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnButtonReleased( btn, modifier, state );
	}

	public void OnButtonPressed( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnButtonPressed( btn, modifier, state );
	}

	public void OnButtonRepeat( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnButtonRepeat( btn, modifier, state );
	}

	public void OnMouseCursorMove( MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnMouseCursorMove( state );
	}

	public void OnMouseLockedMove( MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnMouseLockedMove( state );
	}

	public void OnMouseScroll( double xAxis, double yAxis, MouseState state ) {
		foreach ( UIElement e in this._elements )
			e.OnMouseScroll( xAxis, yAxis, state );
	}
}

/*
public class UIElementSceneObject : ClosedSceneObject<Vertex2, >{

}
*/