﻿using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Module.Render.Entities.Providers;

public sealed class MeshProvider( MeshService meshService ) {
	private readonly MeshService _meshService = meshService;
	public VertexMesh<TVertex> CreateEmptyMesh<TVertex>( uint vertexCount, uint indexCount, string? name = null ) where TVertex : unmanaged => this._meshService.CreateEmptyMesh<TVertex>( vertexCount, indexCount, name );
	public VertexMesh<TVertex> CreateMesh<TVertex>( Span<TVertex> vertices, Span<uint> indices, string? name = null ) where TVertex : unmanaged => this._meshService.CreateMesh( vertices, indices, name );
	public VertexMesh<TVertex> CreateMesh<TVertex>( TVertex[] vertices, uint[] indices, string? name = null ) where TVertex : unmanaged => this._meshService.CreateMesh( vertices, indices, name );
}