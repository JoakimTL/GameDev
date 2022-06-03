using System.Numerics;
using Engine;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TestPlatform.HeightmapTerrain;
public class FiniteHeightmap : Identifiable {

	/// <summary>
	/// Pixel data
	/// </summary>
	private readonly float[] _heights;
	private readonly int _width;
	private readonly int _height;
	private readonly float _metersPerPixel;

	public FiniteHeightmap( float metersPerPixel, float minHeight, float maxHeight, string filepath ) {
		this._metersPerPixel = metersPerPixel;
		using ( Image<Rgba32>? img = Image.Load<Rgba32>( filepath ) ) {
			this._width = img.Width;
			this._height = img.Height;
			this._heights = new float[ img.Width * img.Height ];
			for ( int y = 0; y < img.Height; y++ ) {
				Span<Rgba32> row = img.GetPixelRowSpan( y );
				for ( int x = 0; x < img.Width; x++ ) {
					Vector4 vec4 = row[ x ].ToVector4();
					this._heights[ ( y * img.Width ) + x ] = ( ( vec4.X + vec4.Y + vec4.Z ) / 3 * ( maxHeight - minHeight ) ) + minHeight;
				}
			}
		}
	}

	public float GetHeight( float x, float y ) {
		x /= this._metersPerPixel;
		y /= this._metersPerPixel;
		if ( x < 0 || y < 0 )
			return 0;
		if ( x >= this._width || y >= this._height )
			return 0;
		int xi = (int) MathF.Floor( x );
		int yi = (int) MathF.Floor( y );
		float interpX = ToCosineInterpolation( x - xi );
		float interpY = ToCosineInterpolation( y - yi );

		float height00 = GetHeight( xi, yi );
		float height01 = GetHeight( xi, yi + 1 );
		float height10 = GetHeight( xi + 1, yi );
		float height11 = GetHeight( xi + 1, yi + 1 );

		float heighty0 = ( height00 * ( 1 - interpX ) ) + ( height10 * interpX );
		float heighty1 = ( height01 * ( 1 - interpX ) ) + ( height11 * interpX );

		return ( heighty0 * ( 1 - interpY ) ) + ( heighty1 * interpY );
	}

	public float ToCosineInterpolation( float linear ) => .5f - ( MathF.Cos( linear * MathF.PI ) * .5f );

	public float GetHeight( int xi, int yi ) {
		if ( xi < 0 || yi < 0 )
			return 0;
		if ( xi >= this._width || yi >= this._height )
			return 0;
		return this._heights[ GetIndex( xi, yi ) ];
	}

	private int GetIndex( int x, int y ) => ( y * this._width ) + x;
	private int GetRenderIndex( int x, int y, int width ) => ( y * width ) + x;

	internal BufferedMesh CreateMesh( float sizePerPolygon ) {
		int renderWidth = (int) MathF.Round( this._width * this._metersPerPixel / sizePerPolygon );
		int renderHeight = (int) MathF.Round( this._height * this._metersPerPixel / sizePerPolygon );
		Vertex3[] vertices = new Vertex3[ renderWidth * renderHeight ];
		uint[] indices = new uint[ ( renderWidth - 1 ) * ( renderHeight - 1 ) * 6 ];

		for ( int yi = 0; yi < renderHeight; yi++ ) {
			float y = yi * sizePerPolygon;
			for ( int xi = 0; xi < renderWidth; xi++ ) {
				float x = xi * sizePerPolygon;
				vertices[ GetRenderIndex( xi, yi, renderWidth ) ] = new Vertex3( new Vector3( x, GetHeight( x, y ), y ), new Vector2( x, y ) / new Vector2( this._width * this._metersPerPixel, this._height * this._metersPerPixel ) );
			}
		}

		int i = 0;
		for ( int y = 0; y < renderHeight - 1; y++ ) {
			for ( int x = 0; x < renderWidth - 1; x++ ) {
				uint index00 = (uint) GetRenderIndex( x, y, renderWidth );
				uint index01 = (uint) GetRenderIndex( x, y + 1, renderWidth );
				uint index10 = (uint) GetRenderIndex( x + 1, y, renderWidth );
				uint index11 = (uint) GetRenderIndex( x + 1, y + 1, renderWidth );
				indices[ i++ ] = index00;
				indices[ i++ ] = index01;
				indices[ i++ ] = index10;
				indices[ i++ ] = index10;
				indices[ i++ ] = index01;
				indices[ i++ ] = index11;
			}
		}

		return new VertexMesh<Vertex3>( "heightmap", vertices, indices ) { AutoSaved = false };
	}
}
