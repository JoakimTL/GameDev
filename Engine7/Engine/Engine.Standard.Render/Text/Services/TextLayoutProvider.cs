using Engine.Module.Render.Ogl.Providers;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Fonts.Shaders;
using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.Text.Services;

public sealed class TextLayoutProvider( FontMeshingService fontMeshingService, SceneInstanceProvider sceneInstanceProvider ) : Identifiable, IServiceProvider {
	private readonly FontMeshingService _fontMeshingService = fontMeshingService;
	private readonly SceneInstanceProvider _sceneInstanceProvider = sceneInstanceProvider;

	public TextLayout CreateLayout( string sceneName, uint layer )
		=> new( this._sceneInstanceProvider.RequestSceneInstanceCollection<GlyphVertex, Entity2SceneData, GlyphShaderBundle>( sceneName, layer ), this._fontMeshingService );
}

