using Engine.Data.Transforms;
using Engine.Modules.ECS;

namespace Engine.Standard.ECS.Components;

public abstract class TransformComponentBase<T, R, S> : SerializableComponentBase
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	protected abstract TransformData<T, R, S> GetTransformData();
	protected abstract void SetFromTransformData( TransformData<T, R, S> data );

	protected override bool InternalDeserialize( ReadOnlySpan<byte> data ) {
		unsafe {
			if (data.Length < sizeof( TransformData<T, R, S> ))
				return false;
			fixed (byte* ptr = data)
				SetFromTransformData( *(TransformData<T, R, S>*) ptr );
		}
		return true;
	}

	protected override bool InternalSerialize( Span<byte> data, out uint writtenBytes ) {
		unsafe {
			uint sizeBytes = (uint) sizeof( TransformData<T, R, S> );
			if (data.Length < sizeBytes) {
				writtenBytes = 0;
				return false;
			}
			fixed (byte* ptr = data)
				*(TransformData<T, R, S>*) ptr = GetTransformData();
			writtenBytes = sizeBytes;
			return true;
		}
	}
}