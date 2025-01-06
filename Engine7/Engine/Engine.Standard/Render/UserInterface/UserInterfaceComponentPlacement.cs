using Engine.Transforms.Models;

namespace Engine.Standard.Render.UserInterface;

public sealed class UserInterfaceComponentPlacement {
	private readonly UserInterfaceComponentBase _component;

	public TransformData<Vector2<double>, double, Vector2<double>> Transform { get; private set; }
	public Alignment HorizontalAlignment { get; private set; }
	public Alignment VerticalAlignment { get; private set; }
	public bool Changed { get; private set; }

	public UserInterfaceComponentPlacement( UserInterfaceComponentBase component ) {
		component.PlacementBoundsChanged += OnComponentPlacementBoundsChanged;
		this._component = component;
		Transform = new( 0, 0, 1 );
		HorizontalAlignment = 0;
		VerticalAlignment = 0;
		Changed = true;
	}

	private void OnComponentPlacementBoundsChanged() {
		Changed = true;
	}

	public void SetTransform( TransformData<Vector2<double>, double, Vector2<double>> transform ) {
		Transform = transform;
		Changed = true;
	}

	public void SetHorizontalAlignment( Alignment horizontalAlignment ) {
		HorizontalAlignment = horizontalAlignment;
		Changed = true;
	}

	public void SetVerticalAlignment( Alignment verticalAlignment ) {
		VerticalAlignment = verticalAlignment;
		Changed = true;
	}

	public bool Update() {
		if (!Changed)
			return false;
		Vector2<double> placementCenter = _component.PlacementBounds.GetCenter();
		Vector2<double> placementLengths = _component.PlacementBounds.GetLengths() * 0.5;
		Vector2<double> newTranslation = placementCenter + placementLengths.MultiplyEntrywise( ((int) HorizontalAlignment, (int) VerticalAlignment) ) + Transform.Translation;

		TransformData<Vector2<double>, double, Vector2<double>> newTransform = new( newTranslation, Transform.Rotation, Transform.Scale );

		_component.TransformInterface.SetData( newTransform );
		Changed = false;
		return true;
	}


}
