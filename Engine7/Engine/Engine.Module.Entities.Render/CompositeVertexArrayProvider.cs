using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Entities.Render;

public sealed class CompositeVertexArrayProvider( CompositeVertexArrayObjectService compositeVertexArrayObjectService ) {
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	internal OglVertexArrayObjectBase? GetVertexArray<TVertexData, TSceneInstanceData>() where TVertexData : unmanaged where TSceneInstanceData : unmanaged => this._compositeVertexArrayObjectService.Get( [ typeof( TVertexData ), typeof( TSceneInstanceData ) ] );
}
