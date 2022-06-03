using System.Numerics;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;
public class DefaultVoxelWorldGenerator : IVoxelWorldGenerator {

	private readonly DistanceField3MapGenerator _underground;
	private readonly DistanceField2MapGenerator _heightmap;
	private readonly DistanceField2MapGenerator _groundlayermap;
	private readonly float _amplitude;
	private readonly float _baseline;
	private readonly float _depthFactor;
	private readonly float _minLine;
	private readonly float _minAmplitude;

	public DefaultVoxelWorldGenerator( Vector3 spread3, Vector2 spread2, float amplitude, float baseline, float depthFactor, float minLine, float minAmplitude ) {
		this._underground = new DistanceField3MapGenerator( 23, spread3 );
		this._heightmap = new DistanceField2MapGenerator( 43, spread2 );
		this._groundlayermap = new DistanceField2MapGenerator( 88, spread2 );
		this._amplitude = amplitude;
		this._baseline = baseline;
		this._depthFactor = depthFactor;
		this._minLine = minLine;
		this._minAmplitude = minAmplitude;
	}

	public ushort GetId( Vector3i worldVoxelCoordinate ) {
		float height = (this._heightmap.GetValue( worldVoxelCoordinate.XZ.AsFloat ) * this._amplitude) + this._baseline;
		if ( worldVoxelCoordinate.Y > height )
			return 0;
		float underground = this._underground.GetValue( worldVoxelCoordinate.AsFloat ) * this._depthFactor * ( height - worldVoxelCoordinate.Y );
		if ( underground < 0.5f )
			return 0;
		float groundLayer = (this._groundlayermap.GetValue( worldVoxelCoordinate.XZ.AsFloat ) * this._minAmplitude) + this._minLine;
		if ( worldVoxelCoordinate.Y < groundLayer )
			return Voxel.Clay.Id;
		return Voxel.Stone.Id;
	}

}
