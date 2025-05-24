namespace Engine.Standard.Render.Text.Fonts.Tables;

/// <summary>
/// <c>Maximum profile table</c><br/>
/// Contains information about the maximum profile of the font.<br/>
/// <br/>
/// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6maxp.html">Maximum profile table documentation</see>
/// </summary>
public sealed class MaxPTable : FontTable {

	public ushort MaxGlyphs { get; }
	public ushort MaxPoints { get; }
	public ushort MaxContours { get; }
	public ushort MaxComponentPoints { get; }
	public ushort MaxComponentContours { get; }
	public ushort MaxZones { get; }
	public ushort MaxTwilightPoints { get; }
	public ushort MaxStorage { get; }
	public ushort MaxFunctionDefs { get; }
	public ushort MaxInstructionDefs { get; }
	public ushort MaxStackElements { get; }
	public ushort MaxSizeOfInstructions { get; }
	public ushort MaxComponentElements { get; }
	public ushort MaxComponentDepth { get; }

	public MaxPTable( FontTableHeader header, FontDataReader reader ) : base( header ) {
		FontCaretedDataReader caret = new( reader );
		caret.GoTo( header.Offset );
		caret.MoveCaretBy( 4 ); //version
		this.MaxGlyphs = caret.Read<ushort>();
		this.MaxPoints = caret.Read<ushort>();
		this.MaxContours = caret.Read<ushort>();
		this.MaxComponentPoints = caret.Read<ushort>();
		this.MaxComponentContours = caret.Read<ushort>();
		this.MaxZones = caret.Read<ushort>();
		this.MaxTwilightPoints = caret.Read<ushort>();
		this.MaxStorage = caret.Read<ushort>();
		this.MaxFunctionDefs = caret.Read<ushort>();
		this.MaxInstructionDefs = caret.Read<ushort>();
		this.MaxStackElements = caret.Read<ushort>();
		this.MaxSizeOfInstructions = caret.Read<ushort>();
		this.MaxComponentElements = caret.Read<ushort>();
		this.MaxComponentDepth = caret.Read<ushort>();
	}
}
