BoundedTileCluster.cs:
using Civlike.World.Geometry;
using Engine;

namespace Civlike.World.State;

public sealed class BoundedTileCluster : DisposableIdentifiable {
	public BoundedTileCluster( Globe globe, BoundedRenderCluster cluster, IReadOnlyList<Tile> tiles ) {
		this.Globe = globe;
		this.RenderCluster = cluster;
		this.Tiles = tiles;
		foreach (Tile tile in tiles)
			tile.StateChanged += OnStateChanged;
	}

	public event Action<BoundedTileCluster>? StateChanged;

	public Globe Globe { get; }
	public BoundedRenderCluster RenderCluster { get; }
	public IReadOnlyList<Tile> Tiles { get; }

	private void OnStateChanged( StateBase<Tile> tileState ) {
		StateChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		foreach (Tile tile in this.Tiles)
			tile.StateChanged -= OnStateChanged;
		return true;
	}
}
Globe.cs:
using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Globe : StateContainerBase<Globe> {

	internal Globe( Guid globeId, ReadOnlyGlobe readonlyGlobe, double radius ) {
		this.Id = globeId;
		this.Model = readonlyGlobe;
		this.Nodes = this.Model.Vertices
			.Select( f => new Node( this, f ) )
			.ToList()
			.AsReadOnly();
		this.Tiles = this.Model.Faces
			.Select( f => new Tile( this, f, f.Vertices.Select( v => this.Nodes[ v.Id ] ) ) )
			.ToList()
			.AsReadOnly();
		this.Clusters = this.Model.Clusters
			.Select( c => new BoundedTileCluster( this, c, c.Faces.Select( f => this.Tiles[ f.Id ] ).ToList().AsReadOnly() ) )
			.ToList()
			.AsReadOnly();

		HashSet<Node> neighbouringNodes = [];
		foreach (Node n in Nodes) {
			neighbouringNodes.Clear();
			foreach (ReadOnlyFace f in n.Vertex.ConnectedFaces) 
				foreach (Node neighbour in f.Vertices.Where( v => v.Id != n.Vertex.Id ).Select( v => Nodes[ v.Id ] ))
					neighbouringNodes.Add( neighbour );
			n.NeighbouringNodes = neighbouringNodes.ToList().AsReadOnly();
		}

		this.RadiusMeters = radius;
		this.TileArea = this.Area / this.Tiles.Count;
		this.ApproximateTileLength = 2 * this.RadiusMeters * double.Sin( double.Acos( 1 / ((1 + double.Sqrt( 5 )) / 2) ) / double.Pow( 2, readonlyGlobe.Subdivisions ) * 0.5 );
	}

	public Guid Id { get; }
	public ReadOnlyGlobe Model { get; }
	public IReadOnlyList<Node> Nodes { get; }
	public IReadOnlyList<Tile> Tiles { get; }
	public IReadOnlyList<BoundedTileCluster> Clusters { get; }
	public double RadiusMeters { get; }
	public double RadiusKm => this.RadiusMeters * 0.001;
	public double Area => 4 * double.Pi * this.RadiusMeters * this.RadiusMeters;
	public double TileArea { get; }
	public double ApproximateTileLength { get; }

	public Tile GetTile( ReadOnlyFace face ) => this.Tiles[ face.Id ];
}

Node.cs:
using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Node : StateContainerBase<Node> {

	public Node( Globe globe, ReadOnlyVertex vertex ) {
		this.Globe = globe;
		this.Vertex = vertex;
	}

	public Globe Globe { get; }
	public ReadOnlyVertex Vertex { get; }
	public IReadOnlyList<Node> NeighbouringNodes { get; internal set; }
}
StateBase.cs:
namespace Civlike.World.State;

public abstract class StateBase<TContainer>
	where TContainer : StateContainerBase<TContainer> {
	public TContainer StateContainer { get; private set; } = null!;

	internal void SetStateContainer( TContainer stateContainer )
		=> StateContainer = stateContainer;

	public void InvokeStateChanged()
		=> StateContainer.InvokeStateChanged( this );
}

StateContainerBase.cs:
using System.Diagnostics.CodeAnalysis;

namespace Civlike.World.State;

