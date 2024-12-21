namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Kerning table</c><br/>
/// Contains kerning information for the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6kern.html">Kerning table documentation</see>
/// </summary>
public sealed class KernTable( FontTableHeader header ) : FontTable( header ) {
}
