﻿using OpenGL;

namespace Engine.Rendering.Standard;

public class DataBlockCollection : Identifiable, IDataBlockCollection {

	private readonly Dictionary<ShaderType, List<DataBlock>> _bindings;

	public DataBlockCollection( params DataBlock[] blocks ) {
		this._bindings = new Dictionary<ShaderType, List<DataBlock>>();
		foreach ( DataBlock? block in blocks )
			AddBlock( block );
	}

	public void AddBlock( DataBlock block ) {
		foreach ( ShaderType shaderType in block.ShaderTypes ) {
			if ( !this._bindings.TryGetValue( shaderType, out List<DataBlock>? list ) )
				this._bindings.Add( shaderType, list = new List<DataBlock>() );
			list.Add( block );
		}
	}

	public void DirectUnbindBuffers() {
		foreach ( List<DataBlock> blocks in this._bindings.Values )
			for ( int i = 0; i < blocks.Count; i++ )
				blocks[ i ].DirectUnbindBuffer();
	}

	public void DirectBindShader( ShaderPipeline s ) {
		foreach ( KeyValuePair<ShaderType, List<DataBlock>> kvp in this._bindings )
			if ( s.Programs.TryGetValue( kvp.Key, out ShaderProgram? p ) )
				for ( int i = 0; i < kvp.Value.Count; i++ )
					kvp.Value[ i ].DirectBindShader( p );
	}

}
