using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Entities.Render;

public sealed class ShaderBundleProvider( ShaderBundleService shaderBundleService ) {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	internal ShaderBundleBase? GetShaderBundle<TShaderBundle>() where TShaderBundle : ShaderBundleBase => _shaderBundleService.Get( typeof( TShaderBundle ) );
}
public sealed class MeshProvider( MeshService meshService ) {
	private readonly MeshService _meshService = meshService;
	internal VertexMesh<TVertex> CreateEmptyMesh<TVertex>( uint vertexCount, uint indexCount ) where TVertex : unmanaged => _meshService.CreateEmptyMesh<TVertex>( vertexCount, indexCount );
	internal VertexMesh<TVertex> CreateMesh<TVertex>( Span<TVertex> vertices, Span<uint> indices ) where TVertex : unmanaged => _meshService.CreateMesh( vertices, indices );
}