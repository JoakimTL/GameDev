using System.Numerics;
using Engine;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Transforms;

namespace TestPlatform.Voxels.World;
public abstract class VoxelWorldBase : DisposableIdentifiable {

	protected readonly Transform3 _transform;
	public TransformReadonly<Vector3, Quaternion, Vector3> Transform => this._transform.Readonly;

	public abstract bool IsInfinite { get; }

	public VoxelWorldBase( string name ) : base( name ) {
		this._transform = new Transform3();
	}

	public Vector3i GetVoxelCoordinate( Vector3 worldTranslation ) => Vector3i.Floor( Vector3.Transform( worldTranslation - this._transform.GlobalTranslation, Quaternion.Inverse( this._transform.GlobalRotation ) ) / this._transform.GlobalScale );

	/// <param name="translation">World coordinates</param>
	/// <returns>The chunk this coordinate falls into.</returns>
	public VoxelChunk? GetChunk( Vector3 translation ) => GetChunk( GetVoxelCoordinate( translation ) );
	/// <param name="translation">World coordinates</param>
	/// <returns>The id of the voxel.</returns>
	public ushort GetId( Vector3 translation ) => GetId( GetVoxelCoordinate( translation ) );
	/// <param name="translation">World coordinates</param>
	/// <param name="id">Id to set the voxel to</param>
	public void SetId( Vector3 translation, ushort id ) => SetId( GetVoxelCoordinate( translation ), id );

	/// <param name="translation">World voxel coordinates</param>
	/// <returns>The chunk this coordinate falls into.</returns>
	public abstract VoxelChunk? GetChunk( Vector3i translation );
	/// <param name="translation">World voxel coordinates</param>
	/// <returns>The id of the voxel.</returns>
	public abstract ushort GetId( Vector3i translation );
	/// <param name="translation">World voxel coordinates</param>
	/// <param name="id">Id to set the voxel to</param>
	public abstract void SetId( Vector3i translation, ushort id );
	public abstract void GetChunks( AABB3i chunkArea, List<VoxelChunk?> chunks );
	public abstract bool HasGenerated( Vector3i voxelTranslation );
	internal VoxelWorldModelData GetWorldModelData() => new() { ModelMatrix = this._transform.Matrix };

	//SAVING
}
