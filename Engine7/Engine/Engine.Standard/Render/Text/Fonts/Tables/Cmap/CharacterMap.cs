namespace Engine.Standard.Render.Text.Fonts.Tables.Cmap;

public sealed class CharacterMap {
	public SpecificPlatform SpecificPlatform;
	public uint Offset;

	public CharacterMap( SpecificPlatform specificPlatform, uint offset ) {
		SpecificPlatform = specificPlatform;
		Offset = offset;
	}

	public Platform Platform => (Platform) ((ushort) SpecificPlatform & 0b1111);
}
