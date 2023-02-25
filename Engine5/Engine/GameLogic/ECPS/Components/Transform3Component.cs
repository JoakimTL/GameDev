using Engine.Datatypes;
using Engine.Datatypes.Transforms;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace Engine.GameLogic.ECPS.Components;
public sealed class Transform3Component : ComponentBase, ISerializable {
	public readonly Transform3 Transform;

	protected override string UniqueNameTag => $"{Transform}";

	public unsafe ulong SerializedLength => (uint) sizeof( TransformData<Vector3, Quaternion, Vector3> );

	public static Guid TypeIdentity { get; } = new( "58e72979-eba6-4cd5-913b-d5054db8a05b" );

	public Transform3Component() {
		Transform = new();
		Transform.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider obj ) => AlertComponentChanged();
	protected override void OnDispose() => Transform.MatrixChanged -= OnMatrixChanged;

	public bool DeserializeData( byte[] data ) {
		unsafe {
			if ( (uint) data.Length < sizeof( TransformData<Vector3, Quaternion, Vector3> ) )
				return Log.WarningThenReturn( $"Length of data must be equal to or longer than {sizeof( TransformData<Vector3, Quaternion, Vector3> )}", false );
			fixed ( byte* srcPtr = data )
				Transform.SetData( *(TransformData<Vector3, Quaternion, Vector3>*) srcPtr );
		}
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			byte[] data = new byte[ sizeof( TransformData<Vector3, Quaternion, Vector3> ) ];
			fixed ( byte* dstPtr = data )
				*(TransformData<Vector3, Quaternion, Vector3>*) dstPtr = Transform.Adjusted ? Transform.GlobalData : Transform.Data;
			return data;
		}
	}
}
