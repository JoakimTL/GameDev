using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine;

namespace TestPlatform.Voxels2.Data;
public unsafe class VoxelChunkData : DisposableIdentifiable {
	private ushort* _voxelData;
	private uint _bytesUsed;

	public VoxelChunkData( uint lodLevel ) {
		this.LodLevel = lodLevel;
		this.VoxelSize = Math.Min( 1u << (int) lodLevel, VoxelChunk.Length );
		this.ActualLength = VoxelChunk.Length / this.VoxelSize;
		this.ActualArea = this.ActualLength * this.ActualLength;
		this.ActualVolume = this.ActualArea * this.ActualLength;
		this._bytesUsed = this.ActualVolume * sizeof( ushort );
		this._voxelData = (ushort*) NativeMemory.AllocZeroed( this._bytesUsed );
		this.Initialized = false;
		this.AllAir = true;
	}

	public uint ActualLength { get; private set; }
	public uint ActualArea { get; private set; }
	public uint ActualVolume { get; private set; }
	public uint LodLevel { get; private set; }
	public uint VoxelSize { get; private set; }
	public bool Initialized { get; private set; }
	public bool Freed { get; private set; }
	public bool AllAir { get; private set; }
	public int Users { get; private set; }
	public ReadOnlySpan<ushort> Ids => new( this._voxelData, (int) this.ActualVolume );

	internal void AddUser() {
		if ( this.Disposed )
			this.LogWarning( "Already disposed!" );
		this.Users++;
	}
	internal void RemoveUser() {
		this.Users--;
		if ( this.Disposed )
			Free();
	}

	private void Free() {
		if ( this.Users == 0 && !this.Freed ) {
			this.Freed = true;
			NativeMemory.Free( this._voxelData );
		}
	}

	internal ushort GetIdFromIndex( uint index ) {
		if ( index >= this._bytesUsed )
			throw new ArgumentOutOfRangeException( nameof( index ) );
		return this._voxelData[ index ];
	}

	internal bool SetIdFromIndex( uint index, ushort id ) {
		if ( index >= this._bytesUsed )
			throw new ArgumentOutOfRangeException( nameof( index ) );
		ushort prevId = this._voxelData[ index ];
		this._voxelData[ index ] = id;
		return prevId != id;
	}

	internal void Initialize( ReadOnlySpan<ushort> ids ) {
		if ( ids.Length != this.ActualVolume ) {
			this.LogWarning( "Length of init data and chunk volume does not match. This can be ignored if everything seems fine." );
			return;
		}
		if ( this.Initialized ) {
			this.LogWarning( "Chunk has already been initialized." );
			return;
		}
		if ( this.Freed )
			return;
		fixed ( ushort* srcPtr = ids ) {
			Unsafe.CopyBlock( this._voxelData, srcPtr, this._bytesUsed );
			this.AllAir = true;
			for ( int i = 0; i < this.ActualVolume; i++ ) {
				if ( srcPtr[ i ] != 0 ) {
					this.AllAir = false;
					break;
				}
			}
		}
		this.Initialized = true;
	}

	protected override bool OnDispose() {
		Free();
		return true;
	}
}
