using Engine.Module.Render.Ogl.Scenes.Services;
using Engine.Standard.Render.Text.Fonts;
using Engine.Standard.Render.Text.Fonts.Meshing;

namespace Engine.Standard.Render.Text.Services;

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

