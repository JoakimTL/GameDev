using System.Numerics;

namespace Engine.Datatypes.Views;

public class View3 : MatrixProviderBase {

	private Vector3 _translation;
	private Quaternion _rotation;

	public View3() {
		_translation = new Vector3();
		_rotation = Quaternion.Identity;
		SetChanged();
	}

	public Vector3 Translation {
		get => _translation;
		set {
			_translation = value;
			SetChanged();
		}
	}

	public Quaternion Rotation {
		get => _rotation;
		set {
			_rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => Matrix = Matrix4x4.CreateTranslation( -_translation ) * Matrix4x4.CreateFromQuaternion( Quaternion.Conjugate( _rotation ) );

}
