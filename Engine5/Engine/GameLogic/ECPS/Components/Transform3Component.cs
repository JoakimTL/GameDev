using Engine.Datatypes;
using Engine.Datatypes.Transforms;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace Engine.GameLogic.ECPS.Components;
public sealed class Transform3Component : ComponentBase, ICustomizedSerializable
{
	public readonly Transform3 Transform;

	protected override string UniqueNameTag => $"{Transform}";

	public static Guid SerializationIdentity { get; } = new("58e72979-eba6-4cd5-913b-d5054db8a05b");
	public bool ShouldSerialize => true;

	public Transform3Component()
	{
		Transform = new();
		Transform.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged(IMatrixProvider obj) => AlertComponentChanged();
	protected override void OnDispose() => Transform.MatrixChanged -= OnMatrixChanged;
	protected override void OnOwnerChanged() => Transform.SetParent(Owner?.Parent?.Get<Transform3Component>()?.Transform, true);
	protected override void OnOwnerParentChanged(Entity owner, Entity? parent) => Transform.SetParent(parent?.Get<Transform3Component>()?.Transform, true);

	public bool DeserializeData(byte[] data)
	{
		unsafe
		{
			if ((uint)data.Length < sizeof(TransformData<Vector3, Quaternion, Vector3>))
				return Log.WarningThenReturn($"Length of data must be equal to or longer than {sizeof(TransformData<Vector3, Quaternion, Vector3>)}", false);
			fixed (byte* srcPtr = data)
				Transform.SetData(*(TransformData<Vector3, Quaternion, Vector3>*)srcPtr);
		}
		return true;
	}

	public byte[] SerializeData()
	{
		unsafe
		{
			byte[] data = new byte[sizeof(TransformData<Vector3, Quaternion, Vector3>)];
			fixed (byte* dstPtr = data)
				*(TransformData<Vector3, Quaternion, Vector3>*)dstPtr = Transform.Adjusted ? Transform.GlobalData : Transform.Data;
			return data;
		}
	}
}
