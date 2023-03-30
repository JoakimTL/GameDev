using Engine;
using Engine.Datatypes;
using Engine.GameLogic.ECPS;
using Engine.GlobalServices;
using Engine.Rendering;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StandardPackage.ECPS.Components;

public sealed class RenderInstanceDataComponent : ComponentBase, ICustomizedSerializable, IRenderableInstanceData
{
	public static Guid SerializationIdentity { get; } = new("fc52578f-949c-464b-86ec-80bbb2a14805");
	public bool ShouldSerialize => true;

	public Type? InstanceDataType { get; private set; }

	private string? _instanceTypeIdentity;
	private byte[]? _instanceData;

	public void SetData<T>(Span<T> data) where T : unmanaged
	{
		if (typeof(T) != InstanceDataType)
		{
			InstanceDataType = typeof(T);
			_instanceTypeIdentity = InstanceDataType.GetCustomAttribute<IdentityAttribute>()?.Identity;
		}
		if (_instanceTypeIdentity == null)
		{
			_instanceData = Array.Empty<byte>();
			return;
		}
		int expectedLength = data.Length * Marshal.SizeOf<T>();
		if (_instanceData is null)
			_instanceData = new byte[expectedLength];
		if (expectedLength > _instanceData.Length)
			_instanceData = new byte[expectedLength];
		data.CopyInto(_instanceData);
		AlertComponentChanged();
	}

	public bool DeserializeData(byte[] data)
	{
		var segments = Segmentation.Parse(data);
		if (segments is null)
			return false;
		var newTypeIdentity = segments[0].CreateString();
		if (newTypeIdentity != _instanceTypeIdentity)
		{
			_instanceTypeIdentity = newTypeIdentity;
			InstanceDataType = Global.Get<IdentityTypeService>().GetFromIdentity(_instanceTypeIdentity);
		}
		_instanceData = segments[1];
		AlertComponentChanged();
		return true;
	}

	public byte[] SerializeData()
	{
		if (_instanceTypeIdentity is null || _instanceData is null)
			return Array.Empty<byte>();
		return Segmentation.Segment(_instanceTypeIdentity.ToBytes(), _instanceData);
	}

	public byte[] GetData() => _instanceData ?? Array.Empty<byte>();
}