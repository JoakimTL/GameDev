using Engine.Module.Render.Input;
using Engine.Transforms;
using Engine.Transforms.Models;
using System.Xml.Linq;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceComponentBase : Identifiable, IRemovable {
	protected UserInterfaceElementBase Element { get; }

	private readonly List<UserInterfaceComponentBase> _children;
	public UserInterfaceComponentBase? Parent { get; private set; }

	public UserInterfaceComponentPlacement Placement { get; }
	protected Transform2<double> Transform { get; }
	public TransformInterface<double, Vector2<double>, double, Vector2<double>> TransformInterface { get; }

	public uint RenderLayer { get; private set; }

	public AABB<Vector2<double>> PlacementBounds { get; private set; }

	public bool Visible { get; private set; } = true;
	public bool Removed { get; private set; } = false;

	public event Action? PlacementBoundsChanged;
	public event RemovalHandler? OnRemoved;

	private bool _placementChanged = false;

	public UserInterfaceComponentBase( UserInterfaceElementBase element) {
		Element = element;
		Transform = new();
		Transform.Nickname = GetType().Name;
		TransformInterface = Transform.Interface;
		Transform.OnMatrixChanged += OnMatrixChanged;
		PlacementBounds = new AABB<Vector2<double>>( -1, 1 );
		Placement = new( this );
		_children = [];
		Parent = null;
		RenderLayer = element.BaseLayer;
		Element.AddComponent( this );
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => _placementChanged = true;

	protected T AddChild<T>( T child, uint renderLayerOffset = 1 ) where T : UserInterfaceComponentBase {
		if (child.Element != Element)
			throw new ArgumentException( "Child must belong to the same element." );
		child.SetParent( this, renderLayerOffset );
		_children.Add( child );
		child.OnRemoved += ChildRemoved;
		return child;
	}

	private void SetParent( UserInterfaceComponentBase parent, uint renderLayerOffset = 1 ) {
		if (Parent is not null) 
			throw new Exception( "Cannot set parent, already has one." );
		Parent = parent;
		Transform.SetParent( parent.Transform, false );
		RenderLayer = parent.RenderLayer + renderLayerOffset;
	}

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnCharacter( @event ))
				return true;
		}
		return DoOnCharacter( @event );
	}

	internal bool OnKey( KeyboardEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnKey( @event ))
				return true;
		}
		return DoOnKey( @event );
	}
	internal bool OnMouseButton( MouseButtonEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseButton( @event ))
				return true;
		}
		return DoOnMouseButton( @event );
	}
	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseEnter( @event ))
				return true;
		}
		return DoOnMouseEnter( @event );
	}
	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseMoved( @event ))
				return true;
		}
		return DoOnMouseMoved( @event );
	}
	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		if (!Visible)
			return false;
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseWheelScrolled( @event ))
				return true;
		}
		return DoOnMouseWheelScrolled( @event );
	}


	internal void Update( double time, double deltaTime ) {
		Placement.Update();
		if (_placementChanged) {
			OnPlacementChanged();
			_placementChanged = false;
		}
		foreach (UserInterfaceComponentBase child in _children)
			child.Update( time, deltaTime );
		OnUpdate( time, deltaTime );
	}

	internal void UiSpaceChanged( Vector2<double> newAspectVector ) {
		if (Parent is not null)
			return;
		PlacementBounds = new AABB<Vector2<double>>( -newAspectVector, newAspectVector );
		PlacementBoundsChanged?.Invoke();
	}

	protected virtual bool DoOnCharacter( KeyboardCharacterEvent @event ) => false;
	protected virtual bool DoOnKey( KeyboardEvent @event ) => false;
	protected virtual bool DoOnMouseButton( MouseButtonEvent @event ) => false;
	protected virtual bool DoOnMouseEnter( MouseEnterEvent @event ) => false;
	protected virtual bool DoOnMouseMoved( MouseMoveEvent @event ) => false;
	protected virtual bool DoOnMouseWheelScrolled( MouseWheelEvent @event ) => false;

	protected abstract void OnPlacementChanged();

	internal void Hide() {
		Visible = false;
		foreach (UserInterfaceComponentBase child in _children)
			child.Hide();
		DoHide();
	}

	internal void Show() {
		Visible = true;
		foreach (UserInterfaceComponentBase child in _children)
			child.Show();
		DoShow();
	}

	internal protected abstract void DoHide();
	internal protected abstract void DoShow();

	protected abstract void OnUpdate( double time, double deltaTime );

	public void Remove() {
		if (Removed)
			return;
		Removed = true;
		InternalRemove();
		OnRemoved?.Invoke( this );
		Element.RemoveComponent( this );
		Parent = null;
		Transform.SetParent( null, false );
		foreach (UserInterfaceComponentBase child in _children) {
			child.OnRemoved -= ChildRemoved;
			child.Remove();
		}
		_children.Clear();
	}

	private void ChildRemoved( IRemovable removable ) {
		if (removable is not UserInterfaceComponentBase child)
			return;
		child.OnRemoved -= ChildRemoved;
		_children.Remove( child );
	}

	/// <summary>
	/// Used to remove and clean up scene instances and other resources.
	/// </summary>
	protected virtual void InternalRemove() { }
}