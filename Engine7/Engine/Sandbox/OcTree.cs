using Engine.Logging;
using System.Collections.Generic;
using System.Numerics;

namespace Sandbox;

public interface IReadOnlyOcTree<T, TScalar>
	where T : IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {
	public IReadOnlyList<AABB<Vector3<TScalar>>> GetBoundsAtLevel( uint level = 0 );
	public IReadOnlyList<IReadOnlyCollection<T>> GetContentsAtLevel( uint level = 0 );
	public IReadOnlyList<T> GetAll( AABB<Vector3<TScalar>> bounds );
}

public interface IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {
	AABB<Vector3<TScalar>> Bounds { get; }
}

public sealed class OcTree<T, TScalar>( AABB<Vector3<TScalar>> bounds, uint layers, bool allowLeafOnMultipleBranches = true ) : IReadOnlyOcTree<T, TScalar>
	where T : IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {

	private readonly Branch _root = new( bounds, layers, allowLeafOnMultipleBranches );

	public int Count => _root.Count;

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

	public IReadOnlyList<T> GetAll( AABB<Vector3<TScalar>> bounds ) {
		List<T> output = [];
		_root.GetAll( bounds, output );
		return output;
	}

	public void Add( T item ) => _root.Add( item );

	public void Remove( T item ) => _root.Remove( item );

	private sealed class Branch {

		public AABB<Vector3<TScalar>> BranchBounds { get; }
		public uint Level { get; }

		private readonly Branch[]? _subBranches;
		private readonly HashSet<T> _contents;
		private readonly bool _allowLeafOnMultipleBranches;

		public IReadOnlyCollection<T> Contents => _contents;

		public int Count => _contents?.Count ?? this._subBranches?.Sum( p => p.Count ) ?? 0;

		public Branch( AABB<Vector3<TScalar>> bounds, uint level, bool allowLeafOnMultipleBranches ) {
			this.BranchBounds = bounds;
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
						this._subBranches[ index++ ] = new( childDomain, level - 1, allowLeafOnMultipleBranches );
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

		public void GetAll( AABB<Vector3<TScalar>> volume, List<T> output ) {
			if (Level == 0) {
				output.AddRange( _contents );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( volume ))
					_subBranches[ i ].GetAll( volume, output );
		}

		public bool Add( T item ) {
			if (Level == 0) {
				_contents.Add( item );
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

		public void Remove( T item ) {
			if (Level == 0) {
				_contents.Remove( item );
				return;
			}
			if (_subBranches is null)
				throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
			for (int i = 0; i < 8; i++)
				if (_subBranches[ i ].BranchBounds.Intersects( item.Bounds ))
					_subBranches[ i ].Remove( item );
		}

		public override string ToString() => $"Level {Level}, {BranchBounds}, {Count} items";

	}
}
