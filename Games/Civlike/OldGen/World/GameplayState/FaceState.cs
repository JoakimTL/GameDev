//using Engine;

//namespace OldGen.World.GameplayState;

//public sealed class FaceState( Face face, TerrainTypeBase terrainType ) {
//	private readonly Face _face = face;
//	private uint _terrainTypeId = terrainType.Id;
//	private FaceResources? _resources = terrainType.HasResources ? new() : null;

//	public Vector3<float> Arrow { get; set; }
//	public Vector4<float> Color { get; set; }

//	public TerrainTypeBase TerrainType => TerrainTypeList.GetTerrainType( this._terrainTypeId );

//	public FaceResources? Resources => this._resources;

//	public void SetTerrainType( TerrainTypeBase terrainType ) {
//		if (terrainType.Id == this._terrainTypeId)
//			return;
//		this._terrainTypeId = terrainType.Id;
//		this._resources = terrainType.HasResources ? new() : null;
//		this._face.TriggerFaceStateChanged();
//	}

//	public void SetColor( Vector3<float> color ) {
//		this.Color = new Vector4<float>( color.X, color.Y, color.Z, 1.0f );
//		this._face.TriggerFaceStateChanged();
//	}
//}
