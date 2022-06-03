using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering.ResourceManagement;
using Engine.Rendering.Standard;
using OpenGL;
using TestPlatform.Voxels.World;

namespace TestPlatform.Voxels.Rendering;
public static class VoxelIdRenderData {

	private static readonly VoxelIdRenderDataManager _manager = new();

	public static bool TryAddVoxelData( DataBlockCollection collection ) {
		if ( _manager._voxelData is null ) {
			Log.Warning( "Voxel data buffer not initialized yet!" );
			return false;
		}
		collection.AddBlock( _manager._voxelData );
		return true;
	}


	private class VoxelIdRenderDataManager : IContextUpdateable {

		public ShaderStorageBufferObject? _voxelData;
		public readonly ConcurrentQueue<ushort> _voxelTypeChanges;

		public VoxelIdRenderDataManager() {
			this._voxelTypeChanges = new ConcurrentQueue<ushort>();
			TryInitialize();
			Resources.Render.ContextUpdate.Add( this );
		}

		private void TryInitialize() {
			if ( Resources.Render.InThread ) {
				this._voxelData = new ShaderStorageBufferObject( "VoxelDataBlock", (uint) Marshal.SizeOf<VoxelData>() * Voxel.MaxVoxelCount, ShaderType.VertexShader );
				for ( ushort i = 0; i < Voxel.VoxelCount; i++ ) {
					Voxel? voxel = Voxel.Get( i );
					if ( voxel is not null )
						this._voxelData.DirectWrite( voxel.GetRenderData(), (uint) Marshal.SizeOf<VoxelData>() * i );
				}
				Voxel.Changed += VoxelsChanged;
				Log.Line( $"Voxel face id data set up!", Log.Level.NORMAL );
			}
		}

		private void VoxelsChanged( ushort id ) => this._voxelTypeChanges.Enqueue( id );

		public void ContextUpdate() {
			if ( this._voxelData is null ) {
				TryInitialize();
			} else {
				if ( this._voxelTypeChanges.TryDequeue( out ushort id ) ) {
					Voxel? voxel = Voxel.Get( id );
					if ( voxel is not null )
						this._voxelData.DirectWrite( voxel.GetRenderData(), (uint) Marshal.SizeOf<VoxelData>() * id );
				}
			}
		}
	}
}
