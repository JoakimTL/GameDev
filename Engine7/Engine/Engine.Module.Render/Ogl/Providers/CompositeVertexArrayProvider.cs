using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class CompositeVertexArrayProvider( CompositeVertexArrayObjectService compositeVertexArrayObjectService ) : IServiceProvider {
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	public CompositeVertexArrayObject? GetVertexArray<TVertexData, TSceneInstanceData>() where TVertexData : unmanaged where TSceneInstanceData : unmanaged => this._compositeVertexArrayObjectService.Get( [ typeof( TVertexData ), typeof( TSceneInstanceData ) ] );
}
