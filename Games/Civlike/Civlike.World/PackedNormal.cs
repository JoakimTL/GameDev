using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public readonly struct PackedNormal {
	private readonly uint _data;

	private const int _bitsPerComponent = 16;
	private const uint _maxValue = (1u << _bitsPerComponent) - 1;
	private const float _invMaxValue = 1f / _maxValue;

	public PackedNormal( Vector3<float> unit ) {
		// 1) Project to octahedron and fold
		float l1Norm = MathF.Abs( unit.X ) + MathF.Abs( unit.Y ) + MathF.Abs( unit.Z );

		Vector2<float> xy = new Vector2<float>( unit.X, unit.Y ) / l1Norm;

		Vector2<float> res = xy;
		if (unit.Z < 0f) 			res = new(
				(1f - MathF.Abs( xy.Y )) * MathF.CopySign( 1, unit.X ),
				(1f - MathF.Abs( xy.X )) * MathF.CopySign( 1, unit.Y )
			);

		// 2) Quantize from [-1,1] to [0,MaxValue]
		uint ux = (uint) MathF.Round( (res.X * 0.5f + 0.5f) * _maxValue );
		uint uy = (uint) MathF.Round( (res.Y * 0.5f + 0.5f) * _maxValue );

		// 3) Pack into 32 bits: high 16 = ux, low 16 = uy
		this._data = ux << _bitsPerComponent | uy;
	}

	public Vector3<float> Decode() {
		// 1) Unpack
		uint ux = this._data >> _bitsPerComponent;
		uint uy = this._data & _maxValue;

		// 2) Dequantize back to [-1,1]
		float x = ux * _invMaxValue * 2f - 1f;
		float y = uy * _invMaxValue * 2f - 1f;

		// 3) Reconstruct z from the octahedral mapping
		float z = 1f - (MathF.Abs( x ) + MathF.Abs( y ));
		if (z < 0f) {
			float oldX = x, oldY = y;
			x = (1f - MathF.Abs( oldY )) * MathF.CopySign( 1, oldX );
			y = (1f - MathF.Abs( oldX )) * MathF.CopySign( 1, oldY );
			// z = -z; // optional, but this ends up restoring the correct hemisphere
		}

		// 4) Normalize (optional, to correct tiny quantization drift)
		return new Vector3<float>( x, y, z ).Normalize<Vector3<float>, float>();
	}

	// Expose the packed data if you need to write it out:
	public uint RawData => this._data;
}