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
		this._commandBuffer = new( BufferUsage.DynamicDraw, (uint) Unsafe.SizeOf<IndirectCommand>() * 4096 );
		this._indirectCommands = [];
		this._stages = [];
	}

	public void PrepareForRender( IReadOnlyList<SceneLayer> layers ) {
		this._indirectCommands.Clear();
		this._stages.Clear();
		foreach (SceneLayer layer in layers)
			foreach (SceneObject sceneObject in layer.SceneObjects) {
				uint countBeforeAddition = (uint) this._indirectCommands.Count;
				sceneObject.AddIndirectCommands( this._indirectCommands );
				int numberOfCommandsInStage = this._indirectCommands.Count - (int) countBeforeAddition;
				if (numberOfCommandsInStage == 0)
					continue;
				this._stages.Add( new( sceneObject.VertexArrayObject, sceneObject.ShaderBundle, countBeforeAddition, numberOfCommandsInStage ) );
			}
		unsafe {
			if (this._indirectCommands.Count * Unsafe.SizeOf<IndirectCommand>() > _commandBuffer.LengthBytes) {
				IndirectCommand[] commandArray = this._indirectCommands.ToArray();
				uint newSize = this._commandBuffer.LengthBytes * 2;
				while (this._indirectCommands.Count * Unsafe.SizeOf<IndirectCommand>() > newSize)
					newSize *= 2;
				fixed (IndirectCommand* srcPtr = commandArray) {
					this._commandBuffer.ResizeWrite( (nint) srcPtr, newSize );
				}
				return;
			}
			if (this._indirectCommands.Count < 32768) {
				Span<IndirectCommand> commands = stackalloc IndirectCommand[ this._indirectCommands.Count ];
				this._indirectCommands.CopyTo( commands );
				fixed (IndirectCommand* srcPtr = commands) {
					this._commandBuffer.WriteRange( srcPtr, (uint) (commands.Length * sizeof( IndirectCommand )), 0 );
				}
				return;
			}
			{
				IndirectCommand[] commandArray = this._indirectCommands.ToArray();
				fixed (IndirectCommand* srcPtr = commandArray) {
					this._commandBuffer.WriteRange( srcPtr, (uint) (commandArray.Length * sizeof( IndirectCommand )), 0 );
				}
			}
		}
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, this._commandBuffer.BufferId );
		foreach (RenderStage stage in this._stages) {
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
		this._commandBuffer.Dispose();
		return true;
	}
}
