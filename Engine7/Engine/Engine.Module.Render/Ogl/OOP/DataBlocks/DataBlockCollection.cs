using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Scenes;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.DataBlocks;

public class DataBlockCollection : Identifiable, IDataBlockCollection {

	private readonly Dictionary<ShaderType, List<DataBlockBase>> _bindings;

	public DataBlockCollection( params DataBlockBase[] blocks ) {
		this._bindings = [];
		foreach (DataBlockBase? block in blocks)
			AddBlock( block );
	}

	public void AddBlock( DataBlockBase block ) {
		foreach (ShaderType shaderType in block.ShaderTypes) {
			if (!this._bindings.TryGetValue( shaderType, out List<DataBlockBase>? list ))
				this._bindings.Add( shaderType, list = [] );
			list.Add( block );
		}
	}

	public void UnbindBuffers() {
		foreach (List<DataBlockBase> blocks in this._bindings.Values)
			for (int i = 0; i < blocks.Count; i++)
				blocks[ i ].UnbindBuffer();
	}

	public void BindShader( OglShaderPipelineBase pipeline ) {
		foreach (KeyValuePair<ShaderType, List<DataBlockBase>> kvp in this._bindings)
			if (pipeline.Programs.TryGetValue( kvp.Key, out OglShaderProgramBase? program ))
				for (int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[ i ].Bind( program );
	}

}