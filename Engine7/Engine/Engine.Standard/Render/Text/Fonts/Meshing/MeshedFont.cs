using Engine.Logging;
using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Standard.Render.Text.Fonts.Meshing;

public sealed class MeshedFont : DisposableIdentifiable {
	private readonly MeshService _meshService;
	public Font Font { get; }

	private readonly bool[] _loaded = new bool[ ushort.MaxValue ];
	private readonly GlyphMesh?[] _cachedMeshes = new GlyphMesh?[ ushort.MaxValue ];

	public MeshedFont( Font font, MeshService meshService ) {
		this._meshService = meshService;
		this.Font = font;
		Nickname = font.Path;
	}

	public GlyphMesh? this[ char c ] => GetOrCreateGlyphMesh( c );

	private GlyphMesh? GetOrCreateGlyphMesh( char c ) {
		GlyphMesh? mesh = this._cachedMeshes[ c ];
		if (mesh is not null || _loaded[ c ])
			return mesh;

		IGlyph glyph = this.Font[ c ];
		GlyphTriangle[] triangles = glyph.TriangulateGlyph();
		_loaded[ c ] = true;
		this.LogLine( $"Loaded mesh for {c}.", Log.Level.VERBOSE );
		if (triangles.Length == 0)
			return null;
		mesh = new( glyph, CreateMesh( c.ToString(), glyph.TriangulateGlyph() ) );
		this._cachedMeshes[ c ] = mesh;
		return mesh;
	}

	private VertexMesh<GlyphVertex> CreateMesh( string name, GlyphTriangle[] triangles ) {
		List<GlyphVertex> vertices = [];
		List<uint> indices = [];
		foreach (GlyphTriangle t in triangles) {
			uint index = (uint) vertices.Count;
			vertices.Add( new( t.Triangle.A, 0, 255, t.Filled, t.Flipped ) );
			vertices.Add( new( t.Triangle.B, (0.5f, 0), 255, t.Filled, t.Flipped ) );
			vertices.Add( new( t.Triangle.C, 1, 255, t.Filled, t.Flipped ) );
			indices.Add( index );
			indices.Add( index + 1 );
			indices.Add( index + 2 );
		}
		return this._meshService.CreateMesh( vertices.ToArray(), indices.ToArray(), name );
	}

	protected override bool InternalDispose() {
		foreach (GlyphMesh? mesh in this._cachedMeshes)
			mesh?.Dispose();
		return true;
	}
}
