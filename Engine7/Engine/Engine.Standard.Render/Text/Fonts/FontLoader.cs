using System.Runtime.InteropServices;

namespace Engine.Standard.Render.Text.Fonts;

public unsafe sealed class FontLoader : DisposableIdentifiable {

	public const uint Tag_Cmap = 1885433187;
	public const uint Tag_Glyf = 1719233639;
	public const uint Tag_Head = 1684104552;
	public const uint Tag_Loca = 1633906540;
	public const uint Tag_Maxp = 1886937453;
	public const uint Tag_Hhea = 1634035816;
	public const uint Tag_Hmtx = 2020896104;
	public const uint Tag_Kern = 1801810542;
	public const uint Tag_Kerx = 1801810552;

	private readonly byte* _fontDataPtr;
	private readonly FontDataReader _dataReader;
	private readonly FontLoaderHeaderData _fontHeaderData;
	private readonly Tables.HeadTable _headTable;
	private readonly Tables.MaxPTable _maxpTable;
	private readonly Tables.LocaTable _locaTable;
	private readonly Tables.CmapTable _cmapTable;
	private readonly Tables.GlyfTable _glyfTable;
	private readonly Tables.HheaTable _hheaTable;
	private readonly Tables.HmtxTable _hmtxTable;
	private readonly Tables.KerxTable? _kerxTable;
	private readonly Tables.KernTable? _kernTable;

	public FontLoader( string fontFilePath ) {
		if (Path.GetExtension( fontFilePath ) != ".ttf")
			throw new Exception( "Font file must be a TrueType font (.ttf)" );
		this.FontName = Path.GetFileNameWithoutExtension( fontFilePath );
		byte[] fontDataByteArray = File.ReadAllBytes( fontFilePath );
		this._fontDataPtr = (byte*) NativeMemory.Alloc( (nuint) fontDataByteArray.Length );
		Marshal.Copy( fontDataByteArray, 0, (nint) this._fontDataPtr, fontDataByteArray.Length );
		this._dataReader = new( this._fontDataPtr, fontDataByteArray.Length, false );
		this._fontHeaderData = new( this._dataReader );
		this._headTable = new( this._fontHeaderData.Tables[ Tag_Head ], this._dataReader );
		this._maxpTable = new( this._fontHeaderData.Tables[ Tag_Maxp ], this._dataReader );
		this._locaTable = new( this._fontHeaderData.Tables[ Tag_Loca ], this._headTable, this._maxpTable, this._dataReader );
		this._cmapTable = new( this._fontHeaderData.Tables[ Tag_Cmap ], this._dataReader );
		this._glyfTable = new( this._fontHeaderData.Tables[ Tag_Glyf ], this._locaTable, this._cmapTable, this._dataReader );
		this._hheaTable = new( this._fontHeaderData.Tables[ Tag_Hhea ], this._dataReader );
		this._hmtxTable = new( this._fontHeaderData.Tables[ Tag_Hmtx ], this._locaTable, this._hheaTable, this._dataReader );

		//if (_fontHeaderData.Tables.ContainsKey( Tag_Kerx )) { //TODO: Implement KerxTable and KernTable
		//	_kerxTable = new( _fontHeaderData.Tables[ Tag_Kerx ], _dataReader );
		//} else if (_fontHeaderData.Tables.ContainsKey( Tag_Kern )) {
		//	_kernTable = new( _fontHeaderData.Tables[ Tag_Kern ], _dataReader );
		//}
	}

	public string FontName { get; }
	/// <summary>
	/// Defines general information about the font such as <see cref="HeadTable.UnitsPerEm"/>"/>
	/// </summary>
	public Tables.HeadTable HeadTable => this._headTable;
	/// <summary>
	/// Defines the glyphs in the font and their outlines
	/// </summary>
	public Tables.GlyfTable GlyfTable => this._glyfTable;
	/// <summary>
	/// Defines the general horizontal metrics of the font
	/// </summary>
	public Tables.HheaTable HheaTable => this._hheaTable;
	/// <summary>
	/// Defines the horizontal metrics of the glyphs in the font
	/// </summary>
	public Tables.HmtxTable HmtxTable => this._hmtxTable;

	protected override bool InternalDispose() {
		NativeMemory.Free( this._fontDataPtr );
		this._dataReader.Dispose();
		return true;
	}
}
