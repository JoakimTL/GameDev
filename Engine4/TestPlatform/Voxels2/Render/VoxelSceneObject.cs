using Engine;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace TestPlatform.Voxels2.Render;

public class VoxelSceneObject : ClosedSceneObject<Vertex3, VoxelFaceData> {

	private readonly Dictionary<VoxelChunkFaceRenderer, RenderRange> _renderRanges;

	public bool HasCapacity => this.SceneData is not null && this.SceneData.ActiveInstances < this.SceneData.MaxInstances;

	//This scene object is not tied to any chunk, it is a collection of chunk rendering data.
	public VoxelSceneObject( VoxelFaceDirection faceDirection ) {
		SetMesh( faceDirection switch {
			VoxelFaceDirection.NORTH => ( Resources.Render.Mesh3.FaceForward ),
			VoxelFaceDirection.SOUTH => ( Resources.Render.Mesh3.FaceBackward ),
			VoxelFaceDirection.WEST => ( Resources.Render.Mesh3.FaceLeft ),
			VoxelFaceDirection.EAST => ( Resources.Render.Mesh3.FaceRight ),
			VoxelFaceDirection.UP => ( Resources.Render.Mesh3.FaceUp ),
			VoxelFaceDirection.DOWN => ( Resources.Render.Mesh3.FaceDown ),
			_ => ( Resources.Render.Mesh3.Cube )
		} );
		SetShaders( Resources.Render.Shader.Bundles.Get<VoxelShaderBundle>() );
		SetSceneData( new SceneInstanceData<VoxelFaceData>( 65536, 0 ) ); // 64K elements, should cover a lot of voxel ground!
		this._renderRanges = new Dictionary<VoxelChunkFaceRenderer, RenderRange>();
	}

	public void Clear( VoxelChunkFaceRenderer renderer ) {
		if ( this.SceneData is null || !this._renderRanges.TryGetValue( renderer, out RenderRange? range ) )
			return;
		uint size = ( range.End - range.Start );
		this.SceneData.Remove( range.Start, size );
		this.SceneData.SetActiveInstances( this.SceneData.ActiveInstances - size );
		this._renderRanges.Remove( renderer );
		foreach ( RenderRange otherRange in this._renderRanges.Values ) {
			if ( otherRange.End > range.End )
				otherRange.ShiftDown( size );
		}
	}

	/// <returns>How many faces were added to this scene object</returns>
	public uint Add( VoxelChunkFaceRenderer renderer, uint startIndex, VoxelFaceData[] data ) {
		if ( this.SceneData is null || this._renderRanges.ContainsKey( renderer ) )
			return 0;
		uint available = this.SceneData.MaxInstances - this.SceneData.ActiveInstances;
		if ( available == 0 )
			return 0;
		Span<VoxelFaceData> span = data.AsSpan().Slice( (int) startIndex, (int) Math.Min( available, data.Length - startIndex ) );
		if ( span.Length > 0 ) {
			uint activeInstances = this.SceneData.ActiveInstances;
			uint newActiveCount = activeInstances + (uint) span.Length;
			this.SceneData.SetInstances( activeInstances, span );
			this._renderRanges.Add( renderer, new RenderRange( activeInstances, newActiveCount ) );
			this.SceneData.SetActiveInstances( newActiveCount );
		}
		return (uint) span.Length;
	}
	public override void Bind() { }

	private class RenderRange {
		public uint Start;
		public uint End;

		public RenderRange( uint start, uint end ) {
			this.Start = start;
			this.End = end;
		}

		public void ShiftDown( uint elements ) {
			this.Start -= elements;
			this.End -= elements;
		}

		public override string ToString() => $"{this.Start} -> {this.End}";
	}
}
