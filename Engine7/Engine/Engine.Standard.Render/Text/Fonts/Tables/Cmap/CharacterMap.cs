namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public sealed class CharacterMap( SpecificPlatform specificPlatform, uint offset ) {
	public SpecificPlatform SpecificPlatform = specificPlatform;
	public uint Offset = offset;

	public Platform Platform => (Platform) ((ushort) this.SpecificPlatform & 0b1111);
}
