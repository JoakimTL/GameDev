namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Extended kerning table</c><br/>
/// Contains extended kerning information for the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6kerx.html">Extended kerning table documentation</see>
/// </summary>
public sealed class KerxTable( FontTableHeader header ) : FontTable( header ) {
}