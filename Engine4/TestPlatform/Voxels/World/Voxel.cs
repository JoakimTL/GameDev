using System.Numerics;
using Engine.Rendering.Colors;

namespace TestPlatform.Voxels.World;

public class Voxel {

	public const uint MaxVoxelCount = ushort.MaxValue + 1;
	private static readonly List<Voxel> _voxels;

	public static readonly Voxel Air;
	public static readonly Voxel Stone;
	public static readonly Voxel Clay;
	public static event Action<ushort>? Changed;

	public static uint VoxelCount => (uint) _voxels.Count;

	static Voxel() {
		_voxels = new List<Voxel>();
		Air = new Voxel( Color8x4.Zero ) {
			Transparent = true
		};
		Stone = new Voxel( new Vector4( 0.6f, 0.6f, 0.6f, 1 ) ) {
			Metallic = 0.01f,
			Roughness = 0.75f
		};
		Clay = new Voxel( new Vector4( 0.9f, 0.5f, 0.3f, 1 ) ) {
			Metallic = 0.01f,
			Roughness = 0.75f
		};
	}

	public static Voxel? Get( ushort id ) {
		if ( id < _voxels.Count )
			return _voxels[ id ];
		return null;
	}

	private Color8x4 _diffuseColor;
	private Color8x3 _glowColor;
	private float _roughnesss;
	private float _metallic;
	public readonly ushort Id;
	public float Resistance { get; protected set; }
	public bool Transparent { get; protected set; }
	public Color8x4 DiffuseColor {
		get => this._diffuseColor;
		protected set {
			if ( value == this._diffuseColor )
				return;
			this._diffuseColor = value;
			Changed?.Invoke( this.Id );
		}
	}
	public Color8x3 GlowColor {
		get => this._glowColor;
		protected set {
			if ( value == this._glowColor )
				return;
			this._glowColor = value;
			Changed?.Invoke( this.Id );
		}
	}
	public float Roughness {
		get => this._roughnesss;
		protected set {
			if ( value == this._roughnesss )
				return;
			this._roughnesss = value;
			Changed?.Invoke( this.Id );
		}
	}
	public float Metallic {
		get => this._metallic;
		protected set {
			if ( value == this._metallic )
				return;
			this._metallic = value;
			Changed?.Invoke( this.Id );
		}
	}

	public VoxelData GetRenderData() => new() { DiffuseColor = this._diffuseColor, GlowColor = this._glowColor, Metallic = this._metallic, Roughness = this.Roughness };

	public Voxel( Color8x4 color ) {
		lock ( _voxels ) {
			this.Id = (ushort) _voxels.Count;
			_voxels.Add( this );
		}
		this.DiffuseColor = color;
		this.GlowColor = Color8x3.Black;
		this.Resistance = 1;
		this.Transparent = false;
		this.Roughness = 0.2f;
		this.Metallic = 0.05f;
		Changed?.Invoke( this.Id );
	}

}
