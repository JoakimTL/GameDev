using System.Numerics;
using Engine.Data.Datatypes.Transforms;
using Engine.Rendering.InputHandling;
using GLFW;

namespace Engine.Rendering.Standard.UI;

public class UIElement : Identifiable {

	/*
	 * Mouse events
	 * - Button
	 * - Move
	 * - Scrollwheel
	 * Keyboard events
	 * - Key
	 * Display events
	 * - Window resized
	 * Logic events
	 * - Update
	 * - Activation/deactivation
	 * - Layer changed
	 * Hierarchy events
	 * - Parent set
	 * - Child added
	 * 
	 * Required render functionality
	 * - Stencil shading
	 * - Collision detection
	 */

	private IUIConstraint? _constraint;
	private ISceneObject? _sceneObject;
	private readonly Transform2 _transform;
	private readonly HashSet<UIElement> _children;
	public bool Active { get; private set; }
	public bool Rendering { get; private set; }
	public bool ShouldRender { get; private set; }
	public TransformReadonly<Vector2, float, Vector2> Transform => this._transform.Readonly;
	public UIElement? Parent { get; private set; }
	public IReadOnlyCollection<UIElement> Children => this._children;

	#region Events
	public event Action<UIElement>? Updated;
	public event Action<UIElement, UIElement?>? ParentSet;
	public event Action<UIElement, UIElement>? ChildAdded;
	public event Action<UIElement, UIElement>? ChildRemoved;
	public event Action<UIElement, ISceneObject>? SceneObjectAdded;
	public event Action<UIElement, ISceneObject>? SceneObjectRemoved;
	public event Action<UIElement>? Activated;
	public event Action<UIElement>? Deactivated;
	public event Action<UIElement>? RenderStateChanged;
	#region Input
	public event Action<UIElement, Keys, ModifierKeys, int>? KeyPressed;
	public event Action<UIElement, Keys, ModifierKeys, int>? KeyReleased;
	public event Action<UIElement, Keys, ModifierKeys, int>? KeyRepeated;
	public event Action<UIElement, uint, string, ModifierKeys>? CharacterWritten;
	public event Action<UIElement, MouseButton, ModifierKeys, MouseState>? ButtonPressed;
	public event Action<UIElement, MouseButton, ModifierKeys, MouseState>? ButtonReleased;
	public event Action<UIElement, MouseButton, ModifierKeys, MouseState>? ButtonRepeated;
	public event Action<UIElement, MouseState>? MouseCursorMoved;
	public event Action<UIElement, MouseState>? MouseLockedMoved;
	public event Action<UIElement, double, double, MouseState>? MouseScrolled;
	#endregion
	#endregion

	public UIElement() {
		this._transform = new Transform2();
		this._sceneObject = null;
		this._constraint = null;
		this._children = new HashSet<UIElement>();
		this.Parent = null;
		this.Active = false;
		this.ShouldRender = true;
		SceneObjectAdded += SceneStateChanged;
		SceneObjectRemoved += SceneStateChanged;
		Activated += SceneStateChanged;
		Deactivated += SceneStateChanged;
		RenderStateChanged += SceneStateChanged;
		ParentSet += OnParentChanged;
	}

	private void OnParentChanged( UIElement e, UIElement? parent ) {
		if ( parent is null )
			return;
		if ( parent.Active && !this.Active )
			Activate();
	}

	public void Activate() {
		foreach ( UIElement e in this._children )
			e.Activate();

		this.Active = true;
		Activated?.Invoke( this );
	}

	public void Deactivate() {
		foreach ( UIElement e in this._children )
			e.Deactivate();

		this.Active = false;
		Deactivated?.Invoke( this );
	}

	public void SetRenderState( bool rendering ) {
		if ( rendering == this.ShouldRender )
			return;
		this.ShouldRender = rendering;
		RenderStateChanged?.Invoke( this );
	}

	private void SceneStateChanged( UIElement e, ISceneObject so ) => SceneStateChangedInternal( so );

	private void SceneStateChanged( UIElement e ) => SceneStateChangedInternal( this._sceneObject );

	/// <param name="so">The currently active scene object or the previously active scene object (if removed).</param>
	private void SceneStateChangedInternal( ISceneObject? so ) {
		if ( so is null )
			return;
		bool wasRendering = this.Rendering;
		this.Rendering = this.Active && this.ShouldRender && this._sceneObject is not null;
		if ( this.Rendering != wasRendering )
			if ( this.Rendering ) {
				Resources.Render.Get<UIManager>().Scene.AddSceneObject( so );
			} else {
				Resources.Render.Get<UIManager>().Scene.RemoveSceneObject( so );
			}
	}

	public void SetParent( UIElement e ) {
		if ( e == this.Parent )
			return;
		this.Parent?.RemoveChild( this );
		this.Parent = e;
		this._transform.Parent = this.Parent._transform;
		this.Parent?.AddChild( this );
		ParentSet?.Invoke( this, this.Parent );
	}

	private void AddChild( UIElement e ) {
		if ( this._children.Add( e ) )
			ChildAdded?.Invoke( this, e );
	}

	private void RemoveChild( UIElement e ) {
		if ( this._children.Remove( e ) )
			ChildRemoved?.Invoke( this, e );
	}

	public void SetConstraint( IUIConstraint constriant ) => this._constraint = constriant;

	protected void SetSceneObject( ISceneObject sceneObject ) {
		if ( this._sceneObject is not null )
			SceneObjectRemoved?.Invoke( this, this._sceneObject );
		this._sceneObject = sceneObject;
		if ( this._sceneObject is not null )
			SceneObjectAdded?.Invoke( this, this._sceneObject );
	}

	public void Update( float time, float deltaTime ) {
		if ( !this.Active )
			return;

		Updated?.Invoke( this );
		this._constraint?.Apply( time, deltaTime, this._transform );
	}

	public void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnKeyReleased( key, mods, scanCode );

		KeyReleased?.Invoke( this, key, mods, scanCode );
	}

	public void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnKeyPressed( key, mods, scanCode );

		KeyPressed?.Invoke( this, key, mods, scanCode );
	}

	public void OnKeyRepeated( Keys key, ModifierKeys mods, int scanCode ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnKeyRepeated( key, mods, scanCode );

		KeyRepeated?.Invoke( this, key, mods, scanCode );
	}

	public void OnCharacterWritten( uint charCode, string character, ModifierKeys mods ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnCharacterWritten( charCode, character, mods );

		CharacterWritten?.Invoke( this, charCode, character, mods );
	}

	public void OnButtonReleased( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnButtonReleased( btn, modifier, state );

		ButtonReleased?.Invoke( this, btn, modifier, state );
	}

	public void OnButtonPressed( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnButtonPressed( btn, modifier, state );

		ButtonPressed?.Invoke( this, btn, modifier, state );
	}

	public void OnButtonRepeat( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnButtonRepeat( btn, modifier, state );

		ButtonRepeated?.Invoke( this, btn, modifier, state );
	}

	public void OnMouseCursorMove( MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnMouseCursorMove( state );

		MouseCursorMoved?.Invoke( this, state );
	}

	public void OnMouseLockedMove( MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnMouseLockedMove( state );

		MouseLockedMoved?.Invoke( this, state );
	}

	public void OnMouseScroll( double xAxis, double yAxis, MouseState state ) {
		if ( !this.Active )
			return;

		foreach ( UIElement e in this._children )
			e.OnMouseScroll( xAxis, yAxis, state );

		MouseScrolled?.Invoke( this, xAxis, yAxis, state );
	}
}

/*
public class UIElementSceneObject : ClosedSceneObject<Vertex2, >{

}
*/