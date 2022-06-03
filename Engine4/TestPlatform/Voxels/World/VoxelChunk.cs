using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;
public unsafe sealed class VoxelChunk : DisposableIdentifiable {

	public const uint SideLength = 16;
	public const uint FaceArea = SideLength * SideLength;
	public const uint Volume = SideLength * SideLength * SideLength;
	public const uint BytesUsed = Volume * sizeof( ushort );

	public readonly Vector3i Translation;
	private readonly VoxelWorldBase _world;
	private AABB3i _aabb;
	private readonly ushort* _voxelData;

	public event Action<Vector3i>? VoxelChanged;
	public event Action? VoxelRenderChange;

	protected override string UniqueNameTag => this.Translation.ToString();

	internal VoxelChunk( VoxelWorldBase world, Vector3i translation ) {
		this._world = world;
		this.Translation = translation;
		this._aabb = new AABB3i( translation, translation + ( (int) SideLength - 1 ) );
		this._voxelData = (ushort*) NativeMemory.AllocZeroed( BytesUsed );
	}

	private VoxelChunk( VoxelWorldBase world, Vector3i translation, byte[] data ) : this( world, translation ) {
		fixed ( byte* ptr = data ) {
			Unsafe.CopyBlock( this._voxelData, ptr, BytesUsed );
		}
	}

	private VoxelChunk( VoxelWorldBase world, Vector3i translation, byte* data ) : this( world, translation ) {
		Unsafe.CopyBlock( this._voxelData, data, BytesUsed );
	}

	public static uint GetIndex( Vector3i local ) => (uint) ( ( local.Y * FaceArea ) + ( local.Z * SideLength ) + local.X );
	public ushort GetId( Vector3i local ) => this._voxelData[ GetIndex( local ) ];
	public bool SetId( Vector3i local, ushort id ) => SetId( GetIndex( local ), id);
	public ushort GetId( uint index ) => this._voxelData[ index ];
	public bool SetId( uint index, ushort id ) {
		ushort oldId = this._voxelData[ index ];
		this._voxelData[ index ] = id;
		return oldId != id;
	}

	public Vector3i ToLocal( Vector3i global ) => global - this.Translation;
	public Vector3i ToGlobal( Vector3i local ) => local + this.Translation;

	internal void TriggerVoxelChange( Vector3i global ) {
		if ( AABB3i.Inside( ref this._aabb, ref global ) ) {
			VoxelChanged?.Invoke( ToLocal( global ) );
		} else {
			VoxelChunk? neighbour = this._world.GetChunk( global );
			if ( neighbour is not null )
				neighbour.TriggerVoxelChange( global );
		}
	}
	internal void TriggerVoxelChangeNoCheck( Vector3i local ) => VoxelChanged?.Invoke( local );
	internal void TriggerVoxelRenderChange() => VoxelRenderChange?.Invoke();

	public bool AreNeighboursAvailable() =>
		this._world.HasGenerated( this.Translation - new Vector3i( (int) SideLength, 0, 0 ) ) &&
		this._world.HasGenerated( this.Translation - new Vector3i( 0, (int) SideLength, 0 ) ) &&
		this._world.HasGenerated( this.Translation - new Vector3i( 0, 0, (int) SideLength ) ) &&
		this._world.HasGenerated( this.Translation + new Vector3i( (int) SideLength, 0, 0 ) ) &&
		this._world.HasGenerated( this.Translation + new Vector3i( 0, (int) SideLength, 0 ) ) &&
		this._world.HasGenerated( this.Translation + new Vector3i( 0, 0, (int) SideLength ) );

	public string GetSavename() => GetSavename( this.Translation );
	public void Save( string worldName ) {
		byte[] data = new byte[ BytesUsed ];
		fixed ( byte* ptr = data ) {
			Unsafe.CopyBlock( ptr, this._voxelData, BytesUsed );
		}
		string path = $"{worldName}/{GetSavename()}";
		if ( !Directory.Exists( worldName ) )
			Directory.CreateDirectory( worldName );
		File.WriteAllBytes( $"{worldName}/{GetSavename()}", data );
	}

	public static string GetSavename( Vector3i translation ) => $"{translation.X:X8}{translation.Y:X8}{translation.Z:X8}.chk";
	public static bool ParseSavename( string savename, out Vector3i translation ) {
		string name = Path.GetFileName( savename );
		translation = new Vector3i( 0, 0, 0 );
		if ( name.Length != 3 * 8 * 2 )
			return false;
		bool valid = true;
		valid &= int.TryParse( name[ 0..8 ], System.Globalization.NumberStyles.HexNumber, null, out int tX );
		valid &= int.TryParse( name[ 8..16 ], System.Globalization.NumberStyles.HexNumber, null, out int tY );
		valid &= int.TryParse( name[ 16..24 ], System.Globalization.NumberStyles.HexNumber, null, out int tZ );
		if ( !valid )
			return false;
		translation = new Vector3i( tX, tY, tZ );
		return true;
	}

	public static bool TryLoad( VoxelWorldBase world, string saveName, out VoxelChunk? chunk ) {
		string path = $"{world.Name}/{saveName}";
		chunk = null;
		if ( !File.Exists( path ) )
			return false;

		try {
			if ( !ParseSavename( saveName, out Vector3i translation ) )
				return false;
			byte[] data = File.ReadAllBytes( path );
			fixed ( byte* dataPtr = data )
				return CreateFromWorldGenData( world, translation, dataPtr, (uint) data.Length, out chunk );
		} catch ( IOException ex ) {
			Log.Error( ex );
			return false;
		} catch ( Exception ex ) {
			Log.Error( ex );
			return false;
		}
	}

	public static bool CreateFromWorldGenData( VoxelWorldBase world, Vector3i translation, ushort[] data, out VoxelChunk? chunk ) {
		fixed ( ushort* dataPtr = data )
			return CreateFromWorldGenData( world, translation, (byte*) dataPtr, (uint) data.Length * sizeof( ushort ), out chunk );
	}

	public static bool CreateFromWorldGenData( VoxelWorldBase world, Vector3i translation, byte* data, uint dataLength, out VoxelChunk? chunk ) {
		chunk = null;
		if ( dataLength != BytesUsed )
			return false;
		chunk = new VoxelChunk( world, translation, data );
		return true;
	}

	protected override bool OnDispose() {
		NativeMemory.Free( this._voxelData );
		return true;
	}
}