public abstract class StateContainerBase<TContainer>
	where TContainer : StateContainerBase<TContainer> {
	private readonly Dictionary<Type, StateBase<TContainer>> _states = [];
	public event Action<StateBase<TContainer>>? StateChanged;

	public TState GetStateOrThrow<TState>() where TState : StateBase<TContainer> {
		if (!_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? state ))
			throw new ArgumentException( "State of type " + typeof( TState ) + " does not exist." );
		return (TState) state;
	}

	public TState? GetState<TState>() where TState : StateBase<TContainer> {
		if (!_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? state ))
			return null;
		return (TState) state;
	}

	public bool TryGetState<TState>( [NotNullWhen( true )] out TState? state ) where TState : StateBase<TContainer> {
		state = null;
		if( !_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? stateBase ))
			return false;
		state = (TState) stateBase;
		return true;
	}

	public void AddState<TState>( TState state ) where TState : StateBase<TContainer> {
		if (!_states.TryAdd( typeof( TState ), state ))
			throw new ArgumentException( "State of type " + typeof( TState ) + " already exists." );
		TContainer container = this as TContainer ?? throw new InvalidOperationException();
		state.SetStateContainer( container );
	}

	internal void InvokeStateChanged( StateBase<TContainer> nodeState )
		=> StateChanged?.Invoke( nodeState );
}
Tile.cs:
using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Tile : StateContainerBase<Tile> {

	internal Tile( Globe globe, ReadOnlyFace face, IEnumerable<Node> nodes ) {
		this.Globe = globe;
		this.Face = face;
		this.Nodes = nodes.ToList().AsReadOnly();
	}

	public Globe Globe { get; }
	public ReadOnlyFace Face { get; }
	public IReadOnlyList<Node> Nodes { get; }

	public int Id => this.Face.Id;
	public IEnumerable<Tile> Neighbours => this.Face.Neighbours.Select( n => this.Globe.Tiles[ n.Id ] );

}

Noise3.cs:
using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;
public class Noise3 {

	public float Scale { get; set; } = 1.0f;
	public uint Seed { get; set; } = 0;

	public Noise3( int seed, float scale ) {
		this.Seed = unchecked((uint) seed);
		this.Scale = scale;
	}

	public Noise3( uint seed, float scale ) {
		this.Seed = seed;
		this.Scale = scale;
	}

	public float Noise( Vector3<float> xyz ) => Noise( this.Seed, this.Scale, xyz );

	public static float Noise( uint seed, float scale, Vector3<float> xyz ) {
		Vector3<float> scaledXyz = xyz * scale;
		Vector3<int> low = scaledXyz.Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> frac = scaledXyz - low.CastSaturating<int, float>();
		Span<Vector3<int>> hashPoints = [
			low + (0, 0, 0), low + (0, 0, 1), low + (0, 1, 0), low + (0, 1, 1),
			low + (1, 0, 0), low + (1, 0, 1), low + (1, 1, 0), low + (1, 1, 1)
		];

		Span<float> dataValues = stackalloc float[ 8 ];
		for (int i = 0; i < 8; i++) {
			Vector3<int> point = hashPoints[ i ];
			dataValues[ i ] = Hash4( point.X, point.Y, point.Z, seed );
		}

		float xInterp = float.Cos( frac.X * MathF.PI ) * 0.5f + 0.5f;
		float yInterp = float.Cos( frac.Y * MathF.PI ) * 0.5f + 0.5f;
		float zInterp = float.Cos( frac.Z * MathF.PI ) * 0.5f + 0.5f;

		Span<float> sumsZ = stackalloc float[ 4 ];
		for (int i = 0; i < sumsZ.Length; i++)
			sumsZ[ i ] = zInterp * dataValues[ i * 2 ] + (1 - zInterp) * dataValues[ i * 2 + 1 ];

		Span<float> sumsY = stackalloc float[ 2 ];
		for (int i = 0; i < sumsY.Length; i++)
			sumsY[ i ] = yInterp * sumsZ[ i * 2 ] + (1 - yInterp) * sumsZ[ i * 2 + 1 ];

		return xInterp * sumsY[ 0 ] + (1 - xInterp) * sumsY[ 1 ];
	}

