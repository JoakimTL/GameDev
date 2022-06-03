using System.Numerics;

namespace Engine.Data.Datatypes.Views;
public class View2 : MatrixProviderBase {

	private Vector2 _translation;
	private float _rotation;

	public View2() {
		this._translation = new Vector2();
		this._rotation = 0;
		SetChanged();
	}

	public Vector2 Translation {
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

	protected override void MatrixAccessed() => this.Matrix = Matrix4x4.CreateTranslation( new Vector3( -this._translation, 0 ) ) * Matrix4x4.CreateRotationZ( -this._rotation );

}
