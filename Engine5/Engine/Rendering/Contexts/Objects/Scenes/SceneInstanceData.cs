using Engine.Datatypes.Buffers;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public class SceneInstanceData : Identifiable, ISceneInstanceData
{
    private uint _instanceSizeBytes;
    private ISegmentedBufferSegment _dataSegment;
    public uint MaxInstances { get; private set; }
    public uint ActiveInstances { get; private set; }
    public uint DataOffset => (uint)(_dataSegment.OffsetBytes / _instanceSizeBytes);
    private bool _ownsData;

    public event Action? Changed;

    public SceneInstanceData(ISegmentedBufferSegment dataSegment, uint instanceSizeBytes, bool ownsData)
    {
        _instanceSizeBytes = instanceSizeBytes;
        _dataSegment = dataSegment;
        _dataSegment.OffsetChanged += OnOffsetChanged;
        _ownsData = ownsData;
        MaxInstances = (uint)(_dataSegment.SizeBytes / _instanceSizeBytes);
        ActiveInstances = 0;
    }

    public void SetSegment(ISegmentedBufferSegment dataSegment, uint instanceSizeBytes, bool ownsData)
    {
        if (dataSegment == _dataSegment)
            return;
        if (_ownsData && _dataSegment is not null)
        {
            _dataSegment.OffsetChanged -= OnOffsetChanged;
            _dataSegment.Dispose();
        }
        _instanceSizeBytes = instanceSizeBytes;
        _dataSegment = dataSegment;
        _dataSegment.OffsetChanged += OnOffsetChanged;
        _ownsData = ownsData;
        MaxInstances = (uint)(_dataSegment.SizeBytes / _instanceSizeBytes);
        Changed?.Invoke();
    }

    private void OnOffsetChanged(object segment, ulong newOffsetBytes) => Changed?.Invoke();

    public void SetActiveInstances(uint instances)
    {
        if (instances > MaxInstances)
        {
            this.LogWarning("Active instance count can't exceed max instance count!");
            return;
        }
        if (this.ActiveInstances == instances)
            return;
        this.ActiveInstances = instances;
        Changed?.Invoke();
    }

    public T GetInstance<T>(uint index) where T : unmanaged => this._dataSegment.ReadOne<T>(index * (uint)Marshal.SizeOf<T>());
    public T[] GetInstances<T>(uint startIndex, uint count) where T : unmanaged => this._dataSegment.Snapshot<T>(startIndex * (uint)Marshal.SizeOf<T>(), count);

    public void SetInstance<T>(uint index, T value) where T : unmanaged => this._dataSegment.Write(index * (uint)Marshal.SizeOf<T>(), value);
    public unsafe void SetInstances<T>(uint index, T* ptr, uint elementCount) where T : unmanaged => this._dataSegment.Write(index * (uint)Marshal.SizeOf<T>(), ptr, elementCount);
    public void SetInstances<T>(uint index, ReadOnlySpan<T> values) where T : unmanaged => this._dataSegment.Write(index * (uint)Marshal.SizeOf<T>(), values);
    public void SetInstances(uint offsetBytes, ReadOnlySpan<byte> data) => this._dataSegment.Write(offsetBytes, data);
    public void Remove<T>(uint startIndex, uint elementCount) where T : unmanaged => this._dataSegment.Write(startIndex * (uint)Marshal.SizeOf<T>(), this._dataSegment.Snapshot<T>((startIndex + elementCount) * (uint)Marshal.SizeOf<T>(), this.MaxInstances - (startIndex + elementCount)));

    public void Dispose() => this._dataSegment.Dispose();
}
