using Engine.Module.Render.Input;
using Engine.Transforms;
using Engine.Transforms.Models;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceComponentBase : DisposableIdentifiable {
	protected UserInterfaceElementBase Element { get; }

	private readonly List<UserInterfaceComponentBase> _children;
	public UserInterfaceComponentBase? Parent { get; }

	public UserInterfaceComponentPlacement Placement { get; }
	protected Transform2<double> Transform { get; }
	public TransformInterface<double, Vector2<double>, double, Vector2<double>> TransformInterface { get; }

	public uint RenderLayer { get; }

	public AABB<Vector2<double>> PlacementBounds { get; private set; }
	public event Action? PlacementBoundsChanged;

	private bool _placementChanged = false;

	public UserInterfaceComponentBase( UserInterfaceElementBase element ) {
		this.Element = element;
		Transform = new();
		Transform.Nickname = GetType().Name;
		TransformInterface = Transform.Interface;
		Transform.OnMatrixChanged += OnMatrixChanged;
		PlacementBounds = new AABB<Vector2<double>>( -1, 1 );
		Placement = new( this );
		_children = [];
		Parent = null;
		RenderLayer = element.BaseLayer;
	}

	public UserInterfaceComponentBase( UserInterfaceComponentBase parent, uint renderLayerOffset = 1 ) : this( parent.Element ) {
		Parent = parent;
		parent._children.Add( this );
		Transform.SetParent( parent.Transform, false );
		RenderLayer = parent.RenderLayer + renderLayerOffset;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => _placementChanged = true;

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnCharacter( @event ))
				return true;
		}
		return DoOnCharacter( @event );
	}
	internal bool OnKey( KeyboardEvent @event ) {
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnKey( @event ))
				return true;
		}
		return DoOnKey( @event );
	}
	internal bool OnMouseButton( MouseButtonEvent @event ) {
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseButton( @event ))
				return true;
		}
		return DoOnMouseButton( @event );
	}
	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseEnter( @event ))
				return true;
		}
		return DoOnMouseEnter( @event );
	}
	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		foreach (UserInterfaceComponentBase child in _children) {
			if (child.OnMouseMoved( @event ))
				return true;
		}
		return DoOnMouseMoved( @event );
	}
	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
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
		foreach (UserInterfaceComponentBase child in _children)
			child.Hide();
		DoHide();
	}

	internal void Show() {
		foreach (UserInterfaceComponentBase child in _children)
			child.Show();
		DoShow();
	}

	internal protected abstract void DoHide();
	internal protected abstract void DoShow();

	protected abstract void OnUpdate( double time, double deltaTime );
}