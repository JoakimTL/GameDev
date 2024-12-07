namespace Engine.Transforms.Camera;

public class View3 : MatrixProviderBase<float> {

	private Vector3<float> _translation;
	private Rotor3<float> _rotation;

	public View3() {
		this._translation = Vector3<float>.Zero;
		this._rotation = Rotor3<float>.MultiplicativeIdentity;
		SetChanged();
	}

	public Vector3<float> Translation {
		get => this._translation;
		set {
			this._translation = value;
			SetChanged();
		}
	}

	public Rotor3<float> Rotation {
		get => this._rotation;
		set {
			this._rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => this.Matrix = Engine.Matrix.Create4x4.Translation( -this._translation ) * Engine.Matrix.Create4x4.RotationFromRotor( this._rotation.Conjugate() );

}
