using Engine.Module.Render.Input;
using Engine.Transforms;
using Engine.Transforms.Models;

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

	public UserInterfaceComponentBase( UserInterfaceElementBase element ) {
		this.Element = element;
		this.Transform = new();
		this.Transform.Nickname = GetType().Name;
		this.TransformInterface = this.Transform.Interface;
		this.Transform.OnMatrixChanged += OnMatrixChanged;
		this.PlacementBounds = new AABB<Vector2<double>>( -1, 1 );
		this.Placement = new( this );
		this._children = [];
		this.Parent = null;
		this.RenderLayer = element.BaseLayer;
		this.Element.AddComponent( this );
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => this._placementChanged = true;

	protected T AddChild<T>( T child, uint renderLayerOffset = 1 ) where T : UserInterfaceComponentBase {
		if (child.Element != this.Element)
			throw new ArgumentException( "Child must belong to the same element." );
		child.SetParent( this, renderLayerOffset );
		this._children.Add( child );
		child.OnRemoved += ChildRemoved;
		return child;
	}

	private void SetParent( UserInterfaceComponentBase parent, uint renderLayerOffset = 1 ) {
		if (this.Parent is not null)
			throw new Exception( "Cannot set parent, already has one." );
		this.Parent = parent;
		this.Transform.SetParent( parent.Transform, false );
		SetRenderLayer( parent.RenderLayer + renderLayerOffset );
	}

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnCharacter( @event ))
				return true;
		}
		return DoOnCharacter( @event );
	}

	internal bool OnKey( KeyboardEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnKey( @event ))
				return true;
		}
		return DoOnKey( @event );
	}
	internal bool OnMouseButton( MouseButtonEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnMouseButton( @event ))
				return true;
		}
		return DoOnMouseButton( @event );
	}
	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnMouseEnter( @event ))
				return true;
		}
		return DoOnMouseEnter( @event );
	}
	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnMouseMoved( @event ))
				return true;
		}
		return DoOnMouseMoved( @event );
	}
	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		if (!this.Visible)
			return false;
		foreach (UserInterfaceComponentBase child in this._children) {
			if (child.OnMouseWheelScrolled( @event ))
				return true;
		}
		return DoOnMouseWheelScrolled( @event );
	}


	internal void Update( double time, double deltaTime ) {
		this.Placement.Update();
		if (this._placementChanged) {
			OnPlacementChanged();
			this._placementChanged = false;
		}
		foreach (UserInterfaceComponentBase child in this._children)
			child.Update( time, deltaTime );
		OnUpdate( time, deltaTime );
	}

	internal void UiSpaceChanged( Vector2<double> newAspectVector ) {
		if (this.Parent is not null)
			return;
		this.PlacementBounds = new AABB<Vector2<double>>( -newAspectVector, newAspectVector );
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
		this.Visible = false;
		foreach (UserInterfaceComponentBase child in this._children)
			child.Hide();
		DoHide();
	}

	internal void Show() {
		this.Visible = true;
		foreach (UserInterfaceComponentBase child in this._children)
			child.Show();
		DoShow();
	}

	internal protected abstract void DoHide();
	internal protected abstract void DoShow();

	protected abstract void OnUpdate( double time, double deltaTime );

	public void Remove() {
		if (this.Removed)
			return;
		this.Removed = true;
		InternalRemove();
		OnRemoved?.Invoke( this );
		this.Element.RemoveComponent( this );
		this.Parent = null;
		this.Transform.SetParent( null, false );
		foreach (UserInterfaceComponentBase child in this._children) {
			child.OnRemoved -= ChildRemoved;
			child.Remove();
		}
		this._children.Clear();
	}

	private void ChildRemoved( IRemovable removable ) {
		if (removable is not UserInterfaceComponentBase child)
			return;
		child.OnRemoved -= ChildRemoved;
		this._children.Remove( child );
	}

	public void SetRenderLayer( uint layer ) {
		uint previousLayer = this.RenderLayer;
		if (this.RenderLayer == layer)
			return;
		this.RenderLayer = layer;
		RenderLayerChanged();
		foreach (UserInterfaceComponentBase child in this._children) {
			uint delta = child.RenderLayer - previousLayer;
			child.SetRenderLayer( layer + delta );
		}
	}

	/// <summary>
	/// Used to remove and clean up scene instances and other resources.
	/// </summary>
	protected virtual void InternalRemove() { }

	protected virtual void RenderLayerChanged() { }
}