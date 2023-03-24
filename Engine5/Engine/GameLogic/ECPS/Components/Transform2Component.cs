using Engine.Datatypes;
using Engine.Datatypes.Transforms;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace Engine.GameLogic.ECPS.Components;

public sealed class Transform2Component : ComponentBase, ICustomizedSerializable {
	public readonly Transform2 Transform;

	protected override string UniqueNameTag => $"{Transform}";

	public static Guid SerializationIdentity { get; } = new( "27fef5c2-128a-4422-8d48-38d594112cc9" );
    public bool ShouldSerialize => true;

    public Transform2Component() {
		Transform = new();
		Transform.MatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider obj )
		=> AlertComponentChanged();
	protected override void OnDispose()
		=> Transform.MatrixChanged -= OnMatrixChanged;
	protected override void OnOwnerChanged()
		=> Transform.SetParent( Owner?.Parent?.Get<Transform2Component>()?.Transform, true );

	public bool DeserializeData( byte[] data ) {
		unsafe {
			if ( (uint) data.Length < sizeof( TransformData<Vector2, float, Vector2> ) )
				return Log.WarningThenReturn( $"Length of data must be equal to or longer than {sizeof( TransformData<Vector2, float, Vector2> )}", false );
			fixed ( byte* srcPtr = data )
				Transform.SetData( *(TransformData<Vector2, float, Vector2>*) srcPtr );
		}
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			byte[] data = new byte[ sizeof( TransformData<Vector2, float, Vector2> ) ];
			fixed ( byte* dstPtr = data )
				*(TransformData<Vector2, float, Vector2>*) dstPtr = Transform.Adjusted ? Transform.GlobalData : Transform.Data;
			return data;
		}
	}
}
