using System.Runtime.InteropServices;

namespace Engine.Datatypes.Transforms;

[StructLayout( LayoutKind.Sequential )]
public readonly struct TransformData<T, R, S>
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {
	public readonly T Translation;
	public readonly R Rotation;
	public readonly S Scale;
	public TransformData( T translation, R rotation, S scale ) {
		Translation = translation;
		Rotation = rotation;
		Scale = scale;
	}
}