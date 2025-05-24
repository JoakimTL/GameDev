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
		this._dataSource = [];
		this._headerButton = AddChild( new InteractableButton( element, "Header" ) );
		this._headerButton.OnClicked += OnHeaderClicked;
		this._selectionButtons = AddChild( new ScrollableList<TDisplay, TValue>( element ) );
		Vector2<double> squaringScale = this.Placement.GetSquaringScale();
		this._selectionButtons.Placement.Set( new( (0, -squaringScale.Y), 0, squaringScale ), Alignment.Center, Alignment.Negative );
		this._selectionButtons.DisplayAdded += OnSelectionButtonAdded;
		this._selectionButtons.DisplayRemoved += OnSelectionButtonRemoved;
		this._dataSource.CollectionChanged += OnDataSourceChanged;
	}


	private void OnSelectionButtonAdded( TDisplay button ) {
		button.OnClicked += ValueSelected;
	}

	private void OnSelectionButtonRemoved( TDisplay button ) {
		button.OnClicked -= ValueSelected;
	}

	private void ValueSelected( TDisplay component, MouseButtonEvent mouseButtonEvent ) {
		this._selectedValue = component.Value;
		this._headerButton.Label.Text = this._selectedValue?.ToString() ?? "Select...";
		OnValueSelected?.Invoke( this._selectedValue );
	}

	private void OnHeaderClicked( InteractableButton component, MouseButtonEvent mouseButtonEvent ) {
		this._selectionButtons.ToggleDisplayed();
	}

	private void OnDataSourceChanged( object? sender, NotifyCollectionChangedEventArgs e ) {
		this._needsUpdate = true;
	}

	public IList<TValue> DataSource => this._dataSource;

	public TValue? SelectedValue {
		get => this._selectedValue;
		set => SetSelectedValue( value );
	}

	private void SetSelectedValue( TValue? value ) {
		if (EqualityComparer<TValue>.Default.Equals( this._selectedValue, value ))
			return;
		this._selectedValue = value;
		this._valueChanged = true;
	}

	protected override void OnPlacementChanged() {
		Vector2<double> squaringScale = this.Placement.GetSquaringScale();
		this._selectionButtons.Placement.Set( new( (0, -squaringScale.Y), 0, squaringScale ), Alignment.Center, Alignment.Negative );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (this._valueChanged) {
			this._valueChanged = false;
			this._headerButton.Label.Text = this._selectedValue?.ToString() ?? "Select...";
		}
		if (this._needsUpdate) {
			this._needsUpdate = false;
			this._selectionButtons.SetChoicesTo( this._dataSource );
		}
	}

	protected internal override void DoHide() {

	}

	protected internal override void DoShow() {

	}
}
