using Engine.Data.Transforms;
using Engine.Modules.ECS;
using System.Numerics;

namespace Engine.Standard.ECS.Components;

public abstract class TransformComponentBase<TScalar, TTranslation, TRotation, TScale> : SerializableComponentBase, ITransformComponent<TScalar, TTranslation, TRotation, TScale>
	where TScalar :
		unmanaged, INumber<TScalar>
	where TTranslation :
		unmanaged
	where TRotation :
		unmanaged
	where TScale :
		unmanaged {

	public abstract TransformInterface<TScalar, TTranslation, TRotation, TScale> Transform { get; }

	protected abstract TransformData<TTranslation, TRotation, TScale> GetTransformData();
	protected abstract void SetFromTransformData( TransformData<TTranslation, TRotation, TScale> data );

	protected override bool InternalDeserialize( ReadOnlySpan<byte> data ) {
		unsafe {
			if (data.Length < sizeof( TransformData<TTranslation, TRotation, TScale> ))
				return false;
			fixed (byte* ptr = data)
				SetFromTransformData( *(TransformData<TTranslation, TRotation, TScale>*) ptr );
		}
		return true;
	}

	protected override bool InternalSerialize( Span<byte> data, out uint writtenBytes ) {
		unsafe {
			uint sizeBytes = (uint) sizeof( TransformData<TTranslation, TRotation, TScale> );
			if (data.Length < sizeBytes) {
				writtenBytes = 0;
				return false;
			}
			fixed (byte* ptr = data)
				*(TransformData<TTranslation, TRotation, TScale>*) ptr = GetTransformData();
			writtenBytes = sizeBytes;
			return true;
		}
	}
}
