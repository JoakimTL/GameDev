using Engine.Logging;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Physics;
using Engine.Standard.Render.Input.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Engine.Standard.Render.UserInterface.Standard;

public abstract class InteractableUserInterfaceComponentBase<TSelf> : UserInterfaceComponentBase where TSelf : InteractableUserInterfaceComponentBase<TSelf> {

	public delegate void MouseMovementInteractionDelegate( TSelf component, MouseMoveEvent mouseMoveEvent );
	public delegate void MouseButtonInteractionDelegate( TSelf component, MouseButtonEvent mouseButtonEvent );
	public delegate void MouseWheelInteractionDelegate( TSelf component, MouseWheelEvent mouseWheelEvent );

	public event MouseMovementInteractionDelegate? OnMouseEntered;
	public event MouseMovementInteractionDelegate? OnMouseExited;
	public event MouseMovementInteractionDelegate? OnMouseMovedInside;
	public event MouseButtonInteractionDelegate? OnClicked;
	public event MouseButtonInteractionDelegate? OnPressed;
	public event MouseButtonInteractionDelegate? OnReleased;
	public event MouseWheelInteractionDelegate? OnScrolled;

	protected readonly Collider2Shape Collider;
	protected readonly Collision2Calculation<double> Collision;

	public bool ClickEnabled { get; protected set; }
	public bool ScrollEnabled { get; protected set; }
	public MouseButton? ClickButton { get; protected set; }
	public bool Hovering { get; protected set; }
	public bool HoveredPress { get; protected set; }

	public InteractableUserInterfaceComponentBase( UserInterfaceElementBase element ) : base( element ) {
		Collider = new Collider2Shape();
		Collider.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] );
		Collider.SetTransform( TransformInterface );
		Collision = new Collision2Calculation<double>( Collider, element.UserInterfaceServiceAccess.Get<MouseColliderProvider>().ColliderNDCA );
	}

	protected override bool DoOnMouseMoved( MouseMoveEvent @event ) {
		if (!Collision.Evaluate())
			this.LogWarning( "Collision calculation failed." );
		bool wasHovering = Hovering;
		Hovering = Collision.CollisionResult.IsColliding;
		if (wasHovering != Hovering) {
			if (Hovering) {
				OnMouseEntered?.Invoke( (TSelf) this, @event );
			} else {
				OnMouseExited?.Invoke( (TSelf) this, @event );
			}
		}
		if (Hovering)
			OnMouseMovedInside?.Invoke( (TSelf) this, @event );
		return false;
	}

	protected override bool DoOnMouseButton( MouseButtonEvent @event ) {
		if (!ClickEnabled)
			return false;
		if (ClickButton.HasValue && ClickButton.Value != @event.Button)
			return Hovering;
		if (@event.InputType == TactileInputType.Press && Hovering && (!ClickButton.HasValue || ClickButton.Value == @event.Button)) {
			OnPressed?.Invoke( (TSelf) this, @event );
			HoveredPress = true;
			return Hovering;
		}
		if (@event.InputType == TactileInputType.Release && HoveredPress && (!ClickButton.HasValue || ClickButton.Value == @event.Button)) {
			OnReleased?.Invoke( (TSelf) this, @event );
			if (Hovering)
				OnClicked?.Invoke( (TSelf) this, @event );
			HoveredPress = false;
			return Hovering;
		}
		return Hovering;
	}

	protected override bool DoOnMouseWheelScrolled( MouseWheelEvent @event ) {
		if (!ScrollEnabled)
			return false;
		if (Hovering) {
			OnScrolled?.Invoke( (TSelf) this, @event );
			return true;
		}
		return false;
	}
}

public sealed class InteractableButton : InteractableUserInterfaceComponentBase<InteractableButton> {
	public TexturedNineSlicedBackground Background { get; }
	public Label Label { get; }

	public InteractableButton( UserInterfaceElementBase element, string text ) : base( element ) {
		Background = AddChild( new TexturedNineSlicedBackground( element, element.UserInterfaceServiceAccess.Textures.Get( "test" ) ) );
		Label = AddChild( new Label( element ) {
			Text = text,
			FontName = DefaultFontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		} );

		ClickEnabled = true;
		OnMouseEntered += DefaultOnEnter;
		OnMouseExited += DefaultOnExit;
		OnPressed += DefaultOnPressed;
		OnReleased += DefaultOnReleased;
	}

