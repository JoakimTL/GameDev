namespace Engine.Standard.Render.Text;

public readonly struct FontGlyphHeader( short numberOfContours, short xMin, short yMin, short xMax, short yMax ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6glyf.html
	public readonly short NumberOfContours = numberOfContours;
	public readonly short XMin = xMin;
	public readonly short YMin = yMin;
	public readonly short XMax = xMax;
	public readonly short YMax = yMax;
	public FontGlyphHeader ProperEndianness => new( this.NumberOfContours.FromBigEndian(), this.XMin.FromBigEndian(), this.YMin.FromBigEndian(), this.XMax.FromBigEndian(), this.YMax.FromBigEndian() );
}
