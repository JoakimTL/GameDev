namespace Engine.Transforms.Camera;

public class View3 : MatrixProviderBase<float> {

	private Vector3<float> _translation;
	private Rotor3<float> _rotation;

	public View3() {
		_translation = Vector3<float>.Zero;
		_rotation = Rotor3<float>.MultiplicativeIdentity;
		SetChanged();
	}

	public Vector3<float> Translation {
		get => _translation;
		set {
			_translation = value;
			SetChanged();
		}
	}

	public Rotor3<float> Rotation {
		get => _rotation;
		set {
			_rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => Matrix = Engine.Matrix.Create4x4.Translation( -_translation ) * Engine.Matrix.Create4x4.RotationFromRotor( _rotation.Conjugate() );

}