	protected override void OnUpdate( double time, double deltaTime ) { }
	protected override void OnPlacementChanged() { }
	protected internal override void DoHide() { }
	protected internal override void DoShow() { }
	public void RemoveDefaultDecorations() {
		OnMouseEntered -= DefaultOnEnter;
		OnMouseExited -= DefaultOnExit;
		OnPressed -= DefaultOnPressed;
		OnReleased -= DefaultOnReleased;
	}

	public static string DefaultFontName => "calibrib";
	private static void DefaultOnExit( InteractableButton button, MouseMoveEvent @event ) => button.Background.Color = 1;
	private static void DefaultOnEnter( InteractableButton button, MouseMoveEvent @event ) => button.Background.Color = (.9, .9, .9, 1);
	private static void DefaultOnPressed( InteractableButton button, MouseButtonEvent @event ) => button.Background.Color = (.75, .75, .75, 1);
	private static void DefaultOnReleased( InteractableButton button, MouseButtonEvent @event ) => button.Background.Color = 1;
}

public sealed class VisualButton : UserInterfaceComponentBase {
	public TexturedNineSlicedBackground Background { get; }
	public Label Label { get; }

	public VisualButton( UserInterfaceElementBase element, string text ) : base( element ) {
		Background = AddChild( new TexturedNineSlicedBackground( element, element.UserInterfaceServiceAccess.Textures.Get( "test" ) ) );
		Label = AddChild( new Label( element ) {
			Text = text,
			FontName = DefaultFontName,
			TextScale = 0.5f,
			Color = (0, 0, 0, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		} );
	}

	protected override void OnUpdate( double time, double deltaTime ) { }
	protected override void OnPlacementChanged() { }
	protected internal override void DoHide() { }
	protected internal override void DoShow() { }

	public static string DefaultFontName => "calibrib";
}

public sealed class ListButton<TValue> : ValueDisplayComponentBase<ListButton<TValue>, TValue> where TValue : class {
	private readonly VisualButton _visuals;

	public ListButton( UserInterfaceElementBase element, TValue? value ) : base( element, value ) {
		_visuals = AddChild( new VisualButton( element, value?.ToString() ?? "No value" ) );

		ClickEnabled = true;
		OnMouseEntered += DefaultOnEnter;
		OnMouseExited += DefaultOnExit;
		OnPressed += DefaultOnPressed;
		OnReleased += DefaultOnReleased;
	}

	protected override void OnPlacementChanged() { }

	protected internal override void DoHide() { }

	protected internal override void DoShow() { }

	protected override void OnUpdate( double time, double deltaTime ) { }

	private static void DefaultOnExit( ListButton<TValue> button, MouseMoveEvent @event ) => button._visuals.Background.Color = 1;
	private static void DefaultOnEnter( ListButton<TValue> button, MouseMoveEvent @event ) => button._visuals.Background.Color = (.9, .9, .9, 1);
	private static void DefaultOnPressed( ListButton<TValue> button, MouseButtonEvent @event ) => button._visuals.Background.Color = (.75, .75, .75, 1);
	private static void DefaultOnReleased( ListButton<TValue> button, MouseButtonEvent @event ) => button._visuals.Background.Color = 1;
}

public sealed class DropdownMenu<T> : UserInterfaceComponentBase where T : class {

	private T? _selectedValue;
	private readonly ObservableCollection<T> _dataSource;
	private bool _valueChanged = false;
	private bool _needsUpdate = false;

	private readonly InteractableButton _headerButton;
	private readonly ScrollableList<ListButton<T>, T> _selectionButtons;

	public event Action<T?>? OnValueSelected;

	public DropdownMenu( UserInterfaceElementBase element ) : base( element ) {
		_dataSource = [];
		_headerButton = AddChild( new InteractableButton( element, "Header" ) );
		_headerButton.OnClicked += OnHeaderClicked;
		_selectionButtons = AddChild( new ScrollableList<ListButton<T>, T>( element ) );
		var squaringScale = Placement.GetSquaringScale();
		_selectionButtons.Placement.Set( new( (0, -squaringScale.Y), 0, squaringScale ), Alignment.Center, Alignment.Negative );
		_selectionButtons.DisplayAdded += OnSelectionButtonAdded;
		_selectionButtons.DisplayRemoved += OnSelectionButtonRemoved;
		_dataSource.CollectionChanged += OnDataSourceChanged;
	}


	private void OnSelectionButtonAdded( ListButton<T> button ) {
		button.OnClicked += ValueSelected;
	}

	private void OnSelectionButtonRemoved( ListButton<T> button ) {
		button.OnClicked -= ValueSelected;
	}

