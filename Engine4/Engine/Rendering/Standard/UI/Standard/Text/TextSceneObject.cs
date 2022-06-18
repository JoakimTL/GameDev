using Engine.Rendering.Standard.UI.Standard.Text.Shaders;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.UI.Standard.Text;
public class TextSceneObject : ClosedSceneObject<Vertex2, TextGlyphRenderData> {
	
	private SceneInstanceData<TextGlyphRenderData> _sceneInstanceData;

	public TextSceneObject() {
		SetMesh( Resources.Render.Mesh2.Square );
		SetShaders( Resources.Render.Shader.Bundles.Get<DistanceFieldGlyphShader>() );
		SetSceneData( _sceneInstanceData = new SceneInstanceData<TextGlyphRenderData>( 512, 0 ) );
	}

	public void Resize(uint numGlyphs) {
		if ( numGlyphs < _sceneInstanceData.MaxInstances )
			return;
		_sceneInstanceData.Dispose();
		SetSceneData( _sceneInstanceData = new SceneInstanceData<TextGlyphRenderData>( numGlyphs, 0 ) );
	}

	public void SetData(TextGlyphRenderData[] data ) {
		if ( (uint) data.Length != _sceneInstanceData.MaxInstances )
			Resize( (uint) data.Length );
		_sceneInstanceData.SetInstances( 0, data );
	}

}
