namespace Engine.Transforms.Camera;

public class View2 : MatrixProviderBase<float> {

	private Vector2<float> _translation;
	private float _rotation;

	public View2() {
		this._translation = Vector2<float>.Zero;
		this._rotation = 0;
		SetChanged();
	}

	public Vector2<float> Translation {
		get => this._translation;
		set {
			this._translation = value;
			SetChanged();
		}
	}

	public float Rotation {
		get => this._rotation;
		set {
			this._rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => this.Matrix = Engine.Matrix.Create4x4.Translation( new Vector3<float>( -this._translation.X, -this._translation.Y, 0 ) ) * Engine.Matrix.Create4x4.RotationZ( -this._rotation );

}
