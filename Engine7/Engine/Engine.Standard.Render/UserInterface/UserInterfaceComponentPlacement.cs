using Engine.Transforms.Models;

namespace Engine.Standard.Render.UserInterface;

public sealed class UserInterfaceComponentPlacement {

	private readonly UserInterfaceComponentBase _component;

	public TransformData<Vector2<double>, double, Vector2<double>> Transform { get; private set; }
	public Alignment HorizontalAlignment { get; private set; }
	public Alignment VerticalAlignment { get; private set; }
	public bool HorizontalScaling { get; private set; }
	public bool VerticalScaling { get; private set; }
	public bool InvertScalingOnTranslation { get; private set; }
	public bool InvertScalingOnScaling { get; private set; }
	public bool Changed { get; private set; }

	public UserInterfaceComponentPlacement( UserInterfaceComponentBase component ) {
		component.PlacementBoundsChanged += OnComponentPlacementBoundsChanged;
		this._component = component;
		this.Transform = new( 0, 0, 1 );
		this.HorizontalAlignment = 0;
		this.VerticalAlignment = 0;
		this.HorizontalScaling = false;
		this.VerticalScaling = false;
		InvertScalingOnTranslation = false;
		InvertScalingOnScaling = false;
		this.Changed = true;
	}

	private void OnComponentPlacementBoundsChanged() => this.Changed = true;

	public void SetTransform( TransformData<Vector2<double>, double, Vector2<double>> transform ) {
		this.Transform = transform;
		this.Changed = true;
	}

	public void SetHorizontalAlignment( Alignment horizontalAlignment ) {
		this.HorizontalAlignment = horizontalAlignment;
		this.Changed = true;
	}

	public void SetVerticalAlignment( Alignment verticalAlignment ) {
		this.VerticalAlignment = verticalAlignment;
		this.Changed = true;
	}

	public void SetHorizontalScaling( bool horizontalScaling ) {
		this.HorizontalScaling = horizontalScaling;
		this.Changed = true;
	}

	public void SetVerticalScaling( bool verticalScaling ) {
		this.VerticalScaling = verticalScaling;
		this.Changed = true;
	}

	public void Set( TransformData<Vector2<double>, double, Vector2<double>> transform, Alignment horizontalAlignment, Alignment verticalAlignment, bool horizontalScaling, bool verticalScaling ) {
		this.Transform = transform;
		this.HorizontalAlignment = horizontalAlignment;
		this.VerticalAlignment = verticalAlignment;
		this.HorizontalScaling = horizontalScaling;
		this.VerticalScaling = verticalScaling;
		this.Changed = true;
	}

	public void Set( TransformData<Vector2<double>, double, Vector2<double>> transform, Alignment horizontalAlignment, Alignment verticalAlignment ) {
		this.Transform = transform;
		this.HorizontalAlignment = horizontalAlignment;
		this.VerticalAlignment = verticalAlignment;
		this.Changed = true;
	}

	/// <returns>A vector which scaling returns this component to a square shape, however it does not remove all scaling effects.</returns>
	public Vector2<double> GetSquaringScale() {
		return this.Transform.Scale.X == 0 || this.Transform.Scale.Y == 0
			? Vector2<double>.One
			: this.Transform.Scale.X > this.Transform.Scale.Y ? (1, this.Transform.Scale.X / this.Transform.Scale.Y) : (this.Transform.Scale.Y / this.Transform.Scale.X, 1);
	}

	public void Update() {
		if (!this.Changed)
			return;
		this.Changed = false;
		Vector2<double> boundsCenter = this._component.PlacementBounds.GetCenter();
		Vector2<double> boundsLengths = this._component.PlacementBounds.GetLengths();

		Vector2<double> placementLengths = boundsLengths * 0.5;
		Vector2<double> newTranslation = boundsCenter + placementLengths.MultiplyEntrywise( ((int) this.HorizontalAlignment, (int) this.VerticalAlignment) ) + this.Transform.Translation;

		Vector2<double> scaling = (HorizontalScaling ? placementLengths.X : 1, VerticalScaling ? placementLengths.Y : 1);
		Vector2<double> newScale = this.Transform.Scale.MultiplyEntrywise( scaling );

		TransformData<Vector2<double>, double, Vector2<double>> newTransform = new( newTranslation, this.Transform.Rotation, newScale );

		this._component.TransformInterface.SetData( newTransform );
	}
}
