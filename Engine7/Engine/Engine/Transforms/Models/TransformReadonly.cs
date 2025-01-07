using System.Numerics;

namespace Engine.Transforms.Models;

public sealed class TransformReadonly<TScalar, TTranslation, TRotation, TScale>( TransformBase<TScalar, TTranslation, TRotation, TScale> transform ) : TransformAccessBase<TScalar, TTranslation, TRotation, TScale>(transform)
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {

	public TransformReadonly<TScalar, TTranslation, TRotation, TScale>? Parent => this._transform.Parent?.Readonly;
	public TTranslation Translation => this._transform.Translation;
	public TTranslation GlobalTranslation => this._transform.GlobalTranslation;
	public TRotation Rotation => this._transform.Rotation;
	public TRotation GlobalRotation => this._transform.GlobalRotation;
	public TScale Scale => this._transform.Scale;
	public TScale GlobalScale => this._transform.GlobalScale;
}
