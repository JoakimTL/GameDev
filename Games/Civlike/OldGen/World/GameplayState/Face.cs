//using Engine;
//using Engine.Structures;

//namespace OldGen.World.GameplayState;

//public sealed class Face : IOcTreeLeaf<float> {

//	public event Action<Face>? FaceStateChanged;

//	private Face( uint id ) {
//		this.Id = id;
//	}

//	public uint Id { get; }
//	public FaceBlueprint Blueprint { get; private set; } = null!;
//	public FaceState State { get; private set; } = null!;
//	public GenerationState.FaceBase? GenerationFace { get; private set; } = null!;

//	public AABB<Vector3<float>> Bounds => this.Blueprint.Bounds;

//	internal void TriggerFaceStateChanged() => FaceStateChanged?.Invoke( this );

//	public override bool Equals( object? obj ) => obj is Face vertex && vertex == this;
//	public override int GetHashCode() => this.Id.GetHashCode();

//	public static bool operator ==( Face? left, Face? right ) => ReferenceEquals(left, right);
//	public static bool operator !=( Face? left, Face? right ) => !(left == right);

//	public sealed class Builder( uint id, TerrainTypeBase terrainType ) {
//		public List<Vertex> Vertices { get; } = [];
//		public List<Edge> Edges { get; } = [];
//		public List<Face> Neighbours { get; } = [];
//		public TerrainTypeBase TerrainType { get; set; } = terrainType;
//		public Vector3<float> Debug_Arrow { get; set; } = Vector3<float>.Zero;
//		public Vector4<float> Debug_Color { get; set; } = Vector4<float>.Zero;
//		public GenerationState.FaceBase? GenerationFace { get; set; } = null;

//		public void Complete() {
//			if (this.TerrainType is null)
//				throw new InvalidOperationException( "Terrain type cannot be null." );
//			this.Face.Blueprint = new FaceBlueprint( this.Face, [ .. this.Vertices ], [ .. this.Edges ], [ .. this.Neighbours ] );
//			this.Face.State = new FaceState( this.Face, this.TerrainType );
//			this.Face.State.Arrow = this.Debug_Arrow;
//			this.Face.State.Color = this.Debug_Color;
//			this.Face.GenerationFace = this.GenerationFace;
//		}

//		public Face Face { get; } = new Face( id );
//	}
//}