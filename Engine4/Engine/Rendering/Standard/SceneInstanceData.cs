using System.Runtime.InteropServices;
using Engine.Data.Buffers;

namespace Engine.Rendering.Standard;
public class SceneInstanceData<SD> : DisposableIdentifiable where SD : unmanaged {

	private readonly RenderDataObject _rdo;
	protected readonly IDataSegment _dataSegment;
	public IDataSegmentInformation SegmentData => this._dataSegment;
	public uint MaxInstances { get; }
	public uint ActiveInstances { get; private set; }
	public event Action? ActiveInstanceCountChanged;

	public SceneInstanceData( uint instanceCount, uint activeInstances ) {
		this._rdo = Resources.Render.RDOs.Get<SD>();
		this._dataSegment = this._rdo.Buffer.AllocateSynchronized( instanceCount * (uint) Marshal.SizeOf<SD>() );
		this.MaxInstances = instanceCount;
		this.ActiveInstances = activeInstances;
	}

	public void SetActiveInstances( uint instances ) {
		if ( instances > this._dataSegment.SizeBytes / (uint) Marshal.SizeOf<SD>() ) {
			this.LogWarning( "Active instance count can't exceed max instance count!" );
			return;
		}
		if ( this.ActiveInstances == instances )
			return;
		this.ActiveInstances = instances;
		ActiveInstanceCountChanged?.Invoke();
	}

	public SD this[ uint index ] {
		get => GetInstance( index );
		set => SetInstance( index, value );
	}

	public SD GetInstance( uint index ) => this._dataSegment.Read<SD>( index * (uint) Marshal.SizeOf<SD>() );
	public ReadOnlySpan<SD> GetInstances( uint startIndex, uint count ) => this._dataSegment.Read<SD>( startIndex * (uint) Marshal.SizeOf<SD>(), count );

	public void SetInstance( uint index, SD value ) => this._dataSegment.Write( index * (uint) Marshal.SizeOf<SD>(), value );
	public void SetInstances( uint index, Span<SD> values ) => this._dataSegment.Write( index * (uint) Marshal.SizeOf<SD>(), values );
	public void SetInstances( uint index, ReadOnlySpan<SD> values ) => this._dataSegment.Write( index * (uint) Marshal.SizeOf<SD>(), values );

	public void Remove( uint startIndex, uint elementCount )
		=> this._dataSegment.Write( startIndex * (uint) Marshal.SizeOf<SD>(), this._dataSegment.Read<SD>( ( startIndex + elementCount ) * (uint) Marshal.SizeOf<SD>(), this.MaxInstances - ( startIndex + elementCount ) ) );

	protected override bool OnDispose() {
		this._dataSegment.Dispose();
		return true;
	}
}
