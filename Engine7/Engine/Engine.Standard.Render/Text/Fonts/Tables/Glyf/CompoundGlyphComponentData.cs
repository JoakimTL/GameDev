namespace Engine.Standard.Render.Text.Fonts.Tables.Glyf;

public sealed class CompoundGlyphComponentData( IGlyphData glyph ) {
	public IGlyphData Glyph { get; } = glyph;
	public Matrix2x2<float> TransformationMatrix { get; private set; } = Matrix2x2<float>.MultiplicativeIdentity;
	public Vector2<float> Offset { get; private set; } = Vector2<float>.Zero;

	public void Transform( double offsetX, double offsetY, double iHat_x, double iHat_y, double jHat_x, double jHat_y ) {
		this.TransformationMatrix = new( (float) iHat_x, (float) iHat_y, (float) jHat_x, (float) jHat_y );
		this.Offset = new( (float) offsetX, (float) offsetY );
	}
}