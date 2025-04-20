using Engine.Logging;
using System.Numerics;

namespace Engine.Structures;

public sealed class OcTree<T, TScalar>( AABB<Vector3<TScalar>> bounds, uint layers, bool allowLeafOnMultipleBranches = true ) : IReadOnlyOcTree<T, TScalar>
	where T : IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {

	private readonly Branch _root = new( bounds, layers, allowLeafOnMultipleBranches );

	public int Count => _root.Count;

	public AABB<Vector3<TScalar>> MaxDepthBounds {
		get {
			Vector3<TScalar> span = _root.BranchBounds.Maxima - _root.BranchBounds.Minima;
			span /= TScalar.CreateSaturating( 2 << (int) _root.Level );
			return AABB.Create( [ -span, span ] );
		}
	}

	/// <param name="level">Deepest level is 0, and goes higher from there.</param>
	public IReadOnlyList<AABB<Vector3<TScalar>>> GetBoundsAtLevel( uint level = 0 ) {
		List<AABB<Vector3<TScalar>>> bounds = [];
		_root.GetBoundsAtLevel( bounds, level );
		return bounds;
	}

	public IReadOnlyList<IReadOnlyCollection<T>> GetContentsAtLevel( uint level = 0 ) {
		List<IReadOnlyCollection<T>> contents = [];
		List<Branch> branches = [];
		_root.GetBranchesAtLevel( branches, level );
		foreach (Branch branch in branches)
			contents.Add( branch.Contents );
		return contents;
	}

	public IReadOnlyList<T> Get( AABB<Vector3<TScalar>> bounds, bool requireLeafIntersection = true ) {
		List<T> output = [];
		_root.Get( bounds, output, requireLeafIntersection );
		return output;
	}

	public int Get( List<T> outputList, AABB<Vector3<TScalar>> bounds, bool requireLeafIntersection = true ) {
		lock (outputList) {
			int preAdded = outputList.Count;
			_root.Get( bounds, outputList, requireLeafIntersection );
			return outputList.Count - preAdded;
		}
	}

	public IReadOnlyList<IReadOnlyBranch<T, TScalar>> GetBranches( AABB<Vector3<TScalar>> bounds ) {
		List<IReadOnlyBranch<T, TScalar>> output = [];
		_root.GetBranches( bounds, output );
		return output;
	}

	public IReadOnlyList<IReadOnlyBranch<T, TScalar>> GetBranches() {
		List<IReadOnlyBranch<T, TScalar>> output = [];
		_root.GetBranches( output );
		return output;
	}

	public void Add( T item ) => _root.Add( item );

	public void Remove( T item ) => _root.Remove( item );

	private sealed class Branch : IReadOnlyBranch<T, TScalar> {

		public AABB<Vector3<TScalar>> BranchBounds { get; }
		private AABB<Vector3<TScalar>> _actualBounds;
		public AABB<Vector3<TScalar>> ActualBounds => GetActualBounds();

		public uint Level { get; }

		private readonly Branch[]? _subBranches;
		private readonly HashSet<T> _contents;
		private readonly bool _allowLeafOnMultipleBranches;
		private bool _actualBoundsNeedUpdate;

		public IReadOnlyCollection<T> Contents => _contents;

		public int Count => _contents?.Count ?? this._subBranches?.Sum( p => p.Count ) ?? 0;

		private event Action? ActualBoundsChanged;

		public Branch( AABB<Vector3<TScalar>> bounds, uint level, bool allowLeafOnMultipleBranches ) {
			this.BranchBounds = bounds;
			_actualBounds = bounds;
			this.Level = level;
			this._allowLeafOnMultipleBranches = allowLeafOnMultipleBranches;
			this._contents = [];
			if (level == 0) {
				return;
			}
			this._subBranches = new Branch[ 8 ];
			int index = 0;
			Vector3<TScalar> halfSpan = bounds.GetCenter() - bounds.Minima;
			for (int x = 0; x <= 1; x++) {
				for (int y = 0; y <= 1; y++) {
					for (int z = 0; z <= 1; z++) {
						Vector3<TScalar> walk = new Vector3<int>( x, y, z ).CastSaturating<int, TScalar>();
						AABB<Vector3<TScalar>> childDomain = new( bounds.Minima + halfSpan.MultiplyEntrywise( walk ), bounds.GetCenter() + halfSpan.MultiplyEntrywise( walk ) );
						Branch branch = new( childDomain, level - 1, allowLeafOnMultipleBranches );
						this._subBranches[ index++ ] = branch;
						branch.ActualBoundsChanged += OnActualBoundsChanged;
					}
				}
			}
		}

		public void GetBoundsAtLevel( List<AABB<Vector3<TScalar>>> bounds, uint level ) {
			if (Level == level) {
				bounds.Add( BranchBounds );
				return;
			}
			if (Level == 0) {
				this.LogLine( $"Concluding search at level 0, when intended search level was {level}" );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				_subBranches[ i ].GetBoundsAtLevel( bounds, level );
		}

		public void GetBranchesAtLevel( List<Branch> branches, uint level ) {
			if (Level == level) {
				branches.Add( this );
				return;
			}
			if (Level == 0) {
				this.LogLine( $"Concluding search at level 0, when intended search level was {level}" );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				_subBranches[ i ].GetBranchesAtLevel( branches, level );
		}

		public void Get( AABB<Vector3<TScalar>> bounds, List<T> output, bool checkLeafIntersection ) {
			if (Level == 0) {
				if (checkLeafIntersection) {
					output.AddRange( _contents.Where( p => p.Bounds.Intersects( bounds ) ) );
				} else {
					output.AddRange( _contents );
				}
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( bounds ))
					_subBranches[ i ].Get( bounds, output, checkLeafIntersection );
		}

		public void GetBranches( AABB<Vector3<TScalar>> bounds, List<IReadOnlyBranch<T, TScalar>> output ) {
			if (Level == 0) {
				output.Add( this );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( bounds ))
					_subBranches[ i ].GetBranches( bounds, output );
		}

		public void GetBranches( List<IReadOnlyBranch<T, TScalar>> output ) {
			if (Level == 0) {
				output.Add( this );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				_subBranches[ i ].GetBranches( output );
		}

		public bool Add( T item ) {
			if (Level == 0) {
				_contents.Add( item );
				SetActualBounds( _actualBounds.GetLargestBounds( item.Bounds ) );
				return true;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( item.Bounds ))
					if (_subBranches[ i ].Add( item ) && !_allowLeafOnMultipleBranches)
						return true;
			return false;
		}

		private void SetActualBounds( AABB<Vector3<TScalar>> newBounds ) {
			if (_actualBounds == newBounds)
				return;
			_actualBounds = newBounds;
			ActualBoundsChanged?.Invoke();
		}

		public void Remove( T item ) {
			if (Level == 0) {
				_contents.Remove( item );
				_actualBoundsNeedUpdate = true;
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( item.Bounds ))
					_subBranches[ i ].Remove( item );
		}

		private void OnActualBoundsChanged() {
			_actualBoundsNeedUpdate = true;
			ActualBoundsChanged?.Invoke();
		}

		private AABB<Vector3<TScalar>> GetActualBounds() {
			if (!_actualBoundsNeedUpdate)
				return _actualBounds;
			_actualBoundsNeedUpdate = false;
			AABB<Vector3<TScalar>> newBounds = BranchBounds;

			if (Level == 0) {
				foreach (T item in _contents)
					newBounds = newBounds.GetLargestBounds( item.Bounds );
				SetActualBounds( newBounds );
				return _actualBounds;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			foreach (Branch branch in _subBranches)
				newBounds = newBounds.GetLargestBounds( branch.ActualBounds );
			SetActualBounds( newBounds );
			return _actualBounds;
		}

		public override string ToString() => $"Level {Level}, {BranchBounds}, {Count} items";

	}
}
