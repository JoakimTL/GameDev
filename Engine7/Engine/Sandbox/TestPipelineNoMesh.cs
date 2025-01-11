using Engine.Buffers;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using OpenGL;

namespace Sandbox;

public sealed class TestPipelineNoMesh( ShaderBundleService shaderBundleService, BufferService bufferService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, DataBlockService dataBlockService, OglBufferService oglBufferService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	private readonly BufferService _bufferService = bufferService;
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly OglBufferService _oglBufferService = oglBufferService;

	private SegmentedSystemBuffer _vertexBuffer = null!;
	private BufferSegment _vertexBufferSegment = null!;
	private SegmentedSystemBuffer _sceneDataBuffer = null!;
	private BufferSegment _sceneDataBufferSegment = null!;
	private SegmentedSystemBuffer _elementBuffer = null!;
	private BufferSegment _elementBufferSegment = null!;
	private SegmentedSystemBuffer _commandBuffer = null!;
	private BufferSegment _commandBufferSegment = null!;
	private OglVertexArrayObjectBase _testVertexArrayObject = null!;
	private ShaderBundleBase _shaderBundle = null!;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;

	public void Initialize() {
		if (!this._dataBlockService.TryCreateUniformBlock( "testUniformBlock", 256, [ ShaderType.VertexShader ], out this._testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!this._dataBlockService.TryCreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out this._testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		this._dataBlocks = new DataBlockCollection( this._testUniforms, this._testShaderStorage );

		// ------------------------------- How to create a mesh in parts. ------------------------------- //
		this._vertexBuffer = this._bufferService.Get( typeof( VertexSpecial2 ) ) ?? throw new NullReferenceException( "VertexBuffer not found." );
		unsafe {
			if (!this._vertexBuffer.TryAllocate( 6 * (ulong) sizeof( VertexSpecial2 ), out this._vertexBufferSegment! ))
				throw new InvalidOperationException( "Couldn't allocate vertex buffer segment." );
		}
		this._elementBuffer = this._bufferService.ElementBuffer;
		if (!this._elementBuffer.TryAllocate( 3 * sizeof( int ), out this._elementBufferSegment! ))
			throw new InvalidOperationException( "Couldn't allocate element buffer segment." );
		this._vertexBufferSegment.WriteRange( [
			new VertexSpecial2 { Translation = (0.0f, 0.5f), Color = (255, 255, 0, 255) },
			new VertexSpecial2 { Translation = (-0.5f, -0.5f), Color = (255, 0, 0, 255) },
			new VertexSpecial2 { Translation = (0.5f, -0.5f), Color = (255, 0, 255, 255) } ], 0 );
		this._elementBufferSegment.WriteRange( [ 0, 1, 2 ], 0 );

		this._sceneDataBuffer = this._bufferService.Get( typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexBuffer not found." );
		unsafe {
			if (!this._sceneDataBuffer.TryAllocate( 2 * (ulong) sizeof( Entity2SceneData ), out this._sceneDataBufferSegment! ))
				throw new InvalidOperationException( "Couldn't allocate vertex buffer segment." );
		}

		this._commandBuffer = this._bufferService.Get( typeof( IndirectCommand ) );
		unsafe {
			if (!this._commandBuffer.TryAllocate( 1 * (ulong) sizeof( IndirectCommand ), out this._commandBufferSegment! ))
				throw new InvalidOperationException( "Couldn't allocate command buffer segment." );
		}

		this._testVertexArrayObject = this._compositeVertexArrayObjectService.Get( typeof( VertexSpecial2 ), typeof( Entity2SceneData ) ) ?? throw new NullReferenceException( "VertexArrayObject not found." );
		this._shaderBundle = this._shaderBundleService.Get<TestShaderBundle>() ?? throw new NullReferenceException( "ShaderBundle not found." );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		this._vertexBufferSegment.WriteRange( [
			new VertexSpecial2 { Translation = (0.0f, 0.5f), Color = ((byte) (MathF.Sin((float) time) * 100 + 100), 255, 0, 255) },
			new VertexSpecial2 { Translation = (-0.5f, -0.5f), Color = (255, 0, 0, 255) },
			new VertexSpecial2 { Translation = (0.5f, -0.5f), Color = (255, 0, (byte) (MathF.Cos((float) time) * 100 + 100), 255) } ], 0 );
		this._sceneDataBufferSegment.WriteRange( [
			new Entity2SceneData ( Matrix.Create4x4.RotationZ( -(float) time * 2 ), ushort.MaxValue ),
			new Entity2SceneData ( Matrix.Create4x4.RotationZ( (float) time * 2 ), ushort.MaxValue ) ], 0 );
		this._testUniforms.Buffer.WriteRange( [ MathF.Cos( (float) time ) / 2 ], 0 );
		this._testShaderStorage.Buffer.WriteRange( [ MathF.Sin( (float) time ) / 2 ], 0 );

		IndirectCommand command = new( 3, 2, 0, 0, 0 );
		this._commandBufferSegment.WriteRange( [ command ], 0 );
	}

	public void DrawToScreen() {
		Gl.Clear( ClearBufferMask.ColorBufferBit );
		OglShaderPipelineBase shaderPipeline = this._shaderBundle.Get( "default" )!;
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, this._oglBufferService.Get( typeof( IndirectCommand ) ).BufferId );
		shaderPipeline.Bind();
		this._testVertexArrayObject.Bind();
		this._dataBlocks.BindShader( shaderPipeline );
		Gl.MultiDrawElementsIndirect( PrimitiveType.Triangles, DrawElementsType.UnsignedInt, nint.Zero, 1, 0 );
		this._dataBlocks.UnbindBuffers();
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

	protected override bool InternalDispose() {
		this._vertexBuffer.Dispose();
		this._elementBuffer.Dispose();
		return true;
	}
}