	public static float Hash4( int x, int y, int z, uint seed ) {
		return Hash( unchecked(seed + (uint) x * 19 + (uint) y * 47 + (uint) z * 101) ) / (float) uint.MaxValue;
	}

	public static uint Hash( uint input ) {
		unchecked {
			uint x = input;
			x ^= x >> 16;
			x *= 0x85ebca6bu;
			x ^= x << 16;
			x *= 0xc2b2ae35u;
			return x;
		}
	}

}

SphericalVoronoiGenerator.cs:
using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class SphericalVoronoiGenerator {

	private readonly SphericalVoronoiRegion[] _regions;

	public SphericalVoronoiGenerator( Random r, int voronoiCenterPointCount, float minDistance ) {
		if (minDistance <= 0)
			throw new ArgumentOutOfRangeException( nameof( minDistance ), "Minimum distance must be greater than 0." );
		this._regions = new SphericalVoronoiRegion[ voronoiCenterPointCount ];

		for (int i = 0; i < voronoiCenterPointCount; i++) {
			bool foundOverlap;
			Vector3<float> vector;
			do {
				foundOverlap = false;
				float yaw = (r.NextSingle() * 2 - 1) * float.Pi;
				float pitch = (r.NextSingle() * 2 - 1) * float.Pi;
				Vector2<float> sphericalCoordinates = (yaw, pitch);
				vector = sphericalCoordinates.ToCartesianFromPolar( 1 );
				for (int j = 0; j < i; j++) {
					Vector3<float> diff = vector - this._regions[ j ].Position;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance > minDistance)
						continue;
					foundOverlap = true;
					break;
				}
			} while (foundOverlap);

			this._regions[ i ] = new SphericalVoronoiRegion( vector );
		}
	}

	public IReadOnlyList<SphericalVoronoiRegion> Regions => this._regions;

	public SphericalVoronoiRegion Get( Vector3<float> xyz ) {
		SphericalVoronoiRegion? currentRegion = null;
		float currentMinDistance = float.MaxValue;
		for (int i = 0; i < this._regions.Length; i++) {
			SphericalVoronoiRegion region = this._regions[ i ];
			Vector3<float> vec = region.Position;
			Vector3<float> diff = xyz - vec;
			float distance = diff.MagnitudeSquared();
			if (distance < currentMinDistance) {
				currentMinDistance = distance;
				currentRegion = region;
				continue;
			}
		}
		if (currentRegion is null)
			throw new InvalidOperationException( "No plate found." );

		return currentRegion;
	}

	public SphericalVoronoiRegion GetWithGradients( Vector3<float> xyz, List<(SphericalVoronoiRegion region, float gradient)> neighbours, float borderGradientFactor, float lowerBounds ) {
		neighbours.Clear();
		SphericalVoronoiRegion? currentRegion = null;
		float currentMinDistance = float.MaxValue;
		for (int i = 0; i < this._regions.Length; i++) {
			SphericalVoronoiRegion region = this._regions[ i ];
			Vector3<float> vec = region.Position;
			Vector3<float> diff = xyz - vec;
			float distance = diff.MagnitudeSquared();
			if (distance < currentMinDistance) {
				if (currentRegion is not null)
					neighbours.Add( (currentRegion, currentMinDistance) );
				currentMinDistance = distance;
				currentRegion = region;
				continue;
			} else
				neighbours.Add( (region, distance) );
		}
		if (currentRegion is null)
			throw new InvalidOperationException( "No plate found." );

		for (int i = neighbours.Count - 1; i >= 0; i--) {
			float distance = neighbours[ i ].Item2;
			Vector3<float> pointsDiff = currentRegion.Position - neighbours[ i ].Item1.Position;
			float pointsDistance = pointsDiff.Magnitude<Vector3<float>, float>();
			float gradient = float.Exp( -(distance - currentMinDistance) / pointsDistance * borderGradientFactor );
			if (gradient < lowerBounds) {
				neighbours.RemoveAt( i );
				continue;
			}
			neighbours[ i ] = (neighbours[ i ].region, gradient);
		}

		return currentRegion;
	}
}

SphericalVoronoiRegion.cs:
using Civlike.World.State;
using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class SphericalVoronoiRegion( Vector3<float> position ) : StateContainerBase<SphericalVoronoiRegion> {
	public readonly Vector3<float> Position = position;
}