namespace Engine.Transforms.Models;

public static class TransformExtensions {
	public static TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> GetInterpolated( this TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> a, TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> b, float interpolation )
		=> new( a.Translation.Lerp( b.Translation, interpolation ),
			a.Rotation.Lerp( b.Rotation, interpolation ),
			a.Scale.Lerp( b.Scale, interpolation ) );
	public static TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> GetExtrapolation( this TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> a, TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> b, float interpolation )
		=> new( a.Translation.Lerp( (a.Translation * 2) - b.Translation, interpolation ),
				b.Rotation.Lerp( a.Rotation, interpolation + 1 ),
				a.Scale.Lerp( (a.Scale * 2) - b.Scale, interpolation ) );

	public static Matrix4x4<float> GetMatrix( this TransformData<Vector3<float>, Rotor3<float>, Vector3<float>> transformData )
		=> Matrix.Create4x4.Scaling( transformData.Scale ) * Matrix.Create4x4.RotationFromRotor( transformData.Rotation ) * Matrix.Create4x4.Translation( transformData.Translation );


}
