﻿using Engine.Standard.Render.Text.Fonts;

namespace Engine.Standard.Render.Text.Services;

public sealed class OldFontService {

	public string FontBasePath { get; set; } = ".\\assets\\fonts";
	private readonly Dictionary<string, OldFont> _fonts = [];

	/// <summary>
	/// Throws a <see cref="FileNotFoundException"/> if the font file is not found. The font file is found in the <see cref="FontBasePath"/> directory, and must have the extension ".ttf".
	/// </summary>
	/// <exception cref="FileNotFoundException"></exception>
	public OldFont Get( string fontName ) {
		if (this._fonts.TryGetValue( fontName, out OldFont? font ))
			return font;

		string path = Path.Combine( this.FontBasePath, $"{fontName}.ttf" );
		if (!File.Exists( path ))
			throw new FileNotFoundException( "Font file not found.", path );

		font = new OldFont( path );
		this._fonts.Add( fontName, font );
		return font;
	}
}

