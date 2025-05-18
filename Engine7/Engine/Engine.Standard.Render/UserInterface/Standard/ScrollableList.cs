using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface.Standard;

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
