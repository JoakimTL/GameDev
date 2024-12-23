﻿using System.Numerics;

namespace Engine.Buffers;

public interface IHostClientSynchronizedBufferPair<THostBuffer, THostScalar, TClientBuffer, TClientScalar> : IDisposable
	where THostBuffer : ICopyableBuffer<THostScalar>, IObservableBuffer<THostScalar>, IVariableLengthBuffer<THostScalar>
	where TClientBuffer : IWritableBuffer<TClientScalar>, IWriteResizableBuffer<TClientScalar>
	where THostScalar : unmanaged, IBinaryInteger<THostScalar>, IUnsignedNumber<THostScalar>
	where TClientScalar : unmanaged, IBinaryInteger<TClientScalar>, IUnsignedNumber<TClientScalar> {
	void Synchronize();
}
