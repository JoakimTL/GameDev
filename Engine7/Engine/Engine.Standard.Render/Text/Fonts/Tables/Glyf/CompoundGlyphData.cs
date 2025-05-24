namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public sealed class CompoundGlyphData( FontGlyphHeader header, GlyphMap glyphMap ) : IGlyphData {
	public FontGlyphHeader Header { get; } = header;
	public GlyphMap Mapping { get; } = glyphMap;
	private readonly List<CompoundGlyphComponentData> _componentGlyphData = [];

	public void AddComponentGlyph( CompoundGlyphComponentData componentGlyph ) => this._componentGlyphData.Add( componentGlyph );

	public IReadOnlyList<CompoundGlyphComponentData> ComponentGlyphData => this._componentGlyphData;
}
