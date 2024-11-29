using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.CompilerServices;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class SceneRender : DisposableIdentifiable, ISceneRender {
	private readonly OOP.Buffers.OglDynamicBuffer _commandBuffer;
	private readonly List<IndirectCommand> _indirectCommands;
	private readonly List<RenderStage> _stages;

	public SceneRender() {
		_commandBuffer = new( BufferUsage.DynamicDraw, (uint) Unsafe.SizeOf<IndirectCommand>() * 4096 );
		_indirectCommands = [];
		_stages = [];
	}

	public void PrepareForRender( IReadOnlyList<SceneLayer> layers ) {
		_indirectCommands.Clear();
		_stages.Clear();
		foreach (SceneLayer layer in layers)
			foreach (SceneObject sceneObject in layer.SceneObjects) {
				uint countBeforeAddition = (uint) _indirectCommands.Count;
				sceneObject.AddIndirectCommands( _indirectCommands );
				_stages.Add( new( sceneObject.VertexArrayObject, sceneObject.ShaderBundle, countBeforeAddition, _indirectCommands.Count - (int) countBeforeAddition ) );
			}
		unsafe {
			Span<IndirectCommand> commands = stackalloc IndirectCommand[ _indirectCommands.Count ];
			_indirectCommands.CopyTo( commands );
			fixed (IndirectCommand* srcPtr = commands) {
				_commandBuffer.WriteRange( srcPtr, (uint) (commands.Length * sizeof( IndirectCommand )), 0 );
			}
		}
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, _commandBuffer.BufferId );
		foreach (RenderStage stage in _stages) {
			OglShaderPipelineBase? shader = stage.ShaderBundle.Get( shaderIndex );
			if (shader is null)
				continue;
			shader.Bind();
			stage.VertexArrayObject.Bind();
			dataBlocks?.BindShader( shader );
			Gl.MultiDrawElementsIndirect( primitiveType, DrawElementsType.UnsignedInt, new nint( stage.StageCommandStart * Unsafe.SizeOf<IndirectCommand>() ), stage.StageCommandCount, 0 );
			dataBlocks?.UnbindBuffers();
		}
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

	protected override bool InternalDispose() {
		_commandBuffer.Dispose();
		return true;
	}
}
