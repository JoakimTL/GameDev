using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using OpenGL;
using TestPlatform.Voxels.World;

namespace TestPlatform.Voxels.Rendering;

public class VoxelWorldRenderManager : DisposableIdentifiable {

	public const uint GenerationThreads = 4;
	public SceneRenderer SceneRender { get; }
	public VoxelWorldBase World { get; }
	private readonly Scene _scene;
	private readonly DataBlockCollection _dataBlocks;
	private readonly UniformBlock _worldModelData;
	private bool _voxelDataInBlockCollection;

	private readonly Thread[] _threadPool;
	private readonly Queue<VoxelChunk> _disposedChunks;
	private readonly BlockingCollection<ChunkRenderManager> _generationQueue;
	private readonly Dictionary<VoxelChunk, ChunkRenderManager> _chunkRenders;
	private readonly List<VoxelChunk?> _chunks;
	private readonly AutoResetEvent _generationAllottment;
	private readonly CancellationTokenSource _cancellationTokenSource;

	private readonly VertexMesh<Vertex3>[] _meshes;
	private uint _voxelViewRange;

	public VoxelWorldRenderManager( VoxelWorldBase world, uint voxelViewRange ) {
		this.World = world;
		this._scene = new LayerlessScene();
		this._voxelViewRange = voxelViewRange;
		this._chunks = new List<VoxelChunk?>();
		this._meshes = new VertexMesh<Vertex3>[] {
			Resources.Render.Mesh3.FaceUp,
			Resources.Render.Mesh3.FaceDown,
			Resources.Render.Mesh3.FaceRight,
			Resources.Render.Mesh3.FaceLeft,
			Resources.Render.Mesh3.FaceForward,
			Resources.Render.Mesh3.FaceBackward,
		};
		this._chunkRenders = new Dictionary<VoxelChunk, ChunkRenderManager>();
		this._generationQueue = new BlockingCollection<ChunkRenderManager>();
		this._disposedChunks = new Queue<VoxelChunk>();
		this._generationAllottment = new AutoResetEvent( true );

		Resources.Render.Shader.Bundles.Get<VoxelShaderBundle>();
		Resources.Render.Shader.Bundles.Get<VoxelTransparentShaderBundle>();

		this._cancellationTokenSource = new CancellationTokenSource();
		this._threadPool = new Thread[ GenerationThreads ];
		for ( uint i = 0; i < GenerationThreads; i++ )
			this._threadPool[ i ] = Resources.GlobalService<ThreadManager>().Start( ChunkMeshGeneration, $"{this.FullName}[{i}]" );

		this._dataBlocks = new DataBlockCollection( this._worldModelData = new UniformBlock( "VoxelWorldModelDataBlock", (uint) Marshal.SizeOf<VoxelWorldModelData>(), ShaderType.VertexShader ) );
		this._voxelDataInBlockCollection = VoxelIdRenderData.TryAddVoxelData( this._dataBlocks );
		this.SceneRender = new SceneRenderer( this._scene, this._dataBlocks );
	}

	private void ChunkMeshGeneration() {
		while ( !this.Disposed ) {
			if ( this._generationAllottment.WaitOne( 1000 ) )
				if ( this._generationQueue.TryTake( out ChunkRenderManager? chunkRenderManager, 500, this._cancellationTokenSource.Token ) ) {
					chunkRenderManager.GenerateMesh();
				}
		}
	}

	public VertexMesh<Vertex3>? GetMesh( int index ) {
		if ( index < 0 || index >= this._meshes.Length )
			return null;
		return this._meshes[ index ];
	}

	public void SetViewRange( uint newRange ) => this._voxelViewRange = newRange;

	public void Update( float time, Vector3 cameraTranslation ) {
		if ( !this._voxelDataInBlockCollection )
			this._voxelDataInBlockCollection = VoxelIdRenderData.TryAddVoxelData( this._dataBlocks );
		this._worldModelData.DirectWrite( this.World.GetWorldModelData() );

		Vector3i minChunk = Vector3i.Floor( this.World.GetVoxelCoordinate( cameraTranslation - new Vector3( this._voxelViewRange ) ) / VoxelChunk.SideLength );
		Vector3i maxChunk = Vector3i.Floor( this.World.GetVoxelCoordinate( cameraTranslation + new Vector3( this._voxelViewRange ) ) / VoxelChunk.SideLength );

		this.World.GetChunks( new AABB3i( minChunk, maxChunk ), this._chunks );

		for ( int i = 0; i < this._chunks.Count; i++ ) {
			VoxelChunk? chunk = this._chunks[ i ];
			if ( chunk is null )
				continue;
			if ( !this._chunkRenders.TryGetValue( chunk, out ChunkRenderManager? chunkRender ) ) {
				if ( chunk.AreNeighboursAvailable() ) {
					this._chunkRenders.Add( chunk, chunkRender = new ChunkRenderManager( this, chunk ) );
					this._generationQueue.Add( chunkRender );
				}
			}
			chunkRender?.Update( time );
		}

		foreach ( KeyValuePair<VoxelChunk, ChunkRenderManager> kvp in this._chunkRenders )
			if ( kvp.Value.ShouldDispose( time, 10 ) ) {
				kvp.Value.Dispose();
				this._disposedChunks.Enqueue( kvp.Key );
			}

		while ( this._disposedChunks.TryDequeue( out VoxelChunk? chunk ) ) {
			this._chunkRenders.Remove( chunk );
		}

		this._generationAllottment.Set();
	}

	protected override bool OnDispose() {
		this._cancellationTokenSource.Cancel();
		foreach ( ChunkRenderManager? chunkRender in this._chunkRenders.Values )
			chunkRender.Dispose();
		return true;
	}

	internal void Add( ChunkRender chunkRender ) => this._scene.AddSceneObject( chunkRender );
	internal void Remove( ChunkRender chunkRender ) => this._scene.RemoveSceneObject( chunkRender );
}
