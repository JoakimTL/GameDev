﻿using Engine.Rendering.Objects;
using Engine.Structure.Interfaces;

namespace Engine.Rendering.Services;

public class RenderBufferObjectService : IContextService, IUpdateable {

	private readonly Dictionary<Type, RenderBufferObject> _rbos;
	private readonly VertexBufferObjectService _vertexBufferObjectService;

	public RenderBufferObjectService( VertexBufferObjectService vertexBufferObjectService ) {
		_rbos = new();
		this._vertexBufferObjectService = vertexBufferObjectService;
	}

	public RenderBufferObject Get( Type t ) {
		if ( _rbos.TryGetValue( t, out RenderBufferObject? rbo ) )
			return rbo;
		rbo = new RenderBufferObject( t.Name, 65536 );
		_rbos[ t ] = rbo;
		return rbo;
	}

	public void Update( float time, float deltaTime ) {
		foreach ( var rboKvp in _rbos )
			rboKvp.Value.SyncChanges( _vertexBufferObjectService.Get( rboKvp.Key ) );
	}
}