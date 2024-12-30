using Engine.Transforms;
using System.Numerics;

namespace Engine.Physics;

/// <summary>
/// Used when calculating a GJK collision.
/// </summary>
public abstract class ConvexShapeBase<TVector, TScalar>
	where TVector : unmanaged, ITransformableVector<Matrix4x4<TScalar>, TVector>, IInnerProduct<TVector, TScalar>
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	private readonly List<TVector> _baseVertices = [];
	private IMatrixProvider<TScalar>? _transform;
	private readonly List<TVector> _transformedVertices = [];
	private bool _hasChanged;

	public ConvexShapeBase( ReadOnlySpan<TVector> vertices ) {
		_baseVertices.AddRange( vertices );
		_transformedVertices.AddRange( _baseVertices );
	}

	public ConvexShapeBase( IEnumerable<TVector> vertices ) {
		_baseVertices.AddRange( vertices );
		_transformedVertices.AddRange( _baseVertices );
	}

	protected void SetBaseVertices( ReadOnlySpan<TVector> vertices ) {
		_baseVertices.Clear();
		_baseVertices.AddRange( vertices );
		_hasChanged = true;
	}

	protected void SetBaseVertices( IEnumerable<TVector> vertices ) {
		_baseVertices.Clear();
		_baseVertices.AddRange( vertices );
		_hasChanged = true;
	}

	protected void SetTransform( IMatrixProvider<TScalar>? transform ) {
		if (_transform is not null)
			_transform.OnMatrixChanged -= OnMatrixChanged;
		_transform = transform;
		_hasChanged = true;
		if (_transform is not null)
			_transform.OnMatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider<TScalar> provider ) => _hasChanged = false;

	public bool HasTransformer => _transform is not null;

	public IReadOnlyList<TVector> GetVertices() {
		if (!_hasChanged)
			return _transformedVertices;
		_hasChanged = false;
		_transformedVertices.Clear();
		if (_transform is null) {
			_transformedVertices.AddRange( _baseVertices );
			return _transformedVertices;
		}
		foreach (TVector vector in _baseVertices) {
			TVector? transformed = vector.TransformWorld( _transform.Matrix );
			if (transformed.HasValue)
				_transformedVertices.Add( transformed.Value );
		}
		return _transformedVertices;
	}

	public (int index, TVector vector) GetFurthestInDirection( TVector direction ) {
		IReadOnlyList<TVector> vertices = GetVertices();
		int index = 0;
		TVector furthest = vertices[ 0 ];
		TScalar maxDot = direction.Dot( furthest );
		for (int i = 1; i < vertices.Count; i++) {
			TScalar dot = direction.Dot( vertices[ i ] );
			if (dot > maxDot) {
				maxDot = dot;
				furthest = vertices[ i ];
				index = i;
			}
		}
		return (index, furthest);
	}
}
