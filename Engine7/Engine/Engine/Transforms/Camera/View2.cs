namespace Engine.Transforms.Camera;

public class View2 : MatrixProviderBase<float> {

	private Vector2<float> _translation;
	private float _rotation;

	public View2() {
		_translation = Vector2<float>.Zero;
		_rotation = 0;
		SetChanged();
	}

	public Vector2<float> Translation {
		get => _translation;
		set {
			_translation = value;
			SetChanged();
		}
	}

	public float Rotation {
		get => _rotation;
		set {
			_rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => Matrix = Engine.Matrix.Create4x4.Translation( new Vector3<float>( -_translation.X, -_translation.Y, 0 ) ) * Engine.Matrix.Create4x4.RotationZ( -_rotation );

}
