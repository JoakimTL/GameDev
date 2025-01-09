using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Standard.Render;

public sealed class SceneInstance<TInstanceData>() : SceneInstanceBase( typeof( TInstanceData ) ) where TInstanceData : unmanaged {
	public new void SetLayer( uint layer ) => base.SetLayer( layer );
	public new void SetMesh( IMesh? mesh ) => base.SetMesh( mesh );
	public new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
	public new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );
	public new void SetActive( bool active ) => base.SetActive( active );

	public bool Write( TInstanceData data ) => Write<TInstanceData>( data );
	public bool TryRead( out TInstanceData data ) => TryRead<TInstanceData>( out data );

	protected override void Initialize() {

	}
}