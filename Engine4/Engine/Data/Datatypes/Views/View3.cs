using System.Numerics;

namespace Engine.Data.Datatypes.Views;

public class View3 : MatrixProviderBase {

	private Vector3 _translation;
	private Quaternion _rotation;

	public View3() {
		this._translation = new Vector3();
		this._rotation = Quaternion.Identity;
		SetChanged();
	}

	public Vector3 Translation {
		get => this._translation;
		set {
			this._translation = value;
			SetChanged();
		}
	}

	public Quaternion Rotation {
		get => this._rotation;
		set {
			this._rotation = value;
			SetChanged();
		}
	}

	protected override void MatrixAccessed() => this.Matrix = Matrix4x4.CreateTranslation( -this._translation ) * Matrix4x4.CreateFromQuaternion( Quaternion.Conjugate( this._rotation ) );

}
