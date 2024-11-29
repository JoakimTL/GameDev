using Engine;
using Engine.Buffers;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using OpenGL;

namespace Sandbox;

public sealed class TestPipelineNoScene( ShaderBundleService shaderBundleService, BufferService bufferService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, DataBlockService dataBlockService, OglBufferService oglBufferService, MeshService meshService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	private readonly BufferService _bufferService = bufferService;
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly OglBufferService _oglBufferService = oglBufferService;
	private readonly MeshService _meshService = meshService;

	private SegmentedSystemBuffer _sceneDataBuffer = null!;
	private BufferSegment _sceneDataBufferSegment = null!;
	private SegmentedSystemBuffer _commandBuffer = null!;
	private BufferSegment _commandBufferSegment = null!;
	private OglVertexArrayObjectBase _testVertexArrayObject = null!;
	private ShaderBundleBase _shaderBundle = null!;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private VertexMesh<Vertex2> _vertexMesh = null!;

	public void Initialize() {
		if (!_dataBlockService.CreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out _testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!_dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out _testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		_dataBlocks = new DataBlockCollection( _testUniforms, _testShaderStorage );

		this._sceneDataBuffer = this._bufferService.Get( typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexBuffer not found." );
		unsafe {
			if (!this._sceneDataBuffer.TryAllocate( 2 * (ulong) sizeof( Entity2SceneData ), out _sceneDataBufferSegment! ))
				throw new InvalidOperationException( "Couldn't allocate vertex buffer segment." );
		}

		_vertexMesh = this._meshService.CreateEmptyMesh<Vertex2>( 3, 3 ) ?? throw new NullReferenceException( "Mesh not found." );
		_vertexMesh.ElementBufferSegment.WriteRange( [ 0, 1, 2 ], 0 );

		_commandBuffer = this._bufferService.Get( typeof( IndirectCommand ) );
		unsafe {
			if (!_commandBuffer.TryAllocate( 1 * (ulong) sizeof( IndirectCommand ), out _commandBufferSegment! ))
				throw new InvalidOperationException( "Couldn't allocate command buffer segment." );
		}

		this._testVertexArrayObject = _compositeVertexArrayObjectService.Get( typeof( Vertex2 ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		this._vertexMesh.VertexBufferSegment.WriteRange( [
			new Vertex2 { Translation = (0.0f, 0.5f), Color = ((byte) (MathF.Sin((float) time) * 100 + 100), 255, 0, 255) },
			new Vertex2 { Translation = (-0.5f, -0.5f), Color = (255, 0, 0, 255) },
			new Vertex2 { Translation = (0.5f, -0.5f), Color = (255, 0, (byte) (MathF.Cos((float) time) * 100 + 100), 255) } ], 0 );
		this._sceneDataBufferSegment.WriteRange( [
			new Entity2SceneData { ModelMatrix = Matrix.Create4x4.RotationZ( -(float) time * 2 ) },
			new Entity2SceneData { ModelMatrix = Matrix.Create4x4.RotationZ( (float) time * 2 ) } ], 0 );
		_testUniforms.Buffer.WriteRange( [ MathF.Cos( (float) time ) / 2 ], 0 );
		_testShaderStorage.Buffer.WriteRange( [ MathF.Sin( (float) time ) / 2 ], 0 );

		IndirectCommand command = new( _vertexMesh.ElementCount, 2, _vertexMesh.ElementOffset, _vertexMesh.VertexOffset, 0 );
		_commandBufferSegment.WriteRange( [ command ], 0 );
	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		var shaderPipeline = this._shaderBundle.Get( "default" )!;
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, _oglBufferService.Get( typeof( IndirectCommand ) ).BufferId );
		shaderPipeline.Bind();
		this._testVertexArrayObject.Bind();
		this._dataBlocks.BindShader( shaderPipeline );
		Gl.MultiDrawElementsIndirect( PrimitiveType.Triangles, DrawElementsType.UnsignedInt, nint.Zero, 1, 0 );
		this._dataBlocks.UnbindBuffers();
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

	protected override bool InternalDispose() {
		this._testVertexArrayObject.Dispose();
		return true;
	}
}
