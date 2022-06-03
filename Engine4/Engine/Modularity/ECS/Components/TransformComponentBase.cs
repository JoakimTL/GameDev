using Engine.Data;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Transforms;

namespace Engine.Modularity.ECS.Components;

[OverrideType( typeof( TransformComponentBase<,,> ) )]
public abstract class TransformComponentBase<T, R, S> : Component
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {
	protected readonly TransformBase<T, R, S> _transform;
	public TransformInterface<T, R, S> Transform => this._transform.Interface;

	protected TransformComponentBase( TransformBase<T, R, S> transform ) {
		this._transform = transform;
		this._transform.MatrixChanged += OnChanged;
	}

	private void OnChanged( IMatrixProvider obj ) => TriggerChanged();

	protected override byte[]? GetSerializedData() {
		unsafe {
			TransformData<T, R, S> transformData = this._transform.Data;
			byte[] data = new byte[ sizeof( TransformData<T, R, S> ) ];
			fixed ( byte* dataPtr = data )
				( (TransformData<T, R, S>*) dataPtr )[ 0 ] = transformData;
			return data;
		}
	}

	public override void SetFromSerializedData( byte[] data ) {
		TransformData<T, R, S>? transformData = DataUtils.ToUnmanaged<TransformData<T, R, S>>( data );
		if ( !transformData.HasValue ) {
			this.LogError( "Corrupt data." );
			return;
		}
		this._transform.SetData( transformData.Value );
	}
}
