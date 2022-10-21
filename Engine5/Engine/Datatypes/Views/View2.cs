using System.Numerics;

namespace Engine.Datatypes.Views;
public class View2 : MatrixProviderBase {

	private Vector2 _translation;
	private float _rotation;

	public View2() {
		_translation = new Vector2();
		_rotation = 0;
		SetChanged();
	}

	public Vector2 Translation {
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

	protected override void MatrixAccessed() => Matrix = Matrix4x4.CreateTranslation( new Vector3( -_translation, 0 ) ) * Matrix4x4.CreateRotationZ( -_rotation );

}
