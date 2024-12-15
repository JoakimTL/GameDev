using Engine.Module.Entities.Render;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts;
using Engine.Standard.Render.Text.Fonts.Meshing;
using Engine.Standard.Render.Text.Fonts.Shaders;
using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.Text;

public sealed class FontService {

	public string FontBasePath { get; set; } = ".\\assets\\fonts";
	private readonly Dictionary<string, Font> _fonts = [];

	/// <summary>
	/// Throws a <see cref="FileNotFoundException"/> if the font file is not found. The font file is found in the <see cref="FontBasePath"/> directory, and must have the extension ".ttf".
	/// </summary>
	/// <exception cref="FileNotFoundException"></exception>
	public Font Get( string fontName ) {
		if (this._fonts.TryGetValue( fontName, out Font? font ))
			return font;

		string path = Path.Combine( this.FontBasePath, $"{fontName}.ttf" );
		if (!File.Exists( path ))
			throw new FileNotFoundException( "Font file not found.", path );

		font = new Font( path );
		this._fonts.Add( fontName, font );
		return font;
	}
}
public sealed class FontMeshingService( FontService fontService, MeshService meshService ) {

	public string FontBasePath { get; set; } = ".\\assets\\fonts";
	private readonly Dictionary<string, MeshedFont> _meshedFonts = [];
	private readonly FontService _fontService = fontService;
	private readonly MeshService _meshService = meshService;

	/// <summary>
	/// Throws a <see cref="FileNotFoundException"/> if the font file is not found. The font file is found in the <see cref="FontBasePath"/> directory, and must have the extension ".ttf".
	/// </summary>
	/// <exception cref="FileNotFoundException"></exception>
	public MeshedFont Get( string fontName ) {
		if (this._meshedFonts.TryGetValue( fontName, out MeshedFont? meshedFont ))
			return meshedFont;

		Font font = this._fontService.Get( fontName );
		meshedFont = new MeshedFont( font, _meshService );
		this._meshedFonts.Add( fontName, meshedFont );
		return meshedFont;
	}
}

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

