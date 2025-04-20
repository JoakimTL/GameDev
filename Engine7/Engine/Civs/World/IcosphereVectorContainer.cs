using Engine;
using System.Runtime.InteropServices;

namespace Civs.World;

public sealed unsafe class IcosphereVectorContainer : DisposableIdentifiable {
	private readonly uint _vectorCount;
	private readonly Vector3<float>* _vectors;

	public IcosphereVectorContainer( IReadOnlyList<Vector3<float>> vectors ) {
		_vectorCount = (uint) vectors.Count;
		_vectors = (Vector3<float>*) NativeMemory.Alloc( (nuint) (_vectorCount * sizeof( Vector3<float> )) );
		for (int i = 0; i < _vectorCount; i++)
			_vectors[ i ] = vectors[ i ];
	}

	public Vector3<float> GetVector( uint index ) {
#if DEBUG
		if (index >= _vectorCount)
			throw new ArgumentOutOfRangeException( nameof( index ), "Index is out of range." );
#endif
		return _vectors[ index ];
	}

	protected override bool InternalDispose() {
		NativeMemory.Free( _vectors );
		return true;
	}
}