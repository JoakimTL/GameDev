using System.Numerics;

namespace Engine.Transforms.Models;

public sealed class TransformInterface<TScalar, TTranslation, TRotation, TScale>( TransformBase<TScalar, TTranslation, TRotation, TScale> transform ) : TransformAccessBase<TScalar, TTranslation, TRotation, TScale>(transform)
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {

	public TransformReadonly<TScalar, TTranslation, TRotation, TScale>? Parent 
		=> this._transform.Parent?.Readonly;

	public void SetData( TransformData<TTranslation, TRotation, TScale> data ) 
		=> this._transform.SetData( data );

	public TTranslation Translation {
		get => this._transform.Translation;
		set => this._transform.Translation = value;
	}

	public TRotation Rotation {
		get => this._transform.Rotation;
		set => this._transform.Rotation = value;
	}

	public TScale Scale {
		get => this._transform.Scale;
		set => this._transform.Scale = value;
	}

	public TTranslation GlobalTranslation => this._transform.GlobalTranslation;
	public TRotation GlobalRotation => this._transform.GlobalRotation;
	public TScale GlobalScale => this._transform.GlobalScale;
}
