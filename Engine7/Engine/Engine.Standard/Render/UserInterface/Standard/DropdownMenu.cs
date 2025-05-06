using Engine.Module.Render.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class DropdownMenu<TDisplay, TValue> : UserInterfaceComponentBase where TDisplay : ValueDisplayComponentBase<TDisplay, TValue> where TValue : class {

	private TValue? _selectedValue;
	private readonly ObservableCollection<TValue> _dataSource;
	private bool _valueChanged = false;
	private bool _needsUpdate = false;

	private readonly InteractableButton _headerButton;
	private readonly ScrollableList<TDisplay, TValue> _selectionButtons;

	public event Action<TValue?>? OnValueSelected;

	public DropdownMenu( UserInterfaceElementBase element ) : base( element ) {
		_dataSource = [];
		_headerButton = AddChild( new InteractableButton( element, "Header" ) );
		_headerButton.OnClicked += OnHeaderClicked;
		_selectionButtons = AddChild( new ScrollableList<TDisplay, TValue>( element ) );
		var squaringScale = Placement.GetSquaringScale();
		_selectionButtons.Placement.Set( new( (0, -squaringScale.Y), 0, squaringScale ), Alignment.Center, Alignment.Negative );
		_selectionButtons.DisplayAdded += OnSelectionButtonAdded;
		_selectionButtons.DisplayRemoved += OnSelectionButtonRemoved;
		_dataSource.CollectionChanged += OnDataSourceChanged;
	}


	private void OnSelectionButtonAdded( TDisplay button ) {
		button.OnClicked += ValueSelected;
	}

	private void OnSelectionButtonRemoved( TDisplay button ) {
		button.OnClicked -= ValueSelected;
	}

	private void ValueSelected( TDisplay component, MouseButtonEvent mouseButtonEvent ) {
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

	public IList<TValue> DataSource => _dataSource;

	public TValue? SelectedValue {
		get => _selectedValue;
		set => SetSelectedValue( value );
	}

	private void SetSelectedValue( TValue? value ) {
		if (EqualityComparer<TValue>.Default.Equals( _selectedValue, value ))
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
