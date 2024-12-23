﻿using Engine.Module.Entities.Render;
using Engine.Module.Render.Entities.Providers;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Fonts.Shaders;
using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.Text.Services;

public sealed class TextLayoutProvider( FontMeshingService fontMeshingService, SceneInstanceProvider sceneInstanceProvider ) : DisposableIdentifiable, IRenderEntityServiceProvider {
	private readonly FontMeshingService _fontMeshingService = fontMeshingService;
	private readonly SceneInstanceProvider _sceneInstanceProvider = sceneInstanceProvider;

	private readonly DisposableList _disposableList = new();

	public TextLayout CreateLayout( string sceneName, uint layer ) {
		TextLayout layout = new( _sceneInstanceProvider.RequestSceneInstanceCollection<GlyphVertex, Entity2SceneData, GlyphShaderBundle>( sceneName, layer ), _fontMeshingService );
		_disposableList.Add( layout );
		return layout;
	}

	protected override bool InternalDispose() {
		_disposableList.Dispose();
		return true;
	}
}