	private void ValueSelected( ListButton<T> component, MouseButtonEvent mouseButtonEvent ) {
		_selectedValue = component.Value;
		_headerButton.Label.Text = _selectedValue?.ToString() ?? "Select...";
		OnValueSelected?.Invoke( _selectedValue );
	}

	private void OnHeaderClicked( InteractableButton component, MouseButtonEvent mouseButtonEvent ) {
		_selectionButtons.ToggleDisplayed();
	}

	private void OnDataSourceChanged( object? sender, NotifyCollectionChangedEventArgs e ) {
		_needsUpdate = true;
	}

	public IList<T> DataSource => _dataSource;

	public T? SelectedValue {
		get => _selectedValue;
		set => SetSelectedValue( value );
	}

	private void SetSelectedValue( T? value ) {
		if (EqualityComparer<T>.Default.Equals( _selectedValue, value ))
			return;
		_selectedValue = value;
		_valueChanged = true;
	}

	protected override void OnPlacementChanged() {
		var squaringScale = Placement.GetSquaringScale();
		_selectionButtons.Placement.Set( new( (0, -squaringScale.Y), 0, squaringScale ), Alignment.Center, Alignment.Negative );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (_valueChanged) {
			_valueChanged = false;
			_headerButton.Label.Text = _selectedValue?.ToString() ?? "Select...";
		}
		if (_needsUpdate) {
			_needsUpdate = false;
			_selectionButtons.SetChoicesTo( _dataSource );
		}
	}

	protected internal override void DoHide() {

	}

	protected internal override void DoShow() {

	}
}

public sealed class ScrollableList<TDisplay, TValue> : InteractableUserInterfaceComponentBase<ScrollableList<TDisplay, TValue>> where TDisplay : ValueDisplayComponentBase<TDisplay, TValue> where TValue : class {
	private readonly Dictionary<TValue, TDisplay> _displayedValues;

	public event Action<TDisplay>? DisplayAdded;
	public event Action<TDisplay>? DisplayRemoved;

	public ScrollableList( UserInterfaceElementBase element ) : base( element ) {
		ScrollEnabled = true;
		_displayedValues = [];
		OnScrolled += DefaultOnScrolled;
	}

	public bool IsOpen { get; private set; } = false;

	public void Add( TValue value ) {
		if (_displayedValues.ContainsKey( value ))
			return;
		TDisplay display = typeof( TDisplay ).CreateInstance( [ Element, value ] ) as TDisplay ?? throw new InvalidOperationException( $"Unable to construct {typeof( TDisplay )}." );
		AddChild( display );
		if (!IsOpen)
			display.Hide();
		_displayedValues.Add( value, display );
		SetPlacements();
		DisplayAdded?.Invoke( display );
	}

	public void Remove( TValue value ) {
		if (!_displayedValues.Remove( value, out TDisplay? display ))
			return;
		display.Remove();
		SetPlacements();
		DisplayRemoved?.Invoke( display );
	}

	public void SetChoicesTo( IList<TValue> dataSource ) {
		foreach (var item in _displayedValues.Keys.ToArray()) {
			if (!dataSource.Contains( item )) {
				Remove( item );
			}
		}
		foreach (var item in dataSource) {
			if (!_displayedValues.ContainsKey( item )) {
				Add( item );
			}
		}
	}

	private void DefaultOnScrolled( ScrollableList<TDisplay, TValue> component, MouseWheelEvent mouseWheelEvent ) {

	}

	protected override void OnPlacementChanged() {
	}

	protected override void OnUpdate( double time, double deltaTime ) {
	}

	private void SetPlacements() {
		int i = 0;
		foreach (ValueDisplayComponentBase<TDisplay, TValue> display in _displayedValues.Values) {
			display.Placement.Set( new( (0, -i * 0.25 - 0.125), 0, (1, 0.125) ), Alignment.Center, Alignment.Positive );
			i++;
		}
	}

	public void ToggleDisplayed() {
		if (IsOpen) {
			Hide();
		} else {
			Show();
		}
	}

	protected internal override void DoHide() {
		IsOpen = false;
	}

	protected internal override void DoShow() {
		IsOpen = true;
	}

}

public abstract class ValueDisplayComponentBase<TSelf, TValue>( UserInterfaceElementBase element, TValue? value ) : InteractableUserInterfaceComponentBase<TSelf>( element ) where TSelf : ValueDisplayComponentBase<TSelf, TValue> where TValue : class {
	public TValue? Value { get; } = value;
}