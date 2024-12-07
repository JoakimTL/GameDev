using System.Text;

namespace Engine.Standard.Render.Text;

public readonly struct FontTable( uint tag, uint checksum, uint offset, uint length ) {
	//https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6.html
	public readonly uint Tag = tag;
	public readonly uint Checksum = checksum;
	public readonly uint Offset = offset;
	public readonly uint Length = length;
	public string TagString => Encoding.ASCII.GetString( BitConverter.GetBytes( this.Tag ) );
	public FontTable ProperEndianness => new( this.Tag, this.Checksum.FromBigEndian(), this.Offset.FromBigEndian(), this.Length.FromBigEndian() );

	public override string ToString() => $"Font Table {this.TagString}";
}
